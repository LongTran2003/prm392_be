using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDBContext dbContext, UserManager<ApplicationUser> userManager)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all users with Student role
        /// Used when ShopOwner needs to view customer list for orders
        /// </summary>
        public async Task<List<ApplicationUser>> GetAllStudentsAsync()
        {
            // Get all users, then filter by role (to avoid EF Core issues with many-to-many)
            var allUsers = await _dbContext.Users.ToListAsync();
            var studentsList = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Student"))
                {
                    studentsList.Add(user);
                }
            }

            return studentsList;
        }

        /// <summary>
        /// Get user with all role information loaded
        /// </summary>
        public async Task<ApplicationUser?> GetUserWithRolesAsync(string userId)
        {
            return await _dbContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }
    }
}
