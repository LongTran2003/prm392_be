using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.DTOs.Admin;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodOrderSystem.Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Models.Domains.ApplicationUser> _userManager;

        public AdminService(
            IUnitOfWork unitOfWork,
            UserManager<Models.Domains.ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        /// <summary>
        /// GET /api/v1/Admin/dashboard
        /// Complete admin dashboard with statistics, pending shops, recent orders
        /// </summary>
        public async Task<ApiResponseDto<AdminDashboardDto>> GetDashboardAsync()
        {
            try
            {
                var dashboard = new AdminDashboardDto();

                // Get statistics - convert to List first
                var allUsers = await _userManager.Users.ToListAsync();
                var allShops = (await _unitOfWork.Shop.GetAllAsync()).ToList();
                var allOrders = (await _unitOfWork.Order.GetAllAsync()).ToList();

                var approvedShops = allShops.Where(s => s.Status == "Approved").ToList();
                var pendingShops = allShops.Where(s => s.Status == "Pending").ToList();
                var completedOrders = allOrders.Where(o => o.OrderStatus == "Completed").ToList();

                // Calculate new users this month
                var thisMonth = DateTime.UtcNow.Month;
                var newUsersThisMonth = allUsers.Count(u => u.CreatedTime.HasValue && u.CreatedTime.Value.Month == thisMonth);
                var newShopsThisMonth = allShops.Count(s => s.CreatedDate.Month == thisMonth);

                dashboard.Statistics = new AdminDashboardDto.DashboardStatistics
                {
                    TotalUsers = allUsers.Count,
                    TotalShops = allShops.Count,
                    ApprovedShops = approvedShops.Count,
                    PendingShops = pendingShops.Count,
                    TotalOrders = allOrders.Count,
                    CompletedOrders = completedOrders.Count,
                    TotalRevenue = completedOrders.DefaultIfEmpty().Sum(o => o?.TotalAmount ?? 0),
                    NewUsersThisMonth = newUsersThisMonth,
                    NewShopsThisMonth = newShopsThisMonth
                };

                // Get pending shops with owner info
                foreach (var shop in pendingShops.Take(10))
                {
                    var owner = await _userManager.FindByIdAsync(shop.ShopOwnerId.ToString());
                    dashboard.PendingShops.Add(new PendingShopDto
                    {
                        ShopId = shop.ShopId,
                        ShopName = shop.ShopName,
                        Address = shop.Address,
                        OwnerName = owner?.FullName ?? "Unknown",
                        OwnerEmail = owner?.Email ?? "Unknown",
                        OwnerPhone = owner?.PhoneNumber ?? "Unknown",
                        CreatedDate = shop.CreatedDate,
                        ImageUrl = shop.ImageUrl,
                        BusinessLicenseImageUrl = shop.BusinessLicenseImageUrl
                    });
                }

                // Get recent orders
                var recentOrdersList = allOrders
                    .OrderByDescending(o => o.CreatedDate)
                    .Take(10)
                    .ToList();

                foreach (var order in recentOrdersList)
                {
                    var shop = await _unitOfWork.Shop.GetShopByIdAsync(order.ShopId);
                    var customer = await _userManager.FindByIdAsync(order.CustomerId);

                    dashboard.RecentOrders.Add(new RecentOrderDto
                    {
                        OrderId = order.OrderId,
                        ShopName = shop?.ShopName ?? "Unknown",
                        CustomerName = customer?.FullName ?? "Unknown",
                        TotalAmount = order.TotalAmount,
                        OrderStatus = order.OrderStatus,
                        PaymentStatus = order.PaymentStatus,
                        CreatedDate = order.CreatedDate
                    });
                }

                // Get revenue statistics
                dashboard.RevenueStats = await GetRevenueStatisticsInternalAsync();

                return new ApiResponseDto<AdminDashboardDto>
                {
                    Success = true,
                    Message = "Dashboard retrieved successfully",
                    Data = dashboard
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<AdminDashboardDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve dashboard: {ex.Message}",
                    MessageId = "DASHBOARD_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Admin/shops/pending
        /// Get pending and rejected shops for approval workflow
        /// </summary>
        public async Task<ApiResponseDto<ShopsManagementDto>> GetShopsManagementAsync()
        {
            try
            {
                var allShopsEnum = await _unitOfWork.Shop.GetAllAsync();
                var allShops = allShopsEnum.ToList();
                var result = new ShopsManagementDto();

                var pendingShops = allShops.Where(s => s.Status == "Pending").ToList();
                var rejectedShops = allShops.Where(s => s.Status == "Rejected").ToList();
                var approvedShops = allShops.Where(s => s.Status == "Approved").ToList();

                // Pending shops with owner info
                foreach (var shop in pendingShops)
                {
                    var owner = await _userManager.FindByIdAsync(shop.ShopOwnerId.ToString());
                    result.PendingShops.Add(new PendingShopDto
                    {
                        ShopId = shop.ShopId,
                        ShopName = shop.ShopName,
                        Address = shop.Address,
                        OwnerName = owner?.FullName ?? "Unknown",
                        OwnerEmail = owner?.Email ?? "Unknown",
                        OwnerPhone = owner?.PhoneNumber ?? "Unknown",
                        CreatedDate = shop.CreatedDate,
                        ImageUrl = shop.ImageUrl,
                        BusinessLicenseImageUrl = shop.BusinessLicenseImageUrl
                    });
                }

                // Rejected shops
                foreach (var shop in rejectedShops)
                {
                    var owner = await _userManager.FindByIdAsync(shop.ShopOwnerId.ToString());
                    result.RejectedShops.Add(new RejectedShopDto
                    {
                        ShopId = shop.ShopId,
                        ShopName = shop.ShopName,
                        OwnerName = owner?.FullName ?? "Unknown",
                        RejectedDate = shop.UpdatedDate ?? DateTime.UtcNow
                    });
                }

                result.TotalApprovedShops = approvedShops.Count;

                return new ApiResponseDto<ShopsManagementDto>
                {
                    Success = true,
                    Message = "Shops management data retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ShopsManagementDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve shops: {ex.Message}",
                    MessageId = "SHOPS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Admin/users
        /// Manage all users in system
        /// </summary>
        public async Task<ApiResponseDto<UsersManagementDto>> GetUsersManagementAsync()
        {
            try
            {
                var result = new UsersManagementDto();
                var allUsers = _userManager.Users.ToList();

                // Get user info
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "Student";
                    var orderCount = 0;
                    var walletBalance = 0m;

                    // Get order count
                    if (role == "Student")
                    {
                        var ordersEnum = await _unitOfWork.Order.GetOrdersByCustomerAsync(user.Id);
                        orderCount = ordersEnum.Count();
                    }
                    else if (role == "ShopOwner")
                    {
                        var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == user.Id);
                        if (shopOwner != null)
                        {
                            walletBalance = shopOwner.WalletBalance;
                            var shopsEnum = await _unitOfWork.Shop.GetShopsByOwnerAsync(shopOwner.ShopOwnerId);
                            var shops = shopsEnum.ToList();
                            foreach (var shop in shops)
                            {
                                var ordersEnum = await _unitOfWork.Order.GetOrdersByShopAsync(shop.ShopId);
                                orderCount += ordersEnum.Count();
                            }
                        }
                    }

                    var createdTime = user.CreatedTime.HasValue ? user.CreatedTime.Value : DateTime.UtcNow;

                    result.Users.Add(new UsersManagementDto.UserInfoDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName ?? "Unknown",
                        Email = user.Email ?? "Unknown",
                        FullName = user.FullName,
                        Phone = user.PhoneNumber ?? "N/A",
                        Role = role,
                        CreatedDate = createdTime,
                        OrderCount = orderCount,
                        WalletBalance = walletBalance,
                        IsActive = user.Status == "Active"
                    });
                }

                // Statistics
                var studentCount = 0;
                var shopOwnerCount = 0;
                var adminCount = 0;
                var thisMonth = DateTime.UtcNow.Month;

                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Student")) studentCount++;
                    if (roles.Contains("ShopOwner")) shopOwnerCount++;
                    if (roles.Contains("Admin")) adminCount++;
                }

                var newUsersThisMonth = allUsers.Count(u =>
                    u.CreatedTime.HasValue && u.CreatedTime.Value.Month == thisMonth);

                result.Statistics = new UsersManagementDto.UserStatisticsDto
                {
                    TotalUsers = allUsers.Count,
                    StudentCount = studentCount,
                    ShopOwnerCount = shopOwnerCount,
                    NewUsersThisMonth = newUsersThisMonth
                };

                return new ApiResponseDto<UsersManagementDto>
                {
                    Success = true,
                    Message = "Users data retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<UsersManagementDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve users: {ex.Message}",
                    MessageId = "USERS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Admin/orders/statistics
        /// Order statistics and breakdown
        /// </summary>
        public async Task<ApiResponseDto<OrderStatisticsDto>> GetOrderStatisticsAsync()
        {
            try
            {
                var allOrdersEnum = await _unitOfWork.Order.GetAllAsync();
                var allOrders = allOrdersEnum.ToList();
                var ordersToday = allOrders.Where(o => o.CreatedDate.Date == DateTime.UtcNow.Date).ToList();
                var ordersThisWeek = allOrders.Where(o =>
                    o.CreatedDate >= DateTime.UtcNow.AddDays(-7)).ToList();
                var ordersThisMonth = allOrders.Where(o =>
                    o.CreatedDate.Month == DateTime.UtcNow.Month).ToList();

                var stats = new OrderStatisticsDto
                {
                    TotalOrders = allOrders.Count,
                    PendingOrders = allOrders.Count(o => o.OrderStatus == "Pending"),
                    ConfirmedOrders = allOrders.Count(o => o.OrderStatus == "Confirmed"),
                    DeliveringOrders = allOrders.Count(o => o.OrderStatus == "Delivering"),
                    CompletedOrders = allOrders.Count(o => o.OrderStatus == "Completed"),
                    CancelledOrders = allOrders.Count(o => o.OrderStatus == "Cancelled"),
                    TotalOrderValue = allOrders.Sum(o => o.TotalAmount),
                    AverageOrderValue = allOrders.Count > 0 ? allOrders.Average(o => o.TotalAmount) : 0,
                    TodayOrders = ordersToday.Count,
                    ThisWeekOrders = ordersThisWeek.Count,
                    ThisMonthOrders = ordersThisMonth.Count
                };

                return new ApiResponseDto<OrderStatisticsDto>
                {
                    Success = true,
                    Message = "Order statistics retrieved successfully",
                    Data = stats
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderStatisticsDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve statistics: {ex.Message}",
                    MessageId = "STATISTICS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Admin/revenue
        /// Revenue statistics with daily breakdown
        /// </summary>
        public async Task<ApiResponseDto<RevenueStatisticsDto>> GetRevenueStatisticsAsync()
        {
            try
            {
                var result = await GetRevenueStatisticsInternalAsync();
                return new ApiResponseDto<RevenueStatisticsDto>
                {
                    Success = true,
                    Message = "Revenue statistics retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<RevenueStatisticsDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve revenue: {ex.Message}",
                    MessageId = "REVENUE_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Admin/statistics/summary
        /// System summary with breakdowns
        /// </summary>
        public async Task<ApiResponseDto<SystemSummaryDto>> GetSystemSummaryAsync()
        {
            try
            {
                var allUsers = _userManager.Users.ToList();
                var allShopsEnum = await _unitOfWork.Shop.GetAllAsync();
                var allShops = allShopsEnum.ToList();
                var allOrdersEnum = await _unitOfWork.Order.GetAllAsync();
                var allOrders = allOrdersEnum.ToList();
                var completedOrders = allOrders.Where(o => o.OrderStatus == "Completed").ToList();

                var summary = new SystemSummaryDto
                {
                    TotalUsers = allUsers.Count,
                    TotalShops = allShops.Count,
                    TotalOrders = allOrders.Count,
                    TotalRevenue = completedOrders.DefaultIfEmpty().Sum(o => o?.TotalAmount ?? 0),
                    PendingShopsCount = allShops.Count(s => s.Status == "Pending")
                };

                // Order status breakdown
                summary.OrderStatusBreakdown = new Dictionary<string, int>
                {
                    { "Pending", allOrders.Count(o => o.OrderStatus == "Pending") },
                    { "Confirmed", allOrders.Count(o => o.OrderStatus == "Confirmed") },
                    { "Delivering", allOrders.Count(o => o.OrderStatus == "Delivering") },
                    { "Completed", allOrders.Count(o => o.OrderStatus == "Completed") },
                    { "Cancelled", allOrders.Count(o => o.OrderStatus == "Cancelled") }
                };

                // User role breakdown
                var studentCount = 0;
                var shopOwnerCount = 0;
                var adminCount = 0;

                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Student")) studentCount++;
                    if (roles.Contains("ShopOwner")) shopOwnerCount++;
                    if (roles.Contains("Admin")) adminCount++;
                }

                summary.UserRoleBreakdown = new Dictionary<string, int>
                {
                    { "Student", studentCount },
                    { "ShopOwner", shopOwnerCount },
                    { "Admin", adminCount }
                };

                return new ApiResponseDto<SystemSummaryDto>
                {
                    Success = true,
                    Message = "System summary retrieved successfully",
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<SystemSummaryDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve summary: {ex.Message}",
                    MessageId = "SUMMARY_FETCH_FAILED"
                };
            }
        }

        // Helper method
        private async Task<RevenueStatisticsDto> GetRevenueStatisticsInternalAsync()
        {
            var allOrdersEnum = await _unitOfWork.Order.GetAllAsync();
            var allOrders = allOrdersEnum.ToList();
            var completedOrders = allOrders.Where(o => o.OrderStatus == "Completed").ToList();

            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddMonths(-1);

            var todayRevenue = completedOrders
                .Where(o => o.CompletedDate?.Date == today)
                .Sum(o => o.TotalAmount);

            var weekRevenue = completedOrders
                .Where(o => o.CompletedDate >= weekAgo)
                .Sum(o => o.TotalAmount);

            var monthRevenue = completedOrders
                .Where(o => o.CompletedDate?.Month == today.Month)
                .Sum(o => o.TotalAmount);

            // Daily breakdown for last 30 days
            var dailyRevenue = new List<RevenueStatisticsDto.DailyRevenueDto>();
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var dayRevenue = completedOrders
                    .Where(o => o.CompletedDate?.Date == date)
                    .Sum(o => o.TotalAmount);

                dailyRevenue.Add(new RevenueStatisticsDto.DailyRevenueDto
                {
                    Date = date,
                    Amount = dayRevenue
                });
            }

            return new RevenueStatisticsDto
            {
                TodayRevenue = todayRevenue,
                ThisWeekRevenue = weekRevenue,
                ThisMonthRevenue = monthRevenue,
                TotalRevenue = completedOrders.DefaultIfEmpty().Sum(o => o?.TotalAmount ?? 0),
                DailyRevenue = dailyRevenue
            };
        }
    }
}
