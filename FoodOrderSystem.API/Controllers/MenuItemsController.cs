using FoodOrderSystem.Models.DTOs.MenuItem;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static FoodOrderSystem.Utilities.Constants.StaticOperationStatus;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/menu-items")]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// GET /api/v1/MenuItem?shopId={guid}
        /// Get all menu items for a shop
        /// Called from: ShopDetailFragment.java:181
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenuItemsByShop([FromQuery] Guid shopId)
        {
            var result = await _menuItemService.GetMenuItemsByShopAsync(shopId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// POST /api/v1/MenuItem (Multipart)
        /// Create new menu item
        /// Form fields: menuItemName, description, price, categoryId, shopId, isAvailable
        /// Files: image
        /// Called from: ShopFormFragment.java
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> CreateMenuItem(
            [FromForm] CreateMenuItemDto createDto)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _menuItemService.CreateMenuItemAsync(ownerId, createDto, createDto.Image);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// PUT /api/v1/MenuItem/{menuItemId} (Multipart)
        /// Update menu item - image is optional
        /// Form fields: menuItemId, menuItemName, description, price, categoryId, shopId, isAvailable
        /// Files: image (optional)
        /// Called from: ShopFormFragment.java
        /// </summary>
        [HttpPut("{menuItemId}")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> UpdateMenuItem(
            Guid menuItemId,
            [FromForm] UpdateMenuItemDto updateDto)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            // Ensure menuItemId in URL matches DTO
            updateDto.MenuItemId = menuItemId;

            var result = await _menuItemService.UpdateMenuItemAsync(ownerId, updateDto, updateDto.Image);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// DELETE /api/v1/MenuItem/{menuItemId}
        /// Delete menu item (owner only)
        /// Called from: ShopFormFragment.java
        /// </summary>
        [HttpDelete("{menuItemId}")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> DeleteMenuItem(Guid menuItemId)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var result = await _menuItemService.DeleteMenuItemAsync(ownerId, menuItemId);
            return StatusCode(result.Success ? 200 : 404, result);
        }
    }
}
