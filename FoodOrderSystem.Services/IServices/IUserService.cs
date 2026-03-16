using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface IUserService
    {
        // GET /api/v1/Users/{userId}
        Task<ApiResponseDto<GetUserResponseDto>> GetUserByIdAsync(string userId);

        // PUT /api/v1/Users
        Task<ApiResponseDto<string>> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto);

        // GET /api/v1/Users/checkPhoneNumberExists?phoneNumber={phone}
        Task<ApiResponseDto<bool>> CheckPhoneNumberExistsAsync(string phoneNumber);

        // GET /api/v1/Users/all-cus
        Task<ApiResponseDto<List<CustomerResponseDto>>> GetAllCustomersAsync();
    }
}
