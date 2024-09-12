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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تعريف العلاقة بين Product و Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<AppUser>()
           .HasOne(u => u.Cart)
           .WithOne(c => c.AppUser)
           .HasForeignKey<Cart>(c => c.AppUserId);


            modelBuilder.Entity<Cart>()
           .HasMany(c => c.CartItems)
           .WithOne(ci => ci.Cart)
           .HasForeignKey(ci => ci.CartId);

        }

    }
}
