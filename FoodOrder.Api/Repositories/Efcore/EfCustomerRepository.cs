using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace FoodOrder.Api.Repositories.Efcore
{
    public sealed class EfCustomerRepository : ICustomerRepository
    {
        private readonly FoodOrderDbContext _db;
        public EfCustomerRepository(FoodOrderDbContext db) => _db = db;
        public Task<bool> ExistsAsync(Guid customerId, CancellationToken ct)
           => _db.Customers.AnyAsync(c => c.CustomerId == customerId, ct);
    }

}
