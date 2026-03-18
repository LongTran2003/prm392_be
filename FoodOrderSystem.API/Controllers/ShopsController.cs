using FoodOrderSystem.Models.DTOs.Shop;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/shops")]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
        }

        /// <summary>
        /// GET /api/v1/shops/popular?currentTime={HH:mm:ss}
        /// Get popular shops for home screen
        /// Called from: HomeFragment.java:246
        /// </summary>
        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularShops()
        {
            var result = await _shopService.GetPopularShopsAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/shops/detail/{shopId}
        /// Get shop detail with nested categories and menu items
        /// Called from: ShopDetailFragment.java:181
        /// </summary>
        [HttpGet("detail/{shopId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShopDetail(Guid shopId)
        {
            var result = await _shopService.GetShopDetailAsync(shopId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// GET /api/v1/shops/shop-owner?PageIndex={int}&PageSize={int}
        /// Get shops owned by current user
        /// Called from: MyShopListFragment.java:139
        /// </summary>
        [HttpGet("shop-owner")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> GetShopsByOwner(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _shopService.GetShopsByOwnerAsync(ownerId, pageIndex, pageSize);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// GET /api/v1/shops/status/{status}?PageIndex={int}&PageSize={int}
        /// Get shops by status (Admin only)
        /// Status: Pending, Approved, Rejected
        /// Called from: AdminShopListFragment.java:108
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetShopsByStatus(
            string status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _shopService.GetShopsByStatusAsync(status, pageIndex, pageSize);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// POST /api/v1/shops (Multipart)
        /// Create new shop with image upload
        /// Form fields: shopName, description, address, latitude, longitude, openHours, closeHours
        /// Files: image, businessLicenseImage
        /// Called from: ShopFormFragment.java:251
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> CreateShop([FromForm] CreateShopDto createDto)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _shopService.CreateShopAsync(ownerId, createDto, createDto.Image, createDto.BusinessLicenseImage);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// PUT /api/v1/shops (Multipart)
        /// Update shop - files are optional
        /// Form fields: shopId, shopName, description, address, latitude, longitude, openHours, closeHours
        /// Files: image, businessLicenseImage (optional)
        /// Called from: ShopFormFragment.java:249
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> UpdateShop([FromForm] UpdateShopDto updateDto)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _shopService.UpdateShopAsync(ownerId, updateDto, updateDto.Image, updateDto.BusinessLicenseImage);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// DELETE /api/v1/shops?shopId={shopId}
        /// Delete shop (owner only)
        /// Called from: MyShopListFragment.java:71
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> DeleteShop([FromQuery] Guid shopId)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _shopService.DeleteShopAsync(shopId, ownerId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// POST /api/v1/shops/approve-reject
        /// Approve or reject shop (Admin only)
        /// Request: { shopId, isApproved }
        /// Called from: AdminShopListFragment.java:112, 116
        /// </summary>
        [HttpPost("approve-reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRejectShop([FromBody] ApproveRejectShopDto approveDto)
        {
            var result = await _shopService.ApproveRejectShopAsync(approveDto.ShopId, approveDto.IsApproved);
            return StatusCode(result.Success ? 200 : 400, result);
        }
    }
}
