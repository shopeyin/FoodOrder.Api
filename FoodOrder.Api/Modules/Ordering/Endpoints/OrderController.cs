using FoodOrder.Api.Data;
using FoodOrder.Api.Modules.Ordering.Application;
using FoodOrder.Api.Modules.Ordering.Contracts;
using FoodOrder.Api.Modules.Ordering.Contracts.Queries;
using FoodOrder.Api.Modules.Ordering.Infrastructure.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodOrder.Api.Modules.Ordering.Endpoints
{
    [ApiController]
    [Route("orders")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IOrderQueries _queries;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(OrderService orderService, IOrderQueries queries, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _queries = queries;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Creating order for customer {CustomerId}", req.CustomerId);
            var orderId = await _orderService.CreateOrder(req.CustomerId, ct);
            _logger.LogInformation("Created order {OrderId} for customer {CustomerId}", orderId, req.CustomerId);
            return CreatedAtAction(nameof(GetById), new { orderId }, new CreateOrderResponse(orderId));
        }

        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddItem(Guid orderId, [FromBody] AddOrderItemRequest req, CancellationToken ct)
        {
            _logger.LogInformation(
                "Adding item {MenuItemId} x {Quantity} to order {OrderId}",
                req.MenuItemId,
                req.Quantity,
                orderId);
            await _orderService.AddItem(orderId, req.MenuItemId, req.Quantity, ct);
            _logger.LogInformation("Added item {MenuItemId} to order {OrderId}", req.MenuItemId, orderId);
            return NoContent();
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderDetailsDto>> GetById(Guid orderId, CancellationToken ct)
        {
            _logger.LogInformation("Fetching order details for {OrderId}", orderId);
            var result = await _queries.GetOrderAsync(orderId, ct);
            _logger.LogInformation("Fetched order details for {OrderId}", orderId);
            return Ok(result);
        }

        [HttpPost("{orderId:guid}/pay")]
        public async Task<IActionResult> Pay(Guid orderId, [FromBody] PayOrderRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Paying order {OrderId}", orderId);
            await _orderService.Pay(orderId, req.PaymentReference, ct);
            _logger.LogInformation("Paid order {OrderId}", orderId);
            return NoContent();
        }

    }

}
