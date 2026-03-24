using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Payment;
using FoodOrderSystem.Services.IServices;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;


namespace FoodOrderSystem.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly PayOS _payOS;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            HttpClient httpClient,
            PayOS payOS)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
            _payOS = payOS;
        }

        /// <summary>
        /// POST /api/v1/Payments/create-payment
        /// Generate PayOS QR code for bank transfer
        /// Android: CartFragment.java:158 → OrderViewModel.generateQrCode()
        /// Returns RAW response: { checkoutUrl }
        /// </summary>
        public async Task<CreatePaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto requestDto)
        {
            try
            {
                // Create Order record in database
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    FirebaseOrderId = Guid.NewGuid().ToString(),  // Reference to Firebase
                    CustomerId = requestDto.CustomerId,
                    ShopId = requestDto.ShopId,
                    PaymentMethod = requestDto.PaymentMethod,
                    TotalAmount = requestDto.TotalAmount,
                    OrderStatus = requestDto.OrderStatus,
                    PaymentStatus = "Pending",
                    CreatedDate = DateTime.UtcNow
                };

                // Add order items
                foreach (var item in requestDto.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        SubTotal = item.Price * item.Quantity
                    };

                    // Get menu item name
                    var menuItem = await _unitOfWork.MenuItem.GetAsync(m => m.MenuItemId == item.MenuItemId);
                    if (menuItem != null)
                    {
                        orderItem.MenuItemName = menuItem.MenuItemName;
                    }

                    order.OrderItems.Add(orderItem);
                }

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveAsync();

                // Tạo OrderCode bằng số (PayOS yêu cầu mã ID kiểu long tối đa 53 bit)
                long payosOrderCode = long.Parse(DateTimeOffset.Now.ToString("yyMMddHHmmss"));

                // Gom dữ liệu để thanh toán (Đại diện cho số tiền)
                ItemData payosItem = new ItemData("Food Order", 1, (int)requestDto.TotalAmount);
                List<ItemData> items = new List<ItemData> { payosItem };

                // Tạo đối tượng Request cho PayOS
                PaymentData paymentData = new PaymentData(
                    orderCode: payosOrderCode,
                    amount: (int)requestDto.TotalAmount,
                    description: "Order " + payosOrderCode,
                    items: items,
                    cancelUrl: "https://studentorderfood.app/cancel",
                    returnUrl: "https://studentorderfood.app/return"
                );

                // Call API của PayOS để lấy Link Thanh Toán
                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                var checkoutUrl = createPayment.checkoutUrl;

                // Lưu lại mã hóa đơn để sau này check callback
                order.PayosOrderCode = payosOrderCode.ToString();

                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();

                return new CreatePaymentResponseDto
                {
                    CheckoutUrl = checkoutUrl
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Payment creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// POST /api/v1/Payments/payment-result
        /// Handle payment callback from PayOS
        /// Android: PaymentFragment.java:94, 101
        /// Returns RAW response: { message, orderCode }
        /// </summary>
        public async Task<PaymentResultResponseDto> HandlePaymentResultAsync(PaymentResultRequestDto requestDto)
        {
            try
            {
                // Find order by Payos order code
                var order = await _unitOfWork.Order.GetOrderByPayosCodeAsync(requestDto.OrderCode);
                if (order == null)
                {
                    return new PaymentResultResponseDto
                    {
                        Message = "Order not found",
                        OrderCode = requestDto.OrderCode
                    };
                }

                // Update payment status based on callback status
                if (requestDto.Status == "PAID")
                {
                    order.PaymentStatus = "Paid";
                    order.PaidDate = DateTime.UtcNow;
                    order.OrderStatus = "Confirmed";  // Auto-confirm when paid
                }
                else if (requestDto.Status == "FAILED" || requestDto.Status == "CANCELLED")
                {
                    order.PaymentStatus = "Failed";
                    order.OrderStatus = "Cancelled";
                }

                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();

                return new PaymentResultResponseDto
                {
                    Message = $"Payment {requestDto.Status.ToLower()} successfully",
                    OrderCode = requestDto.OrderCode
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Payment result handling failed: {ex.Message}", ex);
            }
        }
    }
}
