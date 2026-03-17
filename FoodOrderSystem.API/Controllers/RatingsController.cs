using FoodOrderSystem.Models.DTOs.Rating;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/ratings")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// POST /api/ratings (Multipart)
        /// Create rating/review with optional images
        /// Form fields: shopId, starRating, comment
        /// Files: images (optional, multiple)
        /// Called from: CustomerFeedbackFragment.java
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRating(
            [FromForm] CreateRatingDto createDto,
            [FromForm] List<IFormFile>? images)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var result = await _ratingService.CreateRatingAsync(customerId, createDto, images);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// GET /api/ratings/{shopId}
        /// Get all ratings for a shop with pagination
        /// Query params: pageIndex (default 1), pageSize (default 10)
        /// Called from: ShopDetailFragment.java
        /// </summary>
        [HttpGet("{shopId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShopRatings(
            Guid shopId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _ratingService.GetShopRatingsAsync(shopId, pageIndex, pageSize);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// GET /api/ratings/summary/{shopId}
        /// Get rating summary with average, breakdown, and recent ratings
        /// Called from: ShopDetailFragment.java
        /// </summary>
        [HttpGet("summary/{shopId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRatingSummary(Guid shopId)
        {
            var result = await _ratingService.GetRatingSummaryAsync(shopId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// PUT /api/ratings/{ratingId}
        /// Update rating by customer
        /// Form fields: shopId, starRating, comment
        /// Called from: RatingDetailFragment.java
        /// </summary>
        [HttpPut("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> UpdateRating(
            Guid ratingId,
            [FromForm] CreateRatingDto updateDto)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var result = await _ratingService.UpdateRatingAsync(customerId, ratingId, updateDto);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// DELETE /api/ratings/{ratingId}
        /// Delete rating by customer
        /// Called from: RatingDetailFragment.java
        /// </summary>
        [HttpDelete("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRating(Guid ratingId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var result = await _ratingService.DeleteRatingAsync(customerId, ratingId);
            return StatusCode(result.Success ? 200 : 404, result);
        }
    }
}
