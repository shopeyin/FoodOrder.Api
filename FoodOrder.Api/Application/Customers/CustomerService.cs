using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;


namespace FoodOrder.Api.Application.Customers
{
    public sealed class CustomerService
    {
        private readonly FoodOrderDbContext _db;

        public CustomerService(FoodOrderDbContext db) => _db = db;

        public async Task<Guid> CreateCustomer(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name is required.");

            var customer = new Customer(Guid.NewGuid(), name.Trim());
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync(ct);

            return customer.CustomerId;
        }
    }

}
