using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Update shop - files are optional (preserve old if null)
    /// Android: ShopFormFragment.java:249 - PUT /api/v1/shops (Multipart)
    /// </summary>
    public class UpdateShopDto
    {
        [Required]
        public Guid ShopId { get; set; }

        [Required]
        [StringLength(200)]
        public string ShopName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(300)]
        public string Address { get; set; } = null!;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string OpenHours { get; set; } = null!;

        [Required]
        public string CloseHours { get; set; } = null!;

        public IFormFile? Image { get; set; }
        public IFormFile? BusinessLicenseImage { get; set; }

        // Files are optional for updates (sent as multipart)
        // image, businessLicenseImage
    }
}
