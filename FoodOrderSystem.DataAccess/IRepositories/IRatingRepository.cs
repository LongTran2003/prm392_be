using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<List<Rating>> GetRatingsByShopAsync(Guid shopId);
        Task<double> GetAverageRatingAsync(Guid shopId);
        Task<Dictionary<int, int>> GetRatingBreakdownAsync(Guid shopId);
        Task<Rating?> GetCustomerRatingAsync(Guid shopId, string customerId);
    }
}
