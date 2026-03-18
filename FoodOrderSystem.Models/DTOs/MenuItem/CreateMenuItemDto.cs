using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.MenuItem
{
    /// <summary>
    /// Create menu item with multipart form
    /// Android: ShopFormFragment.java - POST /api/v1/MenuItem (Multipart)
    /// Files: image
    /// </summary>
    public class CreateMenuItemDto
    {
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

        public IFormFile? Image { get; set; }

        // image file sent as multipart
    }
}
