using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Shop
    {
        [Key]
        public Guid ShopId { get; set; }

        [Required]
        [StringLength(200)]
        public string ShopName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(300)]
        public string Address { get; set; } = null!;

        [StringLength(200)]
        public string? ImageUrl { get; set; }  // Shop main image

        [StringLength(200)]
        public string? BusinessLicenseImageUrl { get; set; }  // Business license

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public TimeSpan OpenHours { get; set; }  // 09:00:00

        public TimeSpan CloseHours { get; set; }  // 22:00:00

        [StringLength(50)]
        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

        // Foreign Key
        public Guid ShopOwnerId { get; set; }

        [ForeignKey("ShopOwnerId")]
        public virtual ShopOwner ShopOwner { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int TotalOrders { get; set; } = 0;

        public double AverageRating { get; set; } = 0;

        public int TotalRatings { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<MenuItem>? MenuItems { get; set; }

        public virtual ICollection<Rating>? Ratings { get; set; }
    }
}
