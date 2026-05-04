using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Ordering.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodOrder.Api.Modules.Customers.Infrastructure
{
    public sealed class EfCustomerReader : ICustomerReader
    {
        private readonly FoodOrderDbContext _db;

        public EfCustomerReader(FoodOrderDbContext db)
        {
            _db = db;
        }

        public Task<bool> ExistsAsync(Guid customerId, CancellationToken ct)
        {
            return _db.Customers
                .AsNoTracking()
                .AnyAsync(c => c.CustomerId == customerId, ct);
        }
    }


}
