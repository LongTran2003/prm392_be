using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public RatingRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Rating>> GetRatingsByShopAsync(Guid shopId)
        {
            return await _dbContext.Ratings
                .Where(r => r.ShopId == shopId && r.Status == "Active")
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid shopId)
        {
            var ratings = await _dbContext.Ratings
                .Where(r => r.ShopId == shopId && r.Status == "Active")
                .ToListAsync();

            return ratings.Count > 0 ? ratings.Average(r => r.StarRating) : 0;
        }

        public async Task<Dictionary<int, int>> GetRatingBreakdownAsync(Guid shopId)
        {
            var ratings = await _dbContext.Ratings
                .Where(r => r.ShopId == shopId && r.Status == "Active")
                .ToListAsync();

            var breakdown = new Dictionary<int, int>
            {
                { 1, ratings.Count(r => r.StarRating == 1) },
                { 2, ratings.Count(r => r.StarRating == 2) },
                { 3, ratings.Count(r => r.StarRating == 3) },
                { 4, ratings.Count(r => r.StarRating == 4) },
                { 5, ratings.Count(r => r.StarRating == 5) }
            };

            return breakdown;
        }

        public async Task<Rating?> GetCustomerRatingAsync(Guid shopId, string customerId)
        {
            return await _dbContext.Ratings
                .Where(r => r.ShopId == shopId && r.CustomerId == customerId)
                .Include(r => r.Images)
                .FirstOrDefaultAsync();
        }
    }
}
