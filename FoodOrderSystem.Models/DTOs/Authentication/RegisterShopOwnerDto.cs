using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Authentication
{
    public class RegisterShopOwnerDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(50)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string Phone { get; set; } = null!;

        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(200)]
        public string? Avatar { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
