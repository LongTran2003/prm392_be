namespace FoodOrderSystem.Models.DTOs.Rating
{
    /// <summary>
    /// Rating response
    /// GET /api/v1/Ratings/{shopId}
    /// Shows all reviews for a shop
    /// </summary>
    public class RatingResponseDto
    {
        public Guid RatingId { get; set; }
        public Guid ShopId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? CustomerAvatar { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }
}
