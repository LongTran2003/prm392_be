using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirebaseOrderId { get; set; } = null!;  // Reference to Firebase if hybrid

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        public Guid ShopId { get; set; }

        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; } = null!;

        [StringLength(100)]
        public string? PayosOrderCode { get; set; }  // PayOS transaction code

        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending";  // Pending, Confirmed, Delivering, Completed, Cancelled

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "COD";  // COD, Bank

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending";  // Pending, Paid, Failed

        [Required]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? PaidDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string Status { get; set; } = "Active";

        // Navigation
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
