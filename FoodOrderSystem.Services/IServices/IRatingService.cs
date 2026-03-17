using FoodOrderSystem.Models.DTOs.Rating;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.IServices
{
    public interface IRatingService
    {
        // POST /api/v1/Ratings
        Task<ApiResponseDto<RatingResponseDto>> CreateRatingAsync(string customerId, CreateRatingDto createDto, List<IFormFile>? images);

        // GET /api/v1/Ratings/{shopId}
        Task<ApiResponseDto<List<RatingResponseDto>>> GetShopRatingsAsync(Guid shopId, int pageIndex = 1, int pageSize = 10);

        // GET /api/v1/Ratings/summary/{shopId}
        Task<ApiResponseDto<ShopRatingSummaryDto>> GetRatingSummaryAsync(Guid shopId);

        // PUT /api/v1/Ratings/{ratingId}
        Task<ApiResponseDto<string>> UpdateRatingAsync(string customerId, Guid ratingId, CreateRatingDto updateDto);

        // DELETE /api/v1/Ratings/{ratingId}
        Task<ApiResponseDto<string>> DeleteRatingAsync(string customerId, Guid ratingId);
    }
}
