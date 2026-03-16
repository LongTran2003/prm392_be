using FoodOrderSystem.Models.DTOs;
using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;

namespace FoodOrderSystem.Services.IServices
{
    public interface IGoogleAuthService
    {
        Task<ResponseDto> GoogleLoginCustomer(GoogleLoginDto dto);
    }
}
