namespace FoodOrderSystem.Models.DTOs.Authentication.GoogleLogin
{
    public class GoogleLoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserRole { get; set; } = "Customer";
    }
}
