using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Order
{
    /// <summary>
    /// Create order request from customer
    /// Android: CartFragment.java:150 (COD) or :158 (Bank)
    /// POST /api/v1/Orders/create
    /// </summary>
    public class CreateOrderRequestDto
    {
        [Required]
        public Guid ShopId { get; set; }

        [Required]
        public List<OrderItemRequestDto> OrderItems { get; set; } = new List<OrderItemRequestDto>();

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "COD";  // COD, Bank

        [StringLength(500)]
        public string? Notes { get; set; }

        public class OrderItemRequestDto
        {
            [Required]
            public Guid MenuItemId { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }
        }
    }
}
