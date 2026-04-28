using FoodOrder.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Data
{
    public class FoodOrderDbContext : DbContext
    {
        public FoodOrderDbContext(DbContextOptions<FoodOrderDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

                b.Property(x => x.RestaurantId)
                    .ValueGeneratedNever();

                b.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                b.HasMany(x => x.MenuItems)
                    .WithOne()
                    .HasForeignKey(x => x.RestaurantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MenuItem>(b =>
            {
                b.HasKey(x => x.MenuItemId);

                b.Property(x => x.MenuItemId)
                .ValueGeneratedNever();

                b.Property(x => x.RestaurantId)
                .IsRequired();

                b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

                b.Property(x => x.PriceAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

                b.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

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
