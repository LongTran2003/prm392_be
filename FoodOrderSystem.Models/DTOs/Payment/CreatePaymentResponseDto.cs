namespace FoodOrderSystem.Models.DTOs.Payment
{
    /// <summary>
    /// Raw response from payment endpoint (NOT wrapped)
    /// Android expects: { checkoutUrl }
    /// Example: { "checkoutUrl": "https://qr.payos.vn/..." }
    /// </summary>
    public class CreatePaymentResponseDto
    {
        public string CheckoutUrl { get; set; } = null!;
    }
}
