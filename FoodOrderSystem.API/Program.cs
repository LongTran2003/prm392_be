using FoodOrderSystem.API.Extension;
using FoodOrderSystem.API.Middleware;
using FoodOrderSystem.DataAccess.DBContext;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Net.payOS;


var builder = WebApplication.CreateBuilder(args);

//================================================================
// Add services to the container.
//================================================================

builder.Services.AddControllers();

// Configure DbContext with SQL Server  
var connectionString =
    builder.Configuration.GetConnectionString(StaticConnectionString.PostgreSqlConnection)
    ?? builder.Configuration.GetConnectionString(StaticConnectionString.PostgreDefaultConnection);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
        npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)));

// Configure Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

// Thêm d?ch v? Swagger  
builder.Services.AddSwaggerGen(options =>
{
// B?o m?t Swagger v?i JWT
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Please enter your token with this format: \"Bearer YOUR_TOKEN\""
});

options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });

// API document
options.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "Food Order System API",
    Version = "v1",
    Description = "API documentation for Food Order System"
});
options.EnableAnnotations();

    // ??c comment t? XML ?? hi?n th? trên Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Register services from Extensions
builder.Services.RegisterServices(builder.Configuration);

// Khởi tạo và đăng ký PayOS
PayOS payOS = new PayOS(
    builder.Configuration["PayOS:ClientId"],
    builder.Configuration["PayOS:ApiKey"],
    builder.Configuration["PayOS:ChecksumKey"]
);
builder.Services.AddSingleton(payOS);


//================================================================
var app = builder.Build();
//================================================================


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// Enable static files (for image serving from wwwroot)
app.UseStaticFiles();

// Add custom exception middleware
app.UseMiddleware<GlobalExceptionHandllingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize Firebase on startup
using (var scope = app.Services.CreateScope())
{
    var firebaseService = scope.ServiceProvider.GetRequiredService<IFirebaseService>();
    await firebaseService.InitializeAsync();
    Console.WriteLine("Firebase initialized and ready");
}

app.Run();
