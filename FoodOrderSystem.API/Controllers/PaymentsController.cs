using FoodOrderSystem.Models.DTOs.Payment;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// POST /api/v1/Payments/create-payment
        /// Generate PayOS QR code for bank transfer
        /// Request: { customerId, shopId, orderItems[], totalAmount, paymentMethod }
        /// Response: RETURNS RAW { checkoutUrl } - NOT wrapped in ApiResponseDto!
        /// Called from: CartFragment.java:158 → OrderViewModel.generateQrCode()
        /// </summary>
        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto requestDto)
        {
            try
            {
                var result = await _paymentService.CreatePaymentAsync(requestDto);
                // Return RAW response, not wrapped!
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/v1/Payments/payment-result
        /// Handle payment callback from PayOS gateway
        /// Request: { orderCode, status }
        /// Response: RETURNS RAW { message, orderCode } - NOT wrapped in ApiResponseDto!
        /// Called from: PaymentFragment.java:94, 101 (WebView callback)
        /// </summary>
        [HttpPost("payment-result")]
        [AllowAnonymous]  // PayOS calls this as webhook
        public async Task<IActionResult> PaymentResult([FromBody] PaymentResultRequestDto requestDto)
        {
            try
            {
                var result = await _paymentService.HandlePaymentResultAsync(requestDto);
                // Return RAW response, not wrapped!
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
