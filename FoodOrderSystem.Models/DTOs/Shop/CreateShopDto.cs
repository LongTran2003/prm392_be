using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Shop
{
    /// <summary>
    /// Create shop with multipart form data
    /// Android: ShopFormFragment.java:251 - POST /api/v1/shops (Multipart)
    /// Files: image, businessLicenseImage
    /// </summary>
    public class CreateShopDto
    {
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
        public string OpenHours { get; set; } = null!;  // Format: "09:00:00"

        [Required]
        public string CloseHours { get; set; } = null!;  // Format: "22:00:00"

        // Files will be sent separately by Android as multipart
        // image, businessLicenseImage
    }
}
