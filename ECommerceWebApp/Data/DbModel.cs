using ECommerceWebApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebApp.Data
{
    public class DbModel : DbContext
    {
        public DbModel(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}