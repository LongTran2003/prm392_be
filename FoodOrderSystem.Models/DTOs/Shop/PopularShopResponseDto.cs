namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Popular shops for home screen
    /// Android: HomeFragment.java:246 - GET /api/v1/shops/popular?currentTime={HH:mm:ss}
    /// Returns: List (NOT wrapped in ApiResponse per Android expectations)
    /// </summary>
    public class PopularShopResponseDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OpenHours { get; set; } = null!;
        public string CloseHours { get; set; } = null!;
        public int TotalOrders { get; set; }
        public double AverageRating { get; set; }
        public string? Description { get; set; }
    }
}
