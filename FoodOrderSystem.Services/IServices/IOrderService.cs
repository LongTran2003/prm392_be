using FoodOrderSystem.Models.DTOs.Order;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface IOrderService
    {
        // POST /api/v1/Orders/create
        Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(string customerId, CreateOrderRequestDto createDto);

        // GET /api/v1/Orders/{customerId}
        Task<ApiResponseDto<List<OrderResponseDto>>> GetCustomerOrdersAsync(string customerId);

        // GET /api/v1/Orders/shop/{shopId}
        Task<ApiResponseDto<List<OrderResponseDto>>> GetShopOrdersAsync(Guid shopId, string shopOwnerId);

        // PUT /api/v1/Orders/{orderId}/status
        Task<ApiResponseDto<string>> UpdateOrderStatusAsync(Guid orderId, string shopOwnerId, UpdateOrderStatusDto updateDto);

        // DELETE /api/v1/Orders/{orderId}
        Task<ApiResponseDto<string>> CancelOrderAsync(Guid orderId, string customerId);

        // Internal: Sync order to Firebase
        Task SyncOrderToFirebaseAsync(Guid orderId);
    }
}
