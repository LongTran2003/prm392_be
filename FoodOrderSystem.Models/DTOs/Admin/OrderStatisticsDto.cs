namespace FoodOrderSystem.Models.DTOs.Admin
{
    public class OrderStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int DeliveringOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalOrderValue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TodayOrders { get; set; }
        public int ThisWeekOrders { get; set; }
        public int ThisMonthOrders { get; set; }
    }

    public class SystemSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalShops { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingShopsCount { get; set; }
        public Dictionary<string, int> OrderStatusBreakdown { get; set; } = new();
        public Dictionary<string, int> UserRoleBreakdown { get; set; } = new();
    }
}
