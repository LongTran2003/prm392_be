using FoodOrderSystem.Services.IServices;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FoodOrderSystem.Services.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private string _firebaseProjectId;
        private string _databaseUrl;

        public FirebaseService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _firebaseProjectId = _configuration["Firebase:ProjectId"] ?? "";
            _databaseUrl = $"https://{_firebaseProjectId}.firebasedatabase.app";
        }

        /// <summary>
        /// Initialize Firebase (just validate config here)
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_firebaseProjectId))
                {
                    throw new Exception("Firebase:ProjectId not configured in appsettings.json");
                }

                // Test connection by reading a test node
                var testUrl = $"{_databaseUrl}/test.json";
                var response = await _httpClient.GetAsync(testUrl);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Firebase initialized successfully - {_databaseUrl}");
                }
                else
                {
                    Console.WriteLine($"Firebase returned status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase initialization warning: {ex.Message}");
                // Don't throw - Firebase is optional
            }
        }

        /// <summary>
        /// Sync order to Firebase using REST API
        /// </summary>
        public async Task SyncOrderAsync(Models.Domains.Order order)
        {
            try
            {
                if (string.IsNullOrEmpty(_firebaseProjectId))
                    return;

                var firebaseOrder = new
                {
                    orderId = order.FirebaseOrderId,
                    customerId = order.CustomerId,
                    shopId = order.ShopId.ToString(),
                    orderStatus = order.OrderStatus,
                    paymentMethod = order.PaymentMethod,
                    paymentStatus = order.PaymentStatus,
                    totalAmount = order.TotalAmount,
                    items = order.OrderItems.Select(x => new
                    {
                        menuItemId = x.MenuItemId.ToString(),
                        name = x.MenuItemName,
                        quantity = x.Quantity,
                        price = x.Price,
                        subTotal = x.SubTotal
                    }).ToList(),
                    createdAt = order.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    updatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Main order
                var url = $"{_databaseUrl}/orders/{order.FirebaseOrderId}.json";
                var content = new StringContent(
                    JsonSerializer.Serialize(firebaseOrder),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync(url, content);

                // Index by customer
                var custUrl = $"{_databaseUrl}/customer_orders/{order.CustomerId}/{order.FirebaseOrderId}.json";
                var custContent = new StringContent(
                    JsonSerializer.Serialize(new { status = order.OrderStatus }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync(custUrl, custContent);

                // Index by shop
                var shopUrl = $"{_databaseUrl}/shop_orders/{order.ShopId}/{order.FirebaseOrderId}.json";
                var shopContent = new StringContent(
                    JsonSerializer.Serialize(new { status = order.OrderStatus }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync(shopUrl, shopContent);

                Console.WriteLine($"Order {order.FirebaseOrderId} synced to Firebase");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing order to Firebase: {ex.Message}");
            }
        }

        /// <summary>
        /// Update order status in Firebase
        /// </summary>
        public async Task UpdateOrderStatusAsync(string firebaseOrderId, string newStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(_firebaseProjectId))
                    return;

                var url = $"{_databaseUrl}/orders/{firebaseOrderId}/orderStatus.json";
                var content = new StringContent(
                    JsonSerializer.Serialize(newStatus),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync(url, content);

                Console.WriteLine($"Order {firebaseOrderId} status updated to {newStatus}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
            }
        }

        /// <summary>
        /// Sync shop to Firebase
        /// </summary>
        public async Task SyncShopAsync(Models.Domains.Shop shop)
        {
            try
            {
                if (string.IsNullOrEmpty(_firebaseProjectId))
                    return;

                var firebaseShop = new
                {
                    shopId = shop.ShopId.ToString(),
                    name = shop.ShopName,
                    address = shop.Address,
                    imageUrl = shop.ImageUrl,
                    status = shop.Status,
                    totalOrders = shop.TotalOrders,
                    averageRating = shop.AverageRating,
                    latitude = shop.Latitude,
                    longitude = shop.Longitude,
                    openHours = shop.OpenHours.ToString(@"hh\:mm"),
                    closeHours = shop.CloseHours.ToString(@"hh\:mm")
                };

                var url = $"{_databaseUrl}/shops/{shop.ShopId}.json";
                var content = new StringContent(
                    JsonSerializer.Serialize(firebaseShop),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync(url, content);

                Console.WriteLine($"Shop {shop.ShopId} synced to Firebase");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing shop to Firebase: {ex.Message}");
            }
        }
    }
}