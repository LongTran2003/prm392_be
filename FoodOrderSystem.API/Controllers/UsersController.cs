using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// GET /api/v1/Users/{userId}
        /// Get user profile by ID
        /// Returns: { success, messageId, message, data: { userId, email, phone, address, fullName, avatar, walletBalance, roleName } }
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// PUT /api/v1/Users
        /// Update user profile
        /// Request: { fullName, birthDate, phoneNumber, address, gender, imageUrl }
        /// Returns: { success, messageId, message }
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            // Get userId from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userService.UpdateUserProfileAsync(userId, updateDto);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// GET /api/v1/Users/checkPhoneNumberExists?phoneNumber={phone}
        /// Check if phone number already registered
        /// Returns: { success, messageId, message, data: bool }
        /// </summary>
        [HttpGet("checkPhoneNumberExists")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPhoneNumberExists([FromQuery] string phoneNumber)
        {
            var result = await _userService.CheckPhoneNumberExistsAsync(phoneNumber);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// GET /api/v1/Users/all-cus
        /// Get all customers (Students)
        /// Used when ShopOwner wants to view customer list
        /// Returns: { success, messageId, message, data: [ { userId, fullName, email, phone, address, avatar, birthDate, gender }, ... ] }
        /// </summary>
        [HttpGet("all-cus")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _userService.GetAllCustomersAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }
    }
