using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]

    public partial class AuthenticationController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;

        public AuthenticationController(
            /* existing injected services */
            IGoogleAuthService googleAuthService)
        {
            /* existing constructor assignments */
            _googleAuthService = googleAuthService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _googleAuthService.GoogleLoginCustomer(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
