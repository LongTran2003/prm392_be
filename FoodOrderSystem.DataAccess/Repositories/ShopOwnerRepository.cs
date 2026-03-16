using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class ShopOwnerRepository : Repository<ShopOwner>, IShopOwnerRepository
    {
        public ShopOwnerRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
        }
    }
}
