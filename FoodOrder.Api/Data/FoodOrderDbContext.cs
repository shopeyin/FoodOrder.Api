using FoodOrder.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Data
{
    public class FoodOrderDbContext : DbContext
    {
        public FoodOrderDbContext(DbContextOptions<FoodOrderDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(x => x.CustomerId);
                b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<Restaurant>(b =>
            {
                b.HasKey(x => x.RestaurantId);
                b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<MenuItem>(b =>
            {
                b.HasKey(x => x.MenuItemId);
                b.Property(x => x.Name).HasMaxLength(200).IsRequired();
                b.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                b.Property(x => x.PriceAmount).HasColumnType("decimal(18,2)").IsRequired();
                b.HasIndex(x => new { x.RestaurantId, x.Name });
            });

            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(x => x.OrderId);
                b.Property(x => x.Status).HasConversion<int>().IsRequired();

                b.HasMany(x => x.Items)
                 .WithOne()
                 .HasForeignKey(i => i.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(b =>
            {
                b.HasKey(x => x.OrderItemId);

                b.Property(x => x.OrderItemId)
                 .ValueGeneratedNever(); // client generates Guid with Guid.NewGuid()

                b.Property(x => x.NameSnapshot).HasMaxLength(200).IsRequired();
                b.Property(x => x.UnitPriceAmount).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(x => x.Quantity).IsRequired();
                b.HasIndex(x => x.OrderId);
            });

        }
    }


}
