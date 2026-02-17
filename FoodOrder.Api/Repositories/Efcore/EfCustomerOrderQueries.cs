using FoodOrder.Api.Contracts.Customers.Queries;
using FoodOrder.Api.Data;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Repositories.Efcore
{
    public sealed class EfCustomerOrderQueries : ICustomerOrderQueries
    {
        private readonly FoodOrderDbContext _db;

        public EfCustomerOrderQueries(FoodOrderDbContext db) => _db = db;

        public async Task<PagedCustomerOrdersDto> GetCustomerOrdersAsync(Guid customerId, int page, int pageSize, CancellationToken ct)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var customerExists = await _db.Customers.AsNoTracking()
                .AnyAsync(c => c.CustomerId == customerId, ct);

            if (!customerExists)
                throw new KeyNotFoundException("Customer not found.");

            var baseQuery = _db.Orders.AsNoTracking()
                .Where(o => o.CustomerId == customerId);

            var totalCount = await baseQuery.CountAsync(ct);
            var orders = await baseQuery
                     .OrderByDescending(o => o.OrderId)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .GroupJoin(
                         _db.OrderItems.AsNoTracking(),
                         o => o.OrderId,
                         i => i.OrderId,
                         (o, items) => new CustomerOrderSummaryDto(
                             o.OrderId,
                             o.Status.ToString(),
                             items.Count(),
                             items.Sum(x => x.UnitPriceAmount * x.Quantity)
                         ))
                     .ToListAsync(ct);

            return new PagedCustomerOrdersDto(customerId, page, pageSize, totalCount, orders);

        }
    }

}


