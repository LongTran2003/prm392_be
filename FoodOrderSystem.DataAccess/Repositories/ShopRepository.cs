using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class ShopRepository : Repository<Shop>, IShopRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ShopRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Shop>> GetShopsByOwnerAsync(Guid shopOwnerId)
        {
            return await _dbContext.Shops
                .Where(s => s.ShopOwnerId == shopOwnerId && s.Status == "Approved")
                .OrderBy(s => s.ShopName)
                .ToListAsync();
        }

        public async Task<(List<Shop> shops, int total)> GetShopsByStatusAsync(string status, int pageIndex, int pageSize)
        {
            var query = _dbContext.Shops
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.CreatedDate);

            int total = await query.CountAsync();

            var shops = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (shops, total);
        }

        public async Task<List<Shop>> GetPopularShopsAsync()
        {
            // Popular = Approved + sorted by TotalOrders descending
            return await _dbContext.Shops
                .Where(s => s.Status == "Approved")
                .OrderByDescending(s => s.TotalOrders)
                .ThenByDescending(s => s.AverageRating)
                .Take(20)
                .ToListAsync();
        }

        public async Task<Shop?> GetShopWithMenuAsync(Guid shopId)
        {
            return await _dbContext.Shops
                .Include(s => s.MenuItems)
                    .ThenInclude(m => m.Category)
                .Where(s => s.ShopId == shopId && s.Status == "Approved")
                .FirstOrDefaultAsync();
        }

        public async Task<Shop?> GetShopByIdAsync(Guid shopId)
        {
            return await _dbContext.Shops
                .Where(s => s.ShopId == shopId)
                .FirstOrDefaultAsync();
        }
    }
}
