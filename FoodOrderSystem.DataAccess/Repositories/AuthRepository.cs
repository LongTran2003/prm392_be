using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public AuthRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DoesPhoneNumberExistAsync(string phoneNumber)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> DoesEmailExistAsync(string email)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.Email == email.ToLower());
        }

        public async Task<bool> DoesUserNameExistAsync(string userName)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}
