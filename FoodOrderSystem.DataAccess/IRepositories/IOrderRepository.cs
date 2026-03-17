using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetOrderByPayosCodeAsync(string payosOrderCode);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<List<Order>> GetOrdersByShopAsync(Guid shopId);
    }
}
