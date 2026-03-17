using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class RatingImageRepository : Repository<RatingImage>, IRatingImageRepository
    {
        public RatingImageRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
        }
    }
}
