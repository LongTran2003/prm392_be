using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Models.DTOs.Transaction;

namespace FoodOrderSystem.Services.IServices
{
    public interface ITransactionService
    {
        // GET /api/v1/transactions/{userId}
        Task<ApiResponseDto<List<TransactionResponseDto>>> GetTransactionsAsync(string userId);

        // POST /api/v1/transactions/withdraw
        Task<ApiResponseDto<TransactionResponseDto>> WithdrawAsync(string userId, CreateWithdrawalDto withdrawDto);

        // Internal: Record payment transaction when order is paid
        Task RecordPaymentTransactionAsync(string shopOwnerId, decimal amount, string? orderCode, string description);

        // Internal: Refund transaction
        Task RecordRefundTransactionAsync(string userId, decimal amount, string? orderCode, string description);
    }
}
