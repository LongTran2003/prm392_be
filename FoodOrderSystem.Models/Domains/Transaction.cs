using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = null!;  // Payment, Withdrawal, Refund, Commission

        [Required]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? OrderCode { get; set; }  // Reference to order if applicable

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Completed";  // Pending, Completed, Failed

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedDate { get; set; }

        public string EntityStatus { get; set; } = "Active";
    }
}
