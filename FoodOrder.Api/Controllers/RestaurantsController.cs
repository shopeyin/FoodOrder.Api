using FoodOrder.Api.Application.Restaurants;
using FoodOrder.Api.Contracts.Restaurants;
using FoodOrder.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace FoodOrder.Api.Controllers
{
    [ApiController]
    [Route("restaurants")]
    public sealed class RestaurantsController : ControllerBase
    {
        private readonly RestaurantService _svc;
        private readonly FoodOrderDbContext _db;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(RestaurantService svc, FoodOrderDbContext db, ILogger<RestaurantsController> logger)
        {
            _svc = svc;
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateRestaurantResponse>> Create(CreateRestaurantRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Creating restaurant with name {RestaurantName}", req.Name);
            var id = await _svc.CreateRestaurant(req.Name, ct);
            _logger.LogInformation("Created restaurant {RestaurantId}", id);
            return CreatedAtAction(nameof(GetById), new { restaurantId = id }, new CreateRestaurantResponse(id));
        }

        [HttpGet("{restaurantId:guid}")]
        public async Task<IActionResult> GetById(Guid restaurantId, CancellationToken ct)
        {
            _logger.LogInformation("Fetching restaurant {RestaurantId}", restaurantId);
            var restaurant = await _db.Restaurants.AsNoTracking()
                .SingleOrDefaultAsync(r => r.RestaurantId == restaurantId, ct);

            if (restaurant is null) return NotFound();

            var menu = await _db.MenuItems.AsNoTracking()
                .Where(m => m.RestaurantId == restaurantId)
                .OrderByDescending(m => m.IsActive)
                .ThenBy(m => m.Name)
                .Select(m => new { m.MenuItemId, m.Name, Price = m.Price.Amount, m.IsActive })
                .ToListAsync(ct);

            return Ok(new
            {
                restaurant.RestaurantId,
                restaurant.Name,
                MenuItems = menu
            });
        }

        [HttpPost("{restaurantId:guid}/menu-items")]
        public async Task<ActionResult<AddMenuItemResponse>> AddMenuItem(Guid restaurantId, AddMenuItemRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Adding menu item to restaurant {RestaurantId}", restaurantId);
            var menuItemId = await _svc.AddMenuItem(restaurantId, req.Name, req.Price, ct);
            _logger.LogInformation("Added menu item {MenuItemId} to restaurant {RestaurantId}", menuItemId, restaurantId);
            return CreatedAtAction(nameof(GetById), new { restaurantId }, new AddMenuItemResponse(menuItemId));
        }

        [HttpPatch("{restaurantId:guid}/menu-items/{menuItemId:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid restaurantId, Guid menuItemId, UpdateMenuItemPriceRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Updating price for menu item {MenuItemId} in restaurant {RestaurantId}", menuItemId, restaurantId);
            await _svc.UpdateMenuItemPrice(restaurantId, menuItemId, req.Price, ct);
            _logger.LogInformation("Updated price for menu item {MenuItemId} in restaurant {RestaurantId}", menuItemId, restaurantId);
            return NoContent();
        }

        [HttpDelete("{restaurantId:guid}/menu-items/{menuItemId:guid}")]
        public async Task<IActionResult> Deactivate(Guid restaurantId, Guid menuItemId, CancellationToken ct)
        {
            _logger.LogInformation("Deactivating menu item {MenuItemId} in restaurant {RestaurantId}", menuItemId, restaurantId);
            await _svc.DeactivateMenuItem(restaurantId, menuItemId, ct);
            _logger.LogInformation("Deactivated menu item {MenuItemId} in restaurant {RestaurantId}", menuItemId, restaurantId);
            return NoContent();
        }
    }

}
