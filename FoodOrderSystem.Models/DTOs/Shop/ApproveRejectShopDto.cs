using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Admin approve or reject shop
    /// Android: AdminShopListFragment.java - POST /api/v1/shops/approve-reject
    /// </summary>
    public class ApproveRejectShopDto
    {
        [Required]
        public Guid ShopId { get; set; }

        [Required]
        public bool IsApproved { get; set; }
    }
}
