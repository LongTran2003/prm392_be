using FoodOrderSystem.Models.DTOs.Admin;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface IAdminService
    {
        // GET /api/v1/Admin/dashboard
        Task<ApiResponseDto<AdminDashboardDto>> GetDashboardAsync();

        // GET /api/v1/Admin/shops/pending
        Task<ApiResponseDto<ShopsManagementDto>> GetShopsManagementAsync();

        // GET /api/v1/Admin/users
        Task<ApiResponseDto<UsersManagementDto>> GetUsersManagementAsync();

        // GET /api/v1/Admin/orders/statistics
        Task<ApiResponseDto<OrderStatisticsDto>> GetOrderStatisticsAsync();

        // GET /api/v1/Admin/revenue
        Task<ApiResponseDto<RevenueStatisticsDto>> GetRevenueStatisticsAsync();

        // GET /api/v1/Admin/statistics/summary
        Task<ApiResponseDto<SystemSummaryDto>> GetSystemSummaryAsync();
    }
}
