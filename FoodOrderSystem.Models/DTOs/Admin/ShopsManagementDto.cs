namespace FoodOrderSystem.Models.DTOs.Admin
{
    /// <summary>
    /// For shop approval/rejection workflow
    /// GET /api/v1/Admin/shops/pending
    /// </summary>
    public class ShopsManagementDto
    {
        public List<PendingShopDto> PendingShops { get; set; } = new();
        public List<RejectedShopDto> RejectedShops { get; set; } = new();
        public int TotalApprovedShops { get; set; }
    }

    public class RejectedShopDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public DateTime RejectedDate { get; set; }
        public string ReasonRejected { get; set; } = "Shop does not meet requirements";
    }
}
