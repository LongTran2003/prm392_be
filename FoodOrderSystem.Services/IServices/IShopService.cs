using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Models.DTOs.Shop;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.IServices
{
    public interface IShopService
    {
        // GET /api/v1/shops/popular
        Task<ApiResponseDto<List<PopularShopResponseDto>>> GetPopularShopsAsync();

        // GET /api/v1/shops/detail/{shopId}
        Task<ApiResponseDto<ShopDetailResponseDto>> GetShopDetailAsync(Guid shopId);

        // GET /api/v1/shops/shop-owner
        Task<ApiResponseDto<PagingResponseDto<GetShopResponseDto>>> GetShopsByOwnerAsync(string ownerId, int pageIndex, int pageSize);

        // GET /api/v1/shops/status/{status}
        Task<ApiResponseDto<PagingResponseDto<GetShopResponseDto>>> GetShopsByStatusAsync(string status, int pageIndex, int pageSize);

        // POST /api/v1/shops (Multipart)
        Task<ApiResponseDto<GetShopResponseDto>> CreateShopAsync(string ownerId, CreateShopDto createDto, IFormFile? image, IFormFile? businessLicense);

        // PUT /api/v1/shops (Multipart)
        Task<ApiResponseDto<string>> UpdateShopAsync(string ownerId, UpdateShopDto updateDto, IFormFile? image, IFormFile? businessLicense);

        // DELETE /api/v1/shops
        Task<ApiResponseDto<string>> DeleteShopAsync(Guid shopId, string ownerId);

        // POST /api/v1/shops/approve-reject (Admin)
        Task<ApiResponseDto<string>> ApproveRejectShopAsync(Guid shopId, bool isApproved);
    }
}
