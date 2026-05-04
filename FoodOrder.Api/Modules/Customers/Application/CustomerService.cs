using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Customers.Domain;
using Microsoft.Extensions.Logging;

namespace FoodOrder.Api.Modules.Customers.Application
{
    public sealed class CustomerService
    {
        private readonly FoodOrderDbContext _db;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(FoodOrderDbContext db, ILogger<CustomerService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Guid> CreateCustomer(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name is required.");

            var customer = new Customer(name.Trim());
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Created customer {CustomerId} with name {CustomerName}", customer.CustomerId, customer.Name);

            return customer.CustomerId;
        }
    }
}
