using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Rating
    {
        [Key]
        public Guid RatingId { get; set; }

        [Required]
        public Guid ShopId { get; set; }

        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; } = null!;

        [Required]
        public string CustomerId { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int StarRating { get; set; }  // 1-5 stars

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public string Status { get; set; } = StaticOperationStatus.BaseEntity.Active;

        // Navigation
        public virtual ICollection<RatingImage>? Images { get; set; }
    }
}
