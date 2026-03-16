using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class MenuItem
    {
        [Key]
        public Guid MenuItemId { get; set; }

        [Required]
        [StringLength(200)]
        public string MenuItemName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int QuantitySold { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        // Foreign Keys
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public Guid ShopId { get; set; }

        public string Status { get; set; } = StaticOperationStatus.BaseEntity.Active;
    }
}
