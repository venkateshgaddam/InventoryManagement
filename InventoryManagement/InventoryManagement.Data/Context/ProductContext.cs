using InventoryManagement.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}