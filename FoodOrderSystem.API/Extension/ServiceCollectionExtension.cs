using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.DataAccess.Repositories;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Services.Mapping;
using FoodOrderSystem.Services.Services;
using FoodOrderSystem.Utilities.Templates.FileUpload;
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

            // 2. AutoMapper & UnitOfWork
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Các Service cơ bản
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<FileUploadService>();

            //========================================================================
            // 4. Các dịch vụ Scoped khác (nếu có)
            //========================================================================
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IPaymentService, PaymentService>();

            //========================================================================
            // 5. Các dịch vụ HttpClient khác (nếu có)
            //========================================================================
            services.AddHttpClient<IPaymentService, PaymentService>();

            return services;
        }
    }
}
