namespace FoodOrderSystem.Models.DTOs.Payment
{
    /// <summary>
    /// Raw response from payment result endpoint (NOT wrapped)
    /// Android expects: { message, orderCode }
    /// </summary>
    public class PaymentResultResponseDto
    {
        public string Message { get; set; } = null!;
        public string OrderCode { get; set; } = null!;
    }
}
