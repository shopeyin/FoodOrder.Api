using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Application.Restaurants
{
    public class RestaurantService
    {
        private readonly FoodOrderDbContext _db;

        public RestaurantService(FoodOrderDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> CreateRestaurant(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name is required.");

            var restaurant = new Restaurant(Guid.NewGuid(), name.Trim());
            _db.Restaurants.Add(restaurant);
            await _db.SaveChangesAsync(ct);
            return restaurant.RestaurantId;
        }


        public async Task<Guid> AddMenuItem(Guid restaurantId, string name, decimal price, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Menu item name is required.");

            if (price < 0)
                throw new DomainException("Price cannot be negative.");

            var restaurantExists = await _db.Restaurants.AnyAsync(r => r.RestaurantId == restaurantId, ct);
            if (!restaurantExists)
                throw new KeyNotFoundException("Restaurant not found.");

            // Optional: prevent duplicate active names per restaurant
            var duplicate = await _db.MenuItems.AnyAsync(m =>
                m.RestaurantId == restaurantId && m.Name == name.Trim() && m.IsActive, ct);

            if (duplicate)
                throw new DomainException("An active menu item with the same name already exists for this restaurant.");

            var menuItem = new MenuItem(Guid.NewGuid(), restaurantId, name.Trim(), new Money(price));
            _db.MenuItems.Add(menuItem);
            await _db.SaveChangesAsync(ct);

            return menuItem.MenuItemId;
        }

        public async Task UpdateMenuItemPrice(Guid restaurantId, Guid menuItemId, decimal price, CancellationToken ct)
        {
            if (price < 0)
                throw new DomainException("Price cannot be negative.");

            var menuItem = await _db.MenuItems.SingleOrDefaultAsync(m =>
                m.MenuItemId == menuItemId && m.RestaurantId == restaurantId, ct);

            if (menuItem is null)
                throw new KeyNotFoundException("Menu item not found for this restaurant.");

            try
            {
                menuItem.ChangePrice(new Money(price));
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainException(ex.Message);
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task DeactivateMenuItem(Guid restaurantId, Guid menuItemId, CancellationToken ct)
        {
            var menuItem = await _db.MenuItems.SingleOrDefaultAsync(m =>
                m.MenuItemId == menuItemId && m.RestaurantId == restaurantId, ct);

            if (menuItem is null)
                throw new KeyNotFoundException("Menu item not found for this restaurant.");

            menuItem.Deactivate();
            await _db.SaveChangesAsync(ct);
        }


    }
}
