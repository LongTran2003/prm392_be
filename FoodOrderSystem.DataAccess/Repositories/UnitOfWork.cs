using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;

        //===========================================================================
        // Define private fields for your irepositories here
        //===========================================================================

        public IAuthRepository Auth { get; private set; }   
        public IStudentRepository Student { get; private set; }  
        public IShopOwnerRepository ShopOwner { get; private set; }
        public IUserRepository User { get; private set; }



        public UnitOfWork(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            //===========================================================================
            // Initialize your repositories here
            //===========================================================================
            Auth = new AuthRepository(_context);
            Student = new StudentRepository(_context);
            ShopOwner = new ShopOwnerRepository(_context);
            User = new UserRepository(_context, userManager);



        }



        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
