using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.MenuItem
{
    /// <summary>
    /// Update menu item - image is optional
    /// Android: ShopFormFragment.java - PUT /api/v1/MenuItem/{id} (Multipart)
    /// </summary>
    public class UpdateMenuItemDto
    {
        [Required]
        public Guid MenuItemId { get; set; }

        [Required]
        [StringLength(200)]
        public string MenuItemName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid ShopId { get; set; }

        public bool IsAvailable { get; set; } = true;

        // image file sent as multipart (optional)
    }
}
