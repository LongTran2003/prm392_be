using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IShopRepository : IRepository<Shop>
    {
        // Get all shops by owner
        Task<List<Shop>> GetShopsByOwnerAsync(Guid shopOwnerId);

        // Get shops by status with pagination
        Task<(List<Shop> shops, int total)> GetShopsByStatusAsync(string status, int pageIndex, int pageSize);

        // Get popular shops (with time check if needed)
        Task<List<Shop>> GetPopularShopsAsync();

        // Get shop with all menu items and categories
        Task<Shop?> GetShopWithMenuAsync(Guid shopId);

        // Get shop by ID
        Task<Shop?> GetShopByIdAsync(Guid shopId);
    }
}
