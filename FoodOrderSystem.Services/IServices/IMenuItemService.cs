using FoodOrderSystem.Models.DTOs.MenuItem;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.IServices
{
    public interface IMenuItemService
    {
        // GET /api/v1/MenuItem?shopId={guid}
        Task<ApiResponseDto<List<GetMenuItemResponseDto>>> GetMenuItemsByShopAsync(Guid shopId);

        // POST /api/v1/MenuItem (Multipart)
        Task<ApiResponseDto<GetMenuItemResponseDto>> CreateMenuItemAsync(string ownerId, CreateMenuItemDto createDto, IFormFile? image);

        // PUT /api/v1/MenuItem/{menuItemId} (Multipart)
        Task<ApiResponseDto<string>> UpdateMenuItemAsync(string ownerId, UpdateMenuItemDto updateDto, IFormFile? image);

        // DELETE /api/v1/MenuItem/{menuItemId}
        Task<ApiResponseDto<string>> DeleteMenuItemAsync(string ownerId, Guid menuItemId);
    }   
}
