using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Models.DTOs.Transaction;
using FoodOrderSystem.Services.IServices;

namespace FoodOrderSystem.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// GET /api/v1/transactions/{userId}
        /// Get all transactions for user (ShopOwner wallet history)
        /// Called from: WalletFragment.java:69
        /// </summary>
        public async Task<ApiResponseDto<List<TransactionResponseDto>>> GetTransactionsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ApiResponseDto<List<TransactionResponseDto>>
                    {
                        Success = false,
                        Message = "User ID is required",
                        MessageId = "INVALID_USER_ID"
                    };
                }

                // Verify user exists
                var user = await _unitOfWork.User.GetUserWithRolesAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<List<TransactionResponseDto>>
                    {
                        Success = false,
                        Message = "User not found",
                        MessageId = "USER_NOT_FOUND"
                    };
                }

                var transactions = await _unitOfWork.Transaction.GetTransactionsByUserAsync(userId);

                var transactionDtos = transactions
                    .Select(t => new TransactionResponseDto
                    {
                        TransactionId = t.TransactionId,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        OrderCode = t.OrderCode,
                        Description = t.Description,
                        Status = t.Status,
                        CreatedDate = t.CreatedDate,
                        CompletedDate = t.CompletedDate
                    })
                    .ToList();

                return new ApiResponseDto<List<TransactionResponseDto>>
                {
                    Success = true,
                    Message = "Transactions retrieved successfully",
                    Data = transactionDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<TransactionResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve transactions: {ex.Message}",
                    MessageId = "TRANSACTIONS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// POST /api/v1/transactions/withdraw
        /// Withdraw money from wallet
        /// Called from: WalletFragment.java:122 → submit withdrawal request
        /// Only ShopOwner can withdraw
        /// </summary>
        public async Task<ApiResponseDto<TransactionResponseDto>> WithdrawAsync(
            string userId, CreateWithdrawalDto withdrawDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ApiResponseDto<TransactionResponseDto>
                    {
                        Success = false,
                        Message = "User ID is required",
                        MessageId = "INVALID_USER_ID"
                    };
                }

                // Verify user is ShopOwner (has wallet)
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == userId);
                if (shopOwner == null)
                {
                    return new ApiResponseDto<TransactionResponseDto>
                    {
                        Success = false,
                        Message = "Only ShopOwner can withdraw",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Check sufficient balance
                if (shopOwner.WalletBalance < withdrawDto.Amount)
                {
                    return new ApiResponseDto<TransactionResponseDto>
                    {
                        Success = false,
                        Message = $"Insufficient balance. Available: {shopOwner.WalletBalance:C}, Requested: {withdrawDto.Amount:C}",
                        MessageId = "INSUFFICIENT_BALANCE"
                    };
                }

                // Create withdrawal transaction
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    UserId = userId,
                    TransactionType = "Withdrawal",
                    Amount = withdrawDto.Amount,
                    Description = withdrawDto.Description,
                    Status = "Pending",  // Admin must approve
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Transaction.AddAsync(transaction);

                // Deduct from wallet (will be credited back if rejected)
                shopOwner.WalletBalance -= withdrawDto.Amount;
                _unitOfWork.ShopOwner.Update(shopOwner);

                await _unitOfWork.SaveAsync();

                var response = new TransactionResponseDto
                {
                    TransactionId = transaction.TransactionId,
                    TransactionType = transaction.TransactionType,
                    Amount = transaction.Amount,
                    Description = transaction.Description,
                    Status = transaction.Status,
                    CreatedDate = transaction.CreatedDate
                };

                return new ApiResponseDto<TransactionResponseDto>
                {
                    Success = true,
                    Message = "Withdrawal request submitted. Pending admin approval.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TransactionResponseDto>
                {
                    Success = false,
                    Message = $"Withdrawal failed: {ex.Message}",
                    MessageId = "WITHDRAWAL_FAILED"
                };
            }
        }

        /// <summary>
        /// Internal method: Record payment transaction when order is paid
        /// Called from: PaymentService after successful payment
        /// </summary>
        public async Task RecordPaymentTransactionAsync(
            string shopOwnerId, decimal amount, string? orderCode, string description)
        {
            try
            {
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    UserId = shopOwnerId,
                    TransactionType = "Payment",
                    Amount = amount,
                    OrderCode = orderCode,
                    Description = description,
                    Status = "Completed",
                    CreatedDate = DateTime.UtcNow,
                    CompletedDate = DateTime.UtcNow
                };

                await _unitOfWork.Transaction.AddAsync(transaction);

                // Add to wallet
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == shopOwnerId);
                if (shopOwner != null)
                {
                    shopOwner.WalletBalance += amount;
                    _unitOfWork.ShopOwner.Update(shopOwner);
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - payment already succeeded
                // This is just tracking
                Console.WriteLine($"Error recording payment transaction: {ex.Message}");
            }
        }

        /// <summary>
        /// Internal method: Record refund transaction
        /// Called when order is cancelled/refunded
        /// </summary>
        public async Task RecordRefundTransactionAsync(
            string userId, decimal amount, string? orderCode, string description)
        {
            try
            {
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    UserId = userId,
                    TransactionType = "Refund",
                    Amount = amount,
                    OrderCode = orderCode,
                    Description = description,
                    Status = "Completed",
                    CreatedDate = DateTime.UtcNow,
                    CompletedDate = DateTime.UtcNow
                };

                await _unitOfWork.Transaction.AddAsync(transaction);

                // Add back to wallet
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == userId);
                if (shopOwner != null)
                {
                    shopOwner.WalletBalance += amount;
                    _unitOfWork.ShopOwner.Update(shopOwner);
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recording refund transaction: {ex.Message}");
            }
        }
    }
}
