using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<List<ApplicationUser>> GetAllStudentsAsync();
        Task<ApplicationUser?> GetUserWithRolesAsync(string userId);
    }
}
