using FoodOrderSystem.Models.DTOs.Transaction;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// GET /api/v1/transactions/{userId}
        /// Get transaction history (wallet transactions)
        /// Called from: WalletFragment.java:69
        /// ShopOwner can only view their own transactions
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTransactions(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Users can only view their own, Admins can view any
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin") && currentUserId != userId)
            {
                return Forbid();
            }

            var result = await _transactionService.GetTransactionsAsync(userId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// POST /api/v1/transactions/withdraw
        /// Submit withdrawal request from wallet
        /// Called from: WalletFragment.java:122
        /// ShopOwner only
        /// Request: { amount, description }
        /// </summary>
        [HttpPost("withdraw")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> Withdraw([FromBody] CreateWithdrawalDto withdrawDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _transactionService.WithdrawAsync(userId, withdrawDto);
            return StatusCode(result.Success ? 200 : 400, result);
        }
    }
}
