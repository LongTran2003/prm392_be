using FoodOrderSystem.Models.DTOs.Payment;

namespace FoodOrderSystem.Services.IServices
{
    public interface IPaymentService
    {
        // POST /api/v1/Payments/create-payment - generates QR code checkout URL
        Task<CreatePaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto requestDto);

        // POST /api/v1/Payments/payment-result - records payment callback
        Task<PaymentResultResponseDto> HandlePaymentResultAsync(PaymentResultRequestDto requestDto);
    }
}
