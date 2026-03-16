using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.DataAccess.Repositories;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Services.Mapping;
using FoodOrderSystem.Services.Services;
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

            //========================================================================
            // 4. Các dịch vụ khác (nếu có)
            //========================================================================
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IUserService, UserService>();


            return services;
        }
    }
}
