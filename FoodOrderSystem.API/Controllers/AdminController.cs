using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// GET /api/v1/Admin/dashboard
        /// Complete admin dashboard with stats, pending shops, recent orders
        /// Admin only
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/Admin/shops/pending
        /// Manage shop approvals
        /// Admin only
        /// </summary>
        [HttpGet("shops/pending")]
        public async Task<IActionResult> GetShopsManagement()
        {
            var result = await _adminService.GetShopsManagementAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/Admin/users
        /// Manage all users
        /// Admin only
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersManagement()
        {
            var result = await _adminService.GetUsersManagementAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/Admin/orders/statistics
        /// Order statistics and breakdown
        /// Admin only
        /// </summary>
        [HttpGet("orders/statistics")]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var result = await _adminService.GetOrderStatisticsAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/Admin/revenue
        /// Revenue statistics with daily breakdown
        /// Admin only
        /// </summary>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var result = await _adminService.GetRevenueStatisticsAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/Admin/statistics/summary
        /// System summary with breakdowns
        /// Admin only
        /// </summary>
        [HttpGet("statistics/summary")]
        public async Task<IActionResult> GetSystemSummary()
        {
            var result = await _adminService.GetSystemSummaryAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }
    }
}
