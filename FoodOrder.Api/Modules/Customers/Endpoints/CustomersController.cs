using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Customers.Application;
using FoodOrder.Api.Modules.Customers.Contracts;
using FoodOrder.Api.Modules.Customers.Contracts.Queries;
using FoodOrder.Api.Modules.Ordering.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodOrder.Api.Modules.Customers.Endpoints
{

    [Route("customers")]
    [ApiController]
    public sealed class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly FoodOrderDbContext _db;
        private readonly ICustomerOrderQueries _customerOrderQueries;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(CustomerService customerService, FoodOrderDbContext db, ICustomerOrderQueries customerOrderQueries, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _db = db;
            _customerOrderQueries = customerOrderQueries;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateCustomerResponse>> Create([FromBody] CreateCustomerRequest req, CancellationToken ct)
        {
            var id = await _customerService.CreateCustomer(req.Name, ct);
            return CreatedAtAction(nameof(GetById), new { customerId = id }, new CreateCustomerResponse(id));
        }

        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetById(Guid customerId, CancellationToken ct)
        {
            _logger.LogInformation(10, "Fetching customer with ID {CustomerId}", customerId);
            var customer = await _db.Customers.AsNoTracking()
                .SingleOrDefaultAsync(c => c.CustomerId == customerId, ct);

            if (customer is null) return NotFound();
            _logger.LogInformation(10, "Customer found: {CustomerName}", customer.Name);
            return Ok(new { customer.CustomerId, customer.Name });
        }


        [HttpGet("{customerId:guid}/orders")]
        public async Task<ActionResult<PagedCustomerOrdersDto>> GetOrders(Guid customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var result = await _customerOrderQueries.GetCustomerOrdersAsync(customerId, page, pageSize, ct);
            return Ok(result);

        }

    }

}
