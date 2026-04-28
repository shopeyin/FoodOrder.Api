using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Domain.ValueObjects;
using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using System;

namespace FoodOrder.Api.IntegrationTests;

public static class TestDataSeeder
{
    public static readonly Guid CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid RestaurantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid MenuItemId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001");

    public static void Seed(FoodOrderDbContext db)
    {
        if (db.Customers.Any())
            return;

        var customer = new Customer("Test Customer");
        var restaurant = new Restaurant("Test Restaurant");

        restaurant.AddMenuItem("Classic Burger", new Money(9.99m));

        db.Customers.Add(customer);
        db.Restaurants.Add(restaurant);
        db.SaveChanges();
    }
}
