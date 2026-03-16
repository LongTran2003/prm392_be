namespace FoodOrderSystem.Models.DTOs.Profile
{
    public class CustomerResponseDto
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? BirthDate { get; set; }
        public string? Gender { get; set; }
    }
}
