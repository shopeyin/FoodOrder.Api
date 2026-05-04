using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Ordering.Application.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Modules.Restaurants.Infrastructure;

public sealed class EfMenuItemReader : IMenuItemReader
{
    private readonly FoodOrderDbContext _db;

    public EfMenuItemReader(FoodOrderDbContext db)
    {
        _db = db;
    }

    public async Task<MenuItemSnapshot?> GetActiveMenuItemAsync(Guid menuItemId, CancellationToken ct)
    {
        return await _db.MenuItems
            .AsNoTracking()
            .Where(m => m.MenuItemId == menuItemId && m.IsActive)
            .Select(m => new MenuItemSnapshot(
                m.MenuItemId,
                m.Name,
                m.PriceAmount))
            .SingleOrDefaultAsync(ct);
    }
}


