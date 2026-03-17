namespace FoodOrderSystem.Models.DTOs.Profile
{
    /// <summary>
    /// User profile response - returned when fetching user details
    /// Must include all fields that Android displays: userId, email, phone, address, avatar, walletBalance, roleName
    /// </summary>
    public class GetUserResponseDto
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string FullName { get; set; } = null!;
        public string? Avatar { get; set; }
        public decimal WalletBalance { get; set; } = 0;
        public string RoleName { get; set; } = "Student"; // "Student", "ShopOwner", or "Admin"
        public string? UserName { get; set; }
        public string? BirthDate { get; set; }
    }
}
