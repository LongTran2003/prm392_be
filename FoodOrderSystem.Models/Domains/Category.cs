using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.Domains
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public string Status { get; set; } = StaticOperationStatus.BaseEntity.Active;

        // Navigation properties
        public virtual ICollection<MenuItem>? MenuItems { get; set; }
    }
}
