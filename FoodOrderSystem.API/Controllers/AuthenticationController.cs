using FoodOrderSystem.Models.DTOs.Authentication;
using FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin;
using FoodOrderSystem.Services.IServices;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]

    public partial class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthenticationController(
            IAuthService authService,
            IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _googleAuthService.GoogleLoginCustomer(dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Student login with Google ID Token
        /// POST /api/v1/Authentications/student-login
        /// Request: { idToken: "google_id_token" }
        /// </summary>
        [HttpPost("student-login")]
        [AllowAnonymous]
        public async Task<IActionResult> StudentLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _authService.StudentLoginAsync(dto);
            return StatusCode(result.Success ? 200 : 401, result);
        }

        /// <summary>
        /// ShopOwner/Admin login with email and password
        /// POST /api/v1/Authentications/login
        /// Request: { email: "user@example.com", password: "password" }
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] SignInDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return StatusCode(result.Success ? 200 : 401, result);
        }

        /// <summary>
        /// Register new ShopOwner account
        /// POST /api/v1/Authentications/register-shop-owner
        /// </summary>
        [HttpPost("register-shop-owner")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterShopOwner([FromBody] RegisterShopOwnerDto dto)
        {
            var result = await _authService.RegisterShopOwnerAsync(dto);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// Refresh access token
        /// POST /api/v1/Authentications/refresh-token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return StatusCode(result.Success ? 200 : 401, result);
        }
    }
}
