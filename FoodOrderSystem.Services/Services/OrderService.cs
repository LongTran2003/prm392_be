using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Order;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;

namespace FoodOrderSystem.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private readonly IFirebaseService _firebaseService;

        public OrderService(
            IUnitOfWork unitOfWork,
            ITransactionService transactionService,
            IFirebaseService firebaseService)
        {
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
            _firebaseService = firebaseService;
        }

        /// <summary>
        /// POST /api/v1/Orders/create
        /// Create new order from cart
        /// Called from: CartFragment.java:150 (COD), :158 (Bank)
        /// Creates order in DB + syncs to Firebase
        /// </summary>
        public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(
            string customerId, CreateOrderRequestDto createDto)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return new ApiResponseDto<OrderResponseDto>
                    {
                        Success = false,
                        Message = "Customer ID is required",
                        MessageId = "INVALID_CUSTOMER_ID"
                    };
                }

                // Verify shop exists
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(createDto.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<OrderResponseDto>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Verify menu items exist and calculate total
                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in createDto.OrderItems)
                {
                    var menuItem = await _unitOfWork.MenuItem.GetAsync(m => m.MenuItemId == item.MenuItemId);
                    if (menuItem == null)
                    {
                        return new ApiResponseDto<OrderResponseDto>
                        {
                            Success = false,
                            Message = $"Menu item {item.MenuItemId} not found",
                            MessageId = "MENU_ITEM_NOT_FOUND"
                        };
                    }

                    var subTotal = menuItem.Price * item.Quantity;
                    totalAmount += subTotal;

                    orderItems.Add(new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        MenuItemId = menuItem.MenuItemId,
                        MenuItemName = menuItem.MenuItemName,
                        Quantity = item.Quantity,
                        Price = menuItem.Price,
                        SubTotal = subTotal
                    });
                }

                // Create order
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    FirebaseOrderId = Guid.NewGuid().ToString(),
                    CustomerId = customerId,
                    ShopId = createDto.ShopId,
                    PaymentMethod = createDto.PaymentMethod,
                    TotalAmount = totalAmount,
                    OrderStatus = "Pending",
                    PaymentStatus = createDto.PaymentMethod == "COD" ? "Pending" : "Pending",
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active"
                };

                // Add order items
                foreach (var item in orderItems)
                {
                    item.OrderId = order.OrderId;
                    order.OrderItems.Add(item);
                }

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveAsync();

                // Sync to Firebase (background task)
                _ = SyncOrderToFirebaseAsync(order.OrderId);

                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == customerId);
                var cName = user != null && !string.IsNullOrEmpty(user.FullName) ? user.FullName : "Unknown";

                var response = MapOrderToResponse(order, shop.ShopName, cName);

                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = true,
                    Message = "Order created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create order: {ex.Message}",
                    MessageId = "ORDER_CREATE_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Orders/{customerId}
        /// Get all orders for customer
        /// Called from: OrderHistoryFragment / profile
        /// </summary>
        public async Task<ApiResponseDto<List<OrderResponseDto>>> GetCustomerOrdersAsync(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return new ApiResponseDto<List<OrderResponseDto>>
                    {
                        Success = false,
                        Message = "Customer ID is required",
                        MessageId = "INVALID_CUSTOMER_ID"
                    };
                }

                var orders = await _unitOfWork.Order.GetOrdersByCustomerAsync(customerId);

                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == customerId);
                var cName = user != null && !string.IsNullOrEmpty(user.FullName) ? user.FullName : "Unknown";

                var orderDtos = new List<OrderResponseDto>();
                foreach (var order in orders)
                {
                    var shop = await _unitOfWork.Shop.GetShopByIdAsync(order.ShopId);
                    orderDtos.Add(MapOrderToResponse(order, shop?.ShopName ?? "Unknown", cName));
                }

                return new ApiResponseDto<List<OrderResponseDto>>
                {
                    Success = true,
                    Message = "Orders retrieved successfully",
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<OrderResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve orders: {ex.Message}",
                    MessageId = "ORDERS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Orders/shop/{shopId}
        /// Get all orders for shop (ShopOwner only)
        /// Called from: ShopOwner order management screen
        /// </summary>
        public async Task<ApiResponseDto<List<OrderResponseDto>>> GetShopOrdersAsync(Guid shopId, string shopOwnerId)
        {
            try
            {
                if (shopId == Guid.Empty || string.IsNullOrEmpty(shopOwnerId))
                {
                    return new ApiResponseDto<List<OrderResponseDto>>
                    {
                        Success = false,
                        Message = "Shop ID and Owner ID are required",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                // Verify ownership
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    return new ApiResponseDto<List<OrderResponseDto>>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == shopOwnerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<List<OrderResponseDto>>
                    {
                        Success = false,
                        Message = "Unauthorized to view this shop's orders",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                var orders = await _unitOfWork.Order.GetOrdersByShopAsync(shopId);

                var userIds = orders.Select(o => o.CustomerId).Distinct().ToList();
                var users = await _unitOfWork.User.GetAsync(u => userIds.Contains(u.Id));
                var userDict = users.ToDictionary(u => u.Id, u => string.IsNullOrEmpty(u.FullName) ? "Unknown" : u.FullName);

                var orderDtos = orders
                    .Select(o => MapOrderToResponse(o, shop.ShopName, userDict.ContainsKey(o.CustomerId) ? userDict[o.CustomerId] : "Unknown"))
                    .ToList();

                return new ApiResponseDto<List<OrderResponseDto>>
                {
                    Success = true,
                    Message = "Shop orders retrieved successfully",
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<OrderResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve orders: {ex.Message}",
                    MessageId = "ORDERS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// PUT /api/v1/Orders/{orderId}/status
        /// Update order status (ShopOwner only)
        /// Status: Confirmed → Delivering → Completed, or Cancelled
        /// Triggers: Firebase update, wallet transaction if needed
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateOrderStatusAsync(
            Guid orderId, string shopOwnerId, UpdateOrderStatusDto updateDto)
        {
            try
            {
                if (orderId == Guid.Empty || string.IsNullOrEmpty(shopOwnerId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Order ID and Owner ID are required",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                // Validate status
                var validStatuses = new[] { "Confirmed", "Delivering", "Completed", "Cancelled" };
                if (!validStatuses.Contains(updateDto.OrderStatus))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Invalid status. Must be: {string.Join(", ", validStatuses)}",
                        MessageId = "INVALID_STATUS"
                    };
                }

                var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Order not found",
                        MessageId = "ORDER_NOT_FOUND"
                    };
                }

                // Verify ownership
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(order.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == shopOwnerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to update this order",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Can only transition to valid next states
                if (order.OrderStatus == "Completed" || order.OrderStatus == "Cancelled")
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Cannot change status of {order.OrderStatus} order",
                        MessageId = "INVALID_STATUS_TRANSITION"
                    };
                }

                // Update status
                order.OrderStatus = updateDto.OrderStatus;
                order.LastSyncedToFirebase = null;  // Mark for Firebase sync

                // Update timestamps
                if (updateDto.OrderStatus == "Delivering")
                {
                    // Optional: set delivery start time
                }
                else if (updateDto.OrderStatus == "Completed")
                {
                    order.CompletedDate = DateTime.UtcNow;

                    // Record payment transaction for ShopOwner if paid by bank
                    if (order.PaymentMethod == "Bank" && order.PaymentStatus == "Paid")
                    {
                        await _transactionService.RecordPaymentTransactionAsync(
                            shopOwner.UserId,
                            order.TotalAmount,
                            order.PayosOrderCode,
                            $"Payment from order {order.FirebaseOrderId}"
                        );
                    }
                }
                else if (updateDto.OrderStatus == "Cancelled")
                {
                    // Refund if already paid
                    if (order.PaymentStatus == "Paid")
                    {
                        await _transactionService.RecordRefundTransactionAsync(
                            order.CustomerId,
                            order.TotalAmount,
                            order.PayosOrderCode,
                            $"Refund for cancelled order {order.FirebaseOrderId}"
                        );
                    }
                }

                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();

                // Sync to Firebase (background)
                _ = SyncOrderToFirebaseAsync(orderId);

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = $"Order status updated to {updateDto.OrderStatus}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to update order: {ex.Message}",
                    MessageId = "ORDER_UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// DELETE /api/v1/Orders/{orderId}
        /// Cancel order (customer only, before confirmed)
        /// Called from: Order detail screen
        /// </summary>
        public async Task<ApiResponseDto<string>> CancelOrderAsync(Guid orderId, string customerId)
        {
            try
            {
                if (orderId == Guid.Empty || string.IsNullOrEmpty(customerId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Order ID and Customer ID are required",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Order not found",
                        MessageId = "ORDER_NOT_FOUND"
                    };
                }

                // Verify ownership
                if (order.CustomerId != customerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to cancel this order",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Can only cancel Pending or Confirmed orders
                if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Cannot cancel {order.OrderStatus} order",
                        MessageId = "INVALID_CANCELLATION"
                    };
                }

                // Refund if already paid
                if (order.PaymentStatus == "Paid")
                {
                    await _transactionService.RecordRefundTransactionAsync(
                        customerId,
                        order.TotalAmount,
                        order.PayosOrderCode,
                        $"Refund for cancelled order {order.FirebaseOrderId}"
                    );
                }

                order.OrderStatus = "Cancelled";
                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();

                // Sync to Firebase
                _ = SyncOrderToFirebaseAsync(orderId);

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Order cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to cancel order: {ex.Message}",
                    MessageId = "ORDER_CANCEL_FAILED"
                };
            }
        }

        /// <summary>
        /// Internal: Sync order to Firebase
        /// Called after order creation or status update
        /// </summary>
        public async Task SyncOrderToFirebaseAsync(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
                if (order == null)
                    return;

                // Sync to Firebase
                await _firebaseService.SyncOrderAsync(order);

                order.LastSyncedToFirebase = DateTime.UtcNow;
                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing order to Firebase: {ex.Message}");
                // Don't throw - Firebase sync is background task
            }
        }

        /// <summary>
        /// Helper: Map Order entity to Response DTO
        /// </summary>
        private OrderResponseDto MapOrderToResponse(Order order, string shopName, string customerName)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                FirebaseOrderId = order.FirebaseOrderId,
                ShopId = order.ShopId,
                ShopName = shopName,
                CustomerName = customerName,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems
                    .Select(oi => new OrderResponseDto.OrderItemResponseDto
                    {
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItemName,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        SubTotal = oi.SubTotal
                    })
                    .ToList(),
                CreatedDate = order.CreatedDate,
                PaidDate = order.PaidDate,
                DeliveredDate = order.DeliveredDate,
                CompletedDate = order.CompletedDate
            };
        }
    }
}
