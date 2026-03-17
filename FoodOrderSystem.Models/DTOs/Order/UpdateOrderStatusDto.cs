using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Order
{
    /// <summary>
    /// Update order status (ShopOwner only)
    /// Android: ShopOwner views orders and updates status
    /// PUT /api/v1/Orders/{orderId}/status
    /// </summary>
    public class UpdateOrderStatusDto
    {
        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = null!;  // Confirmed, Delivering, Completed, Cancelled
    }
}
