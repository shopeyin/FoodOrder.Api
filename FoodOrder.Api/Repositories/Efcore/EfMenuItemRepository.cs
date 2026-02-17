using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace FoodOrder.Api.Repositories.Efcore
{
    public sealed class EfMenuItemRepository : IMenuItemRepository
    {
        private readonly FoodOrderDbContext _db;
        public EfMenuItemRepository(FoodOrderDbContext db) => _db = db;

        public Task<MenuItem?> GetByIdAsync(Guid menuItemId, CancellationToken ct)
            => _db.MenuItems.SingleOrDefaultAsync(m => m.MenuItemId == menuItemId, ct);
    }

}
