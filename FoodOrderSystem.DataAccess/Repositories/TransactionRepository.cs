using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public TransactionRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Transaction>> GetTransactionsByUserAsync(string userId)
        {
            return await _dbContext.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByUserWithFilterAsync(
            string userId, string? transactionType = null, int pageIndex = 1, int pageSize = 20)
        {
            var query = _dbContext.Transactions
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(t => t.TransactionType == transactionType);
            }

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalWithdrawnAsync(string userId)
        {
            return await _dbContext.Transactions
                .Where(t => t.UserId == userId && t.TransactionType == "Withdrawal" && t.Status == "Completed")
                .SumAsync(t => t.Amount);
        }
    }
}
