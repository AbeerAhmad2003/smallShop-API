using Microsoft.EntityFrameworkCore;
using smallShop.Models;

namespace smallShop.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {
        
        
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تعريف العلاقة بين Product و Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
           
        }
    }
}
