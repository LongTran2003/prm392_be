using StackExchange.Redis;

namespace FoodOrderSystem.API.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,
            ConfigurationManager builderConfiguration)
        {
            // 1. Redis
            // Đọc chuỗi kết nối Redis từ file cấu hình
            var redisConnectionString = builderConfiguration.GetValue<string>("Redis:ConnectionString");
            // Đăng ký IConnectionMultiplexer
            var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);


            //========================================================================
            // 2. Các dịch vụ khác (nếu có)
            //========================================================================


            return services;
        }
    }
}
