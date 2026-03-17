using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Payment
{
    /// <summary>
    /// Create payment request for bank transfer/QR code
    /// Android: CartFragment.java:158 → POST /api/v1/Payments/create-payment
    /// Request body contains order details for QR code generation
    /// </summary>
    public class CreatePaymentRequestDto
    {
        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        public Guid ShopId { get; set; }

        [Required]
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        [Required]
        public string OrderStatus { get; set; } = "Pending";  // Pending, Confirmed, Delivered, Completed

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Bank";  // COD, Bank

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string? PayosOrderCode { get; set; }  // Payos transaction code if exists

        public class OrderItemDto
        {
            [Required]
            public Guid MenuItemId { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }

            [Required]
            [Range(0.01, double.MaxValue)]
            public decimal Price { get; set; }
        }
    }
}
