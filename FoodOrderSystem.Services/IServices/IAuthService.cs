using FoodOrderSystem.Models.DTOs.Authentication;
using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface IAuthService
    {
        // Student login with Google ID Token
        Task<ApiResponseDto<TokenResponseDto>> StudentLoginAsync(GoogleLoginDto loginDto);

        // ShopOwner/Admin login with email and password
        Task<ApiResponseDto<TokenResponseDto>> LoginAsync(SignInDto signInDto);

        // ShopOwner registration
        Task<ApiResponseDto<string>> RegisterShopOwnerAsync(RegisterShopOwnerDto registerDto);

        // Refresh token
        Task<ApiResponseDto<TokenResponseDto>> RefreshTokenAsync(string refreshToken);

        // Get user profile
        Task<ApiResponseDto<GetUserResponseDto>> GetUserProfileAsync(string userId);

        // Check if phone number exists
        Task<ApiResponseDto<bool>> CheckPhoneNumberExistsAsync(string phoneNumber);
    }
}
