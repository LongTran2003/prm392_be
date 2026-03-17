namespace FoodOrderSystem.Models.DTOs.Order
{
    /// <summary>
    /// Order response for listing and detail
    /// Android: Shows order history with status
    /// </summary>
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string FirebaseOrderId { get; set; } = null!;
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;  // Pending, Confirmed, Delivering, Completed, Cancelled
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;  // Pending, Paid, Failed
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public class OrderItemResponseDto
        {
            public Guid MenuItemId { get; set; }
            public string MenuItemName { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal SubTotal { get; set; }
        }
    }
}
