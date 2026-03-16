using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        //===========================================================================
        // Define properties for your repositories here
        //===========================================================================

        IAuthRepository Auth { get; }
        IStudentRepository Student { get; }
        IShopOwnerRepository ShopOwner { get; }


        //===========================================================================
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
