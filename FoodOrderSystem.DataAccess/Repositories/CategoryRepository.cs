using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoryRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all active categories (sorted by name)
        /// Used for listing in HomeFragment
        /// </summary>
        public async Task<List<Category>> GetAllActiveCategoriesAsync()
        {
            return await _dbContext.Categories
                .Where(c => c.Status == "Active")
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public void Update (Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
