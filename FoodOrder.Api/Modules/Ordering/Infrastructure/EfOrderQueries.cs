using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Ordering.Contracts.Queries;
using FoodOrder.Api.Modules.Ordering.Infrastructure.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Modules.Ordering.Infrastructure
{
    public sealed class EfOrderQueries : IOrderQueries
    {
        private readonly FoodOrderDbContext _db;
        public EfOrderQueries(FoodOrderDbContext db) => _db = db;

        public async Task<OrderDetailsDto> GetOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.OrderId == orderId, ct);

            if (order is null) throw new KeyNotFoundException("Order not found.");

            var total = order.Total.Amount;
            var items = order.Items.Select(i =>
                        new OrderItemDto(
                            i.OrderItemId,
                            i.MenuItemId,
                            i.NameSnapshot,
                            i.UnitPriceAmount,
                            i.Quantity,
                            i.UnitPriceAmount * i.Quantity
                        )
                    ).ToList();

            return new OrderDetailsDto(
                order.OrderId,
                order.CustomerId,
                order.Status.ToString(),
                total,
                items
            );

        }
    }

}
