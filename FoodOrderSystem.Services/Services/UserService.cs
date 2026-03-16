using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderSystem.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// GET /api/v1/Users/{userId}
        /// Get user profile by ID - returns user with all required fields
        /// Called after login: LoginFragment.java:141
        /// </summary>
        public async Task<ApiResponseDto<GetUserResponseDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ApiResponseDto<GetUserResponseDto>
                    {
                        Success = false,
                        Message = "User ID is required",
                        MessageId = "INVALID_USER_ID"
                    };
                }

                var user = await _unitOfWork.User.GetUserWithRolesAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<GetUserResponseDto>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Student";

                // Map to response
                var userResponse = new GetUserResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Address = user.Address,
                    FullName = user.FullName,
                    Avatar = user.ImageUrl,
                    RoleName = userRole,
                    UserName = user.UserName,
                    BirthDate = user.BirthDate.ToString("yyyy-MM-dd")
                };

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
        /// PUT /api/v1/Users
        /// Update user profile (address, phone, avatar, etc.)
        /// Called from: ProfileFragment.java (after phone verification OTP)
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "User ID is required",
                        MessageId = "INVALID_USER_ID"
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                // Check if new phone already exists (if phone changed)
                if (!string.IsNullOrEmpty(updateDto.PhoneNumber) &&
                    updateDto.PhoneNumber != user.PhoneNumber)
                {
                    var phoneExists = await _unitOfWork.Auth.DoesPhoneNumberExistAsync(updateDto.PhoneNumber);
                    if (phoneExists)
                    {
                        return new ApiResponseDto<string>
                        {
                            Success = false,
                            Message = "Phone number already registered",
                            MessageId = "PHONE_EXISTS"
                        };
                    }
                }

                // Update user fields
                user.FullName = updateDto.FullName;
                user.BirthDate = updateDto.BirthDate;
                user.PhoneNumber = updateDto.PhoneNumber;
                user.Address = updateDto.Address;
                user.Gender = updateDto.Gender;
                user.ImageUrl = updateDto.ImageUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Update failed: {errors}",
                        MessageId = "UPDATE_FAILED"
                    };
                }

                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "User profile updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Update failed: {ex.Message}",
                    MessageId = "UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Users/checkPhoneNumberExists?phoneNumber={phone}
        /// Check if phone number is already registered
        /// Used during registration form validation
        /// Android: PhoneInputFragment.java → UserViewModel.startPhoneVerification()
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

        /// <summary>
        /// GET /api/v1/Users/all-cus
        /// Get all student/customer users (for ShopOwner to view customers)
        /// Called from: ShopDetailFragment.java:108 → UserViewModel.getAllCustomers()
        /// Android expects: List<CustomerResponse>
        /// </summary>
        public async Task<ApiResponseDto<List<CustomerResponseDto>>> GetAllCustomersAsync()
        {
            try
            {
                var students = await _unitOfWork.User.GetAllStudentsAsync();

                var customerList = students.Select(s => new CustomerResponseDto
                {
                    UserId = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.PhoneNumber,
                    Address = s.Address,
                    Avatar = s.ImageUrl,
                    BirthDate = s.BirthDate.ToString("yyyy-MM-dd"),
                    Gender = s.Gender
                }).ToList();

                return new ApiResponseDto<List<CustomerResponseDto>>
                {
                    Success = true,
                    Message = "Customers retrieved successfully",
                    Data = customerList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<CustomerResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve customers: {ex.Message}",
                    MessageId = "CUSTOMERS_FETCH_FAILED"
                };
            }
        }
    }
}
