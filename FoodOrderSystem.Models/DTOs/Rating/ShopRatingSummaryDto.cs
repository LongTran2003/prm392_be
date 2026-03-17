namespace FoodOrderSystem.Models.DTOs.Rating
{
    /// <summary>
    /// Shop rating summary
    /// GET /api/v1/Ratings/summary/{shopId}
    /// Shows average rating and breakdown
    /// </summary>
    public class ShopRatingSummaryDto
    {
        public Guid ShopId { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new();  // {1: 10, 2: 5, 3: 20, 4: 30, 5: 40}
        public List<RatingResponseDto> RecentRatings { get; set; } = new();  // Top 10
    }
}
