namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Shop detail with nested categories and menu items
    /// Android: ShopDetailFragment.java:181 - GET /api/v1/shops/detail/{shopId}
    /// Must include: categories[] with nested menuItems[]
    /// </summary>
    public class ShopDetailResponseDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? BusinessLicenseImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OpenHours { get; set; } = null!;
        public string CloseHours { get; set; } = null!;
        public int TotalOrders { get; set; }
        public double AverageRating { get; set; }
        public List<CategoryWithMenuItemsDto> Categories { get; set; } = new List<CategoryWithMenuItemsDto>();
    }
}
