namespace FoodOrderSystem.Models.DTOs.Transaction
{
    /// <summary>
    /// Transaction response for listing
    /// Android: WalletFragment.java:69 - GET /api/v1/transactions/{userId}
    /// Shows all transactions (payments, withdrawals, refunds)
    /// </summary>
    public class TransactionResponseDto
    {
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; } = null!;  // Payment, Withdrawal, Refund, Commission
        public decimal Amount { get; set; }
        public string? OrderCode { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
