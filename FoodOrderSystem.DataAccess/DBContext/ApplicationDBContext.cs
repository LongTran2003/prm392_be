using FoodOrderSystem.DataAccess.Seed;
using FoodOrderSystem.Models.Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.DataAccess.DBContext
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        //===========================================================================
        // Define DbSet properties for your entities here
        //===========================================================================
        public DbSet<Student> Students { get; set; }
        public DbSet<ShopOwner> ShopOwners { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed data
            ApplicationDbContextSeed.SeedAdminAccount(modelBuilder);

            //===========================================================================
            // Configure your entity relationships and constraints here
            //===========================================================================

            // ShopOwner relationship
            modelBuilder.Entity<ShopOwner>()
                .HasOne(so => so.ApplicationUser)
                .WithOne(u => u.ShopOwner)
                .HasForeignKey<ShopOwner>(so => so.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithMany(u => u.Students)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category-MenuItem relationship
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop relationships
            modelBuilder.Entity<Shop>()
                .HasOne(s => s.ShopOwner)
                .WithMany(so => so.Shops)
                .HasForeignKey(s => s.ShopOwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Shop)
                .WithMany(s => s.MenuItems)
                .HasForeignKey(m => m.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for better query performance
            modelBuilder.Entity<Shop>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Shop>()
                .HasIndex(s => s.ShopOwnerId);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Shop)
                .WithMany()
                .HasForeignKey(o => o.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.PaymentStatus);

            // Transaction relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for queries
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionType);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Status);
        }
    }
}
