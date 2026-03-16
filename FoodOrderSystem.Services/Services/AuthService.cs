using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Authentication;
using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService
        (
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ITokenService tokenService,
            IEmailService emailService
        )
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        /// <summary>
        /// Student login using Google ID Token
        /// Request: { idToken }
        /// Response: { accessToken, refreshToken, userRole: "Student", userId }
        /// </summary>
        public async Task<ApiResponseDto<TokenResponseDto>> StudentLoginAsync(GoogleLoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.IdToken))
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "ID token is required",
                        MessageId = "INVALID_TOKEN"
                    };
                }

                // TODO: Verify Google ID Token signature here
                // For now, we'll skip verification in development
                // In production, call Google's tokeninfo endpoint or verify locally

                // For development, we'll extract email from token claims
                // In production, properly verify the token first
                var payload = GoogleJsonWebSignature.ValidateAsync(loginDto.IdToken).Result;

                var email = payload.Email;
                var fullName = payload.Name;

                // Find or create user
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create new Student user
                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        FullName = fullName ?? email.Split('@')[0],
                        BirthDate = DateTime.UtcNow,
                        Status = "Active"
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new ApiResponseDto<TokenResponseDto>
                        {
                            Success = false,
                            Message = "Failed to create user account",
                            MessageId = "USER_CREATION_FAILED"
                        };
                    }

                    // Assign Student role
                    if (!await _roleManager.RoleExistsAsync("Student"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Student"));
                    }
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Create Student profile
                    var student = new Student
                    {
                        StudentId = Guid.NewGuid(),
                        UserId = user.Id,
                        StudentCode = $"STU{DateTime.UtcNow.Ticks}",
                        CreatedTime = DateTime.UtcNow,
                        Status = "Active"
                    };
                    await _unitOfWork.Student.AddAsync(student);
                    await _unitOfWork.SaveAsync();
                }

                // Generate tokens
                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
                await _tokenService.StoreRefreshToken(user.Id, refreshToken);

                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new TokenResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        UserRole = "Student",
                        UserId = user.Id
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}",
                    MessageId = "LOGIN_FAILED"
                };
            }
        }

        /// <summary>
        /// ShopOwner/Admin login with email and password
        /// Request: { identifier (email), password }
        /// Response: { accessToken, refreshToken, userRole: "ShopOwner" or "Admin", userId }
        /// </summary>
        public async Task<ApiResponseDto<TokenResponseDto>> LoginAsync(SignInDto signInDto)
        {
            try
            {
                if (string.IsNullOrEmpty(signInDto.Email) || string.IsNullOrEmpty(signInDto.Password))
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Email and password are required",
                        MessageId = "INVALID_CREDENTIALS"
                    };
                }

                // Find user by email
                var user = await _userManager.FindByEmailAsync(signInDto.Email);
                if (user == null)
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                // Verify password
                var passwordValid = await _userManager.CheckPasswordAsync(user, signInDto.Password);
                if (!passwordValid)
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Invalid password",
                        MessageId = "INVALID_PASSWORD"
                    };
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Student";

                // Only ShopOwner and Admin can login here
                if (!roles.Contains("ShopOwner") && !roles.Contains("Admin"))
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Only ShopOwner or Admin can login with password",
                        MessageId = "INVALID_ROLE"
                    };
                }

                // Generate tokens
                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
                await _tokenService.StoreRefreshToken(user.Id, refreshToken);

                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new TokenResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        UserRole = userRole,
                        UserId = user.Id
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}",
                    MessageId = "LOGIN_FAILED"
                };
            }
        }

        /// <summary>
        /// Register new ShopOwner account
        /// Request: { fullName, userName, email, password, phone, address?, avatar?, dob? }
        /// Response: { message }
        /// </summary>
        public async Task<ApiResponseDto<string>> RegisterShopOwnerAsync(RegisterShopOwnerDto registerDto)
        {
            try
            {
                // Check if email already exists
                if (await _unitOfWork.Auth.DoesEmailExistAsync(registerDto.Email))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Email already registered",
                        MessageId = "EMAIL_EXISTS"
                    };
                }

                // Check if username already exists
                if (await _unitOfWork.Auth.DoesUserNameExistAsync(registerDto.UserName))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Username already taken",
                        MessageId = "USERNAME_EXISTS"
                    };
                }

                // Check if phone already exists
                if (await _unitOfWork.Auth.DoesPhoneNumberExistAsync(registerDto.Phone))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Phone number already registered",
                        MessageId = "PHONE_EXISTS"
                    };
                }

                // Create new user
                var user = _mapper.Map<ApplicationUser>(registerDto);
                user.Email = registerDto.Email.ToLower();
                user.UserName = registerDto.UserName.ToLower();
                user.Status = "Active";

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Registration failed: {errors}",
                        MessageId = "REGISTRATION_FAILED"
                    };
                }

                // Assign ShopOwner role
                if (!await _roleManager.RoleExistsAsync("ShopOwner"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("ShopOwner"));
                }
                await _userManager.AddToRoleAsync(user, "ShopOwner");

                // Create ShopOwner profile
                var shopOwner = new ShopOwner
                {
                    ShopOwnerId = Guid.NewGuid(),
                    UserId = user.Id,
                    WalletBalance = 0,
                    CreatedTime = DateTime.UtcNow,
                    Status = "Active"
                };
                await _unitOfWork.ShopOwner.AddAsync(shopOwner);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "ShopOwner registered successfully. Please login to continue."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}",
                    MessageId = "REGISTRATION_FAILED"
                };
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        public async Task<ApiResponseDto<TokenResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Refresh token is required",
                        MessageId = "INVALID_TOKEN"
                    };
                }

                var principal = await _tokenService.GetPrincipalFromToken(refreshToken);
                var userId = principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                        MessageId = "INVALID_TOKEN"
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                var storedRefreshToken = await _tokenService.RetrieveRefreshTokenAsync(userId);
                if (storedRefreshToken != refreshToken)
                {
                    return new ApiResponseDto<TokenResponseDto>
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                        MessageId = "INVALID_TOKEN"
                    };
                }

                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var newRefreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
                await _tokenService.StoreRefreshToken(userId, newRefreshToken);

                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = new TokenResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = newRefreshToken,
                        UserRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Student",
                        UserId = user.Id
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TokenResponseDto>
                {
                    Success = false,
                    Message = $"Token refresh failed: {ex.Message}",
                    MessageId = "REFRESH_FAILED"
                };
            }
        }

        /// <summary>
        /// Get user profile by userId
        /// Returns all user information including wallet balance and role
        /// </summary>
        public async Task<ApiResponseDto<GetUserResponseDto>> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<GetUserResponseDto>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Student";

                var userResponse = _mapper.Map<GetUserResponseDto>(user);
                userResponse.RoleName = userRole;

                // Get wallet balance if ShopOwner
                if (userRole == "ShopOwner")
                {
                    var shopOwner = await _unitOfWork.ShopOwner
                        .GetAsync(so => so.UserId == userId);

                    if (shopOwner != null)
                    {
                        userResponse.WalletBalance = shopOwner.WalletBalance;
                    }
                }

                return new ApiResponseDto<GetUserResponseDto>
                {
                    Success = true,
                    Message = "User profile retrieved successfully",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<GetUserResponseDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve user profile: {ex.Message}",
                    MessageId = "PROFILE_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// Check if phone number already exists
        /// Used during registration form validation
        /// </summary>
        public async Task<ApiResponseDto<bool>> CheckPhoneNumberExistsAsync(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Phone number is required",
                        MessageId = "INVALID_PHONE"
                    };
                }

                var exists = await _unitOfWork.Auth.DoesPhoneNumberExistAsync(phoneNumber);

                return new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = exists ? "Phone number already exists" : "Phone number is available",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = $"Check failed: {ex.Message}",
                    MessageId = "CHECK_FAILED"
                };
            }
        }
    }
}
