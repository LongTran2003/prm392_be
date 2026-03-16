using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetAllActiveCategoriesAsync();
        void Update (Category category);
    }
}
