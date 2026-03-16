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
        }
    }
}
