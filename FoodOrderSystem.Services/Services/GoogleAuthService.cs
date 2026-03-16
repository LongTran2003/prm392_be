using AutoMapper;
using Google.Apis.Auth;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GoogleAuthService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GoogleLoginCustomer(GoogleLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.IdToken))
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Missing idToken",
                    StatusCode = 400,
                    Result = null
                };
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] } // ensure this is set in config
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid Google idToken: " + ex.Message,
                    StatusCode = 401,
                    Result = null
                };
            }

            // email from Google payload
            var email = payload.Email?.ToLowerInvariant();
            if (string.IsNullOrEmpty(email))
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Google token does not contain email",
                    StatusCode = 400,
                    Result = null
                };
            }

            // find or create application user
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            var isNewUser = false;

            if (user == null)
            {
                // Create ApplicationUser from Google payload
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = payload.Name ?? string.Empty,
                    ImageUrl = payload.Picture,
                    // Set defaults for fields that exist on ApplicationUser in your project:
                    // e.g. BirthDate = DateTime.UtcNow, PhoneNumber = null, etc.
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Create user failed: " + string.Join("; ", createResult.Errors.Select(e => e.Description)),
                        StatusCode = 500,
                        Result = null
                    };
                }

                isNewUser = true;
            }

            // Ensure user has Customer role + Customer entity record
            var customerRole = "Customer";
            // Create role if needed (requires RoleManager in DI). If your system uses a static helper, use that.
            try
            {
                // Add role to user if not already
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(customerRole))
                {
                    await _userManager.AddToRoleAsync(user, customerRole);
                }
            }
            catch
            {
                // swallow role creation error here (role creation typically handled on startup)
            }

            // If new user, create Customer entity linked to this ApplicationUser (only if Customer repository exists)
            if (isNewUser)
            {
                try
                {
                    // If your IUnitOfWork exposes Customer repository, adapt as needed.
                    var customer = new Student
                    {
                        StudentId = Guid.NewGuid(),
                        UserId = user.Id,
                        CreatedTime = StaticOperationStatus.Timezone.Vietnam,
                        Status = StaticOperationStatus.BaseEntity.Active
                    };

                    await _unitOfWork.Student.AddAsync(customer);
                    await _unitOfWork.SaveAsync();
                }
                catch
                {
                    // Non-fatal: user exists; if creating Customer fails, continue but log in real app
                }
            }

            // Generate tokens
            var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
            var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
            await _tokenService.StoreRefreshToken(user.Id, refreshToken);

            var response = new GoogleLoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserRole = customerRole
            };

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Google login successful",
                StatusCode = 200,
                Result = response
            };
        }
    }
}
