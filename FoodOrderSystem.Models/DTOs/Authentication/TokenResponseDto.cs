namespace FoodOrderSystem.Models.DTOs.Authentication
{
    /// <summary>
    /// Response after successful login (Student or ShopOwner)
    /// Must contain: accessToken, refreshToken, userRole, userId
    /// </summary>
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string UserRole { get; set; } = null!; // "Student", "ShopOwner", or "Admin"
        public string UserId { get; set; } = null!;
    }
}
