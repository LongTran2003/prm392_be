using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Payment
{
    /// <summary>
    /// Payment result callback from PayOS gateway
    /// Android: PaymentFragment.java:94, 101
    /// POST /api/v1/Payments/payment-result
    /// </summary>
    public class PaymentResultRequestDto
    {
        [Required]
        [StringLength(100)]
        public string OrderCode { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!;  // PAID, FAILED, CANCELLED
    }
}
