using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public MenuItemRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MenuItem>> GetMenuItemsByShopAsync(Guid shopId)
        {
            return await _dbContext.MenuItems
                .Where(m => m.ShopId == shopId && m.Status == "Active")
                .Include(m => m.Category)
                .OrderBy(m => m.Category.CategoryName)
                .ThenBy(m => m.MenuItemName)
                .ToListAsync();
        }
    }
}
