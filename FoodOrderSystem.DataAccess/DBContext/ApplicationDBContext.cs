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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed data
            ApplicationDbContextSeed.SeedAdminAccount(modelBuilder);

            //===========================================================================
            // Configure your entity relationships and constraints here
            //===========================================================================


        }
    }
}
