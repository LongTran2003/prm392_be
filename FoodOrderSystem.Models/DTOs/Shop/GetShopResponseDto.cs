namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Shop info for listing (used in shop-owner, status filtering)
    /// Android: MyShopListFragment.java, AdminShopListFragment.java
    /// </summary>
    public class GetShopResponseDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = null!;
        public string Status { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OpenHours { get; set; } = null!;
        public string CloseHours { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int TotalOrders { get; set; }
        public double AverageRating { get; set; }
    }
}
