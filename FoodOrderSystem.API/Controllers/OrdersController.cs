using FoodOrderSystem.Models.DTOs.Order;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static FoodOrderSystem.Utilities.Constants.StaticOperationStatus;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// POST /api/v1/Orders/create
        /// Create new order from cart
        /// Called from: CartFragment.java:150 (COD), :158 (Bank)
        /// Request: { shopId, orderItems[], paymentMethod }
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createDto)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var result = await _orderService.CreateOrderAsync(customerId, createDto);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// GET /api/v1/Orders/{customerId}
        /// Get customer's order history
        /// User can only view their own, Admin can view any
        /// </summary>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerOrders(string customerId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Check authorization
            if (!roles.Contains("Admin") && currentUserId != customerId)
            {
                return Forbid();
            }

            var result = await _orderService.GetCustomerOrdersAsync(customerId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// GET /api/v1/Orders/shop/{shopId}
        /// Get orders for shop (ShopOwner only)
        /// Called from: ShopOwner order management screen
        /// </summary>
        [HttpGet("shop/{shopId}")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> GetShopOrders(Guid shopId)
        {
            var shopOwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(shopOwnerId))
            {
                return Unauthorized();
            }

            var result = await _orderService.GetShopOrdersAsync(shopId, shopOwnerId);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// PUT /api/v1/Orders/{orderId}/status
        /// Update order status (ShopOwner only)
        /// Status: Confirmed, Delivering, Completed, Cancelled
        /// </summary>
        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusDto updateDto)
        {
            var shopOwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(shopOwnerId))
            {
                return Unauthorized();
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, shopOwnerId, updateDto);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// DELETE /api/v1/Orders/{orderId}
        /// Cancel order (customer only, before confirmed)
        /// </summary>
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var result = await _orderService.CancelOrderAsync(orderId, customerId);
            return StatusCode(result.Success ? 200 : 400, result);
        }
    }
}
