
using FoodOrder.Api.Common.ValueObjects;
using FoodOrder.Api.Modules.Customers.Domain;
using FoodOrder.Api.Modules.Restaurants.Domain;
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

            var customer1 = new Customer("Oladimeji Shopeyin");
            var customer2 = new Customer("Ada Lovelace");

            var restaurant1 = new Restaurant("Burger Palace");
            var restaurant2 = new Restaurant("Pasta Corner");

            restaurant1.AddMenuItem("Classic Burger", new Money(9.99m));
            restaurant1.AddMenuItem("Fries", new Money(3.50m));

            restaurant2.AddMenuItem("Spaghetti Bolognese", new Money(12.75m));
            restaurant2.AddMenuItem("Garlic Bread", new Money(4.25m));

            db.Customers.AddRange(customer1, customer2);
            db.Restaurants.AddRange(restaurant1, restaurant2);

            await db.SaveChangesAsync(ct);
        }
    }






}
