using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Data
{
    public static class SeedData
    {
        public static async Task EnsureSeededAsync(FoodOrderDbContext db, CancellationToken ct = default)
        {
            var alreadySeeded =
                await db.Customers.AnyAsync(ct) ||
                await db.Restaurants.AnyAsync(ct) ||
                await db.MenuItems.AnyAsync(ct);

            if (alreadySeeded) return;

            // Fixed IDs for easy testing
            var customer1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var customer2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var restaurant1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var restaurant2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            var customer1 = new Customer(customer1Id, "Ola Shopeyin");
            var customer2 = new Customer(customer2Id, "Ada Lovelace");

            var restaurant1 = new Restaurant(restaurant1Id, "Burger Palace");
            var restaurant2 = new Restaurant(restaurant2Id, "Pasta Corner");

            var menuItems = new[]
            {
            new MenuItem(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"),
                restaurant1Id,
                "Classic Burger",
                new Money(9.99m)
            ),
            new MenuItem(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000002"),
                restaurant1Id,
                "Fries",
                new Money(3.50m)
            ),
            new MenuItem(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000001"),
                restaurant2Id,
                "Spaghetti Bolognese",
                new Money(12.75m)
            ),
            new MenuItem(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000002"),
                restaurant2Id,
                "Garlic Bread",
                new Money(4.25m)
            ),
        };

            db.Customers.AddRange(customer1, customer2);
            db.Restaurants.AddRange(restaurant1, restaurant2);
            db.MenuItems.AddRange(menuItems);

            await db.SaveChangesAsync(ct);
        }
    }




}
