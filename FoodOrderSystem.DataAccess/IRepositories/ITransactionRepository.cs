using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<List<Transaction>> GetTransactionsByUserAsync(string userId);
        Task<List<Transaction>> GetTransactionsByUserWithFilterAsync(string userId, string? transactionType = null, int pageIndex = 1, int pageSize = 20);
        Task<decimal> GetTotalWithdrawnAsync(string userId);
    }
}
