namespace FoodOrderSystem.Models.DTOs.Admin
{
    /// <summary>
    /// Admin dashboard statistics
    /// GET /api/v1/Admin/dashboard
    /// Shows overview of entire system
    /// </summary>
    public class AdminDashboardDto
    {
        public DashboardStatistics Statistics { get; set; } = new();
        public List<PendingShopDto> PendingShops { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
        public RevenueStatisticsDto RevenueStats { get; set; } = new();

        public class DashboardStatistics
        {
            public int TotalUsers { get; set; }
            public int TotalShops { get; set; }
            public int ApprovedShops { get; set; }
            public int PendingShops { get; set; }
            public int TotalOrders { get; set; }
            public int CompletedOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public int NewUsersThisMonth { get; set; }
            public int NewShopsThisMonth { get; set; }
        }
    }

    public class PendingShopDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
        public string OwnerPhone { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? BusinessLicenseImageUrl { get; set; }
    }

    public class RecentOrderDto
    {
        public Guid OrderId { get; set; }
        public string ShopName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }

    public class RevenueStatisticsDto
    {
        public decimal TodayRevenue { get; set; }
        public decimal ThisWeekRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();

        public class DailyRevenueDto
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
