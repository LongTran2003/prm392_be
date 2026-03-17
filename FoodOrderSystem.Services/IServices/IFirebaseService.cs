namespace FoodOrderSystem.Services.IServices
{
    public interface IFirebaseService
    {
        // Sync order to Firebase Realtime Database
        Task SyncOrderAsync(Models.Domains.Order order);

        // Update order status in Firebase
        Task UpdateOrderStatusAsync(string firebaseOrderId, string newStatus);

        // Sync shop to Firebase
        Task SyncShopAsync(Models.Domains.Shop shop);

        // Initialize Firebase (call once on startup)
        Task InitializeAsync();
    }
}
