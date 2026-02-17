using FoodOrder.Api.Application.Customers;
using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Contracts.Customers;
using FoodOrder.Api.Contracts.Customers.Queries;
using FoodOrder.Api.Data;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FoodOrder.Api.Controllers
{

    [Route("customers")]
    [ApiController]
    public sealed class CustomersController : ControllerBase
    {
        private readonly CustomerService _svc;
        private readonly FoodOrderDbContext _db;
        private readonly ICustomerOrderQueries _customerOrderQueries;

        public CustomersController(CustomerService svc, FoodOrderDbContext db)
        {
            _svc = svc;
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<CreateCustomerResponse>> Create([FromBody] CreateCustomerRequest req, CancellationToken ct)
        {
            var id = await _svc.CreateCustomer(req.Name, ct);
            return CreatedAtAction(nameof(GetById), new { customerId = id }, new CreateCustomerResponse(id));
        }

        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetById(Guid customerId, CancellationToken ct)
        {
            var customer = await _db.Customers.AsNoTracking()
                .SingleOrDefaultAsync(c => c.CustomerId == customerId, ct);

            if (customer is null) return NotFound();

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
