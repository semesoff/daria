using Microsoft.EntityFrameworkCore;
using MenuOrder.Models;

namespace MenuOrder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=menuorder.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация для правильного наследования
            modelBuilder.Entity<Dish>().ToTable("Dishes");
            modelBuilder.Entity<Beverage>().ToTable("Beverages");

            // Конфигурация для Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.ItemsJson).HasColumnName("Items");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
}
