using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        //===========================================================================
        // Define properties for your repositories here
        //===========================================================================

        IStudentRepository Student { get; }



        //===========================================================================
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
