using FoodOrderSystem.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.Domains
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string FullName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [StringLength(6)]
        public string? OtpCode { get; set; }

        public DateTime? OtpExpiry { get; set; }

        public string Status { get; set; } = "Active"; // Default status is Active

        public DateTime? CreatedTime { get; set; } 


        // ADD THESE RELATIONSHIPS
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ShopOwner? ShopOwner { get; set; }
    }
}
