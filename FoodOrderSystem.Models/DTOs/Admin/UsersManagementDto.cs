namespace FoodOrderSystem.Models.DTOs.Admin
{
    /// <summary>
    /// User management for admins
    /// GET /api/v1/Admin/users
    /// </summary>
    public class UsersManagementDto
    {
        public List<UserInfoDto> Users { get; set; } = new();
        public UserStatisticsDto Statistics { get; set; } = new();

        public class UserInfoDto
        {
            public string UserId { get; set; } = null!;
            public string UserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public string Role { get; set; } = null!;
            public DateTime CreatedDate { get; set; }
            public int OrderCount { get; set; }
            public decimal WalletBalance { get; set; }
            public bool IsActive { get; set; }
        }

        public class UserStatisticsDto
        {
            public int TotalUsers { get; set; }
            public int StudentCount { get; set; }
            public int ShopOwnerCount { get; set; }
            public int NewUsersThisMonth { get; set; }
        }
    }
}
