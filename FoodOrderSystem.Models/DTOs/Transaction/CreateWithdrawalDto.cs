using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Transaction
{
    /// <summary>
    /// Withdrawal request from wallet
    /// Android: WalletFragment.java:122 - POST /api/v1/transactions/withdraw
    /// ShopOwner requests to withdraw money from wallet to bank account
    /// </summary>
    public class CreateWithdrawalDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;
    }
}
