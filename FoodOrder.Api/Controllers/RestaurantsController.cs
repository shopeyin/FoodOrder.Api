using FoodOrder.Api.Application.Restaurants;
using FoodOrder.Api.Contracts.Restaurants;
using FoodOrder.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FoodOrder.Api.Controllers
{
    [ApiController]
    [Route("restaurants")]
    public sealed class RestaurantsController : ControllerBase
    {
        private readonly RestaurantService _svc;
        private readonly FoodOrderDbContext _db;

        public RestaurantsController(RestaurantService svc, FoodOrderDbContext db)
        {
            _svc = svc;
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<CreateRestaurantResponse>> Create(CreateRestaurantRequest req, CancellationToken ct)
        {
            var id = await _svc.CreateRestaurant(req.Name, ct);
            return CreatedAtAction(nameof(GetById), new { restaurantId = id }, new CreateRestaurantResponse(id));
        }

        [HttpGet("{restaurantId:guid}")]
        public async Task<IActionResult> GetById(Guid restaurantId, CancellationToken ct)
        {
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
            var menuItemId = await _svc.AddMenuItem(restaurantId, req.Name, req.Price, ct);
            return CreatedAtAction(nameof(GetById), new { restaurantId }, new AddMenuItemResponse(menuItemId));
        }

        [HttpPatch("{restaurantId:guid}/menu-items/{menuItemId:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid restaurantId, Guid menuItemId, UpdateMenuItemPriceRequest req, CancellationToken ct)
        {
            await _svc.UpdateMenuItemPrice(restaurantId, menuItemId, req.Price, ct);
            return NoContent();
        }

        [HttpDelete("{restaurantId:guid}/menu-items/{menuItemId:guid}")]
        public async Task<IActionResult> Deactivate(Guid restaurantId, Guid menuItemId, CancellationToken ct)
        {
            await _svc.DeactivateMenuItem(restaurantId, menuItemId, ct);
            return NoContent();
        }
    }

}
