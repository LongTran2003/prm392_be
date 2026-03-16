using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface IGoogleAuthService
    {
        Task<ResponseDto> GoogleLoginCustomer(GoogleLoginDto dto);
    }
}
