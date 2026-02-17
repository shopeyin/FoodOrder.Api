using FoodOrder.Api.Application.Orders;
using FoodOrder.Api.Contracts.Orders;
using FoodOrder.Api.Contracts.Orders.Queries;
using FoodOrder.Api.Data;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Api.Controllers
{
    [ApiController]
    [Route("orders")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly OrderService _svc;
        private readonly IOrderQueries _queries;

        public OrdersController(OrderService svc, IOrderQueries queries)
        {
            _svc = svc;
            _queries = queries;
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest req, CancellationToken ct)
        {
            var orderId = await _svc.CreateOrder(req.CustomerId, ct);
            return CreatedAtAction(nameof(GetById), new { orderId }, new CreateOrderResponse(orderId));
        }

        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddItem(Guid orderId, [FromBody] AddOrderItemRequest req, CancellationToken ct)
        {
            await _svc.AddItem(orderId, req.MenuItemId, req.Quantity, ct);
            return NoContent();
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderDetailsDto>> GetById(Guid orderId, CancellationToken ct)
        {
            var result = await _queries.GetOrderAsync(orderId, ct);
            return Ok(result);
        }

        [HttpPost("{orderId:guid}/pay")]
        public async Task<IActionResult> Pay(Guid orderId, [FromBody] PayOrderRequest req, CancellationToken ct)
        {
            await _svc.Pay(orderId, req.PaymentReference, ct);
            return NoContent();
        }

    }

}
