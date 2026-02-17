using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodOrder.Api.Repositories.Efcore
{
    public sealed class EfOrderRepository : IOrderRepository
    {
        private readonly FoodOrderDbContext _db;
        public EfOrderRepository(FoodOrderDbContext db) => _db = db;

        public async Task AddAsync(Order order, CancellationToken ct) => await _db.Orders.AddAsync(order, ct);

        public Task<Order?> GetByIdWithItemsAsync(Guid orderId, CancellationToken ct)
        {
            // Choose ONE include style based on how you mapped Order items:

            // If you mapped Items directly (HasMany(x => x.Items)):
            return _db.Orders.Include(o => o.Items).SingleOrDefaultAsync(o => o.OrderId == orderId, ct);

            // If you mapped backing field "_items", use this instead:
            // return _db.Orders.Include("_items").SingleOrDefaultAsync(o => o.OrderId == orderId, ct);
        }



        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
            => _db.Orders.SingleOrDefaultAsync(o => o.OrderId == orderId, ct);

        public Task<bool> HasAnyItemsAsync(Guid orderId, CancellationToken ct)
            => _db.OrderItems.AnyAsync(i => i.OrderId == orderId, ct);




        public Task SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);
    }

}
