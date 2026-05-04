using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Common.ValueObjects;
using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Restaurants.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Modules.Restaurants.Application
{
    public class RestaurantService
    {
        private readonly FoodOrderDbContext _db;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(FoodOrderDbContext db, ILogger<RestaurantService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Guid> CreateRestaurant(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name is required.");

            var restaurant = new Restaurant(name.Trim());
            _db.Restaurants.Add(restaurant);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Created restaurant {RestaurantId} with name {RestaurantName}", restaurant.RestaurantId, restaurant.Name);
            return restaurant.RestaurantId;
        }


        public async Task<Guid> AddMenuItem(Guid restaurantId, string name, decimal price, CancellationToken ct)
        {
            if (price < 0)
                throw new DomainException("Price cannot be negative.");

            var restaurant = await _db.Restaurants
                .Include(r => r.MenuItems)
                .SingleOrDefaultAsync(r => r.RestaurantId == restaurantId, ct);

            if (restaurant is null)
                throw new KeyNotFoundException("Restaurant not found.");

            MenuItem menuItem;

            try
            {
                menuItem = restaurant.AddMenuItem(name, new Money(price));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                throw new DomainException(ex.Message);
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Added menu item {MenuItemId} to restaurant {RestaurantId} with name {MenuItemName}",
                menuItem.MenuItemId,
                restaurantId,
                menuItem.Name);

            return menuItem.MenuItemId;
        }


        public async Task UpdateMenuItemPrice(Guid restaurantId, Guid menuItemId, decimal price, CancellationToken ct)
        {
            if (price <= 0)
                throw new DomainException("Price cannot be negative.");

            var restaurant = await _db.Restaurants
                .Include(r => r.MenuItems)
                .SingleOrDefaultAsync(r => r.RestaurantId == restaurantId, ct);

            if (restaurant is null)
                throw new KeyNotFoundException("Restaurant not found.");

            try
            {
                restaurant.ChangeMenuItemPrice(menuItemId, new Money(price));
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainException(ex.Message);
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Updated price for menu item {MenuItemId} in restaurant {RestaurantId} to {Price}",
                menuItemId,
                restaurantId,
                price);
        }

        public async Task DeactivateMenuItem(Guid restaurantId, Guid menuItemId, CancellationToken ct)
        {
            var restaurant = await _db.Restaurants
                .Include(r => r.MenuItems)
                .SingleOrDefaultAsync(r => r.RestaurantId == restaurantId, ct);

            if (restaurant is null)
                throw new KeyNotFoundException("Restaurant not found.");

            try
            {
                restaurant.DeactivateMenuItem(menuItemId);
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainException(ex.Message);
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Deactivated menu item {MenuItemId} in restaurant {RestaurantId}",
                menuItemId,
                restaurantId);
        }


    }
}
