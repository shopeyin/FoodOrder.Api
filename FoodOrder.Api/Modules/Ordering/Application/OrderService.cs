using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Common.ValueObjects;
using FoodOrder.Api.Modules.Ordering.Application.Abstraction;
using FoodOrder.Api.Modules.Ordering.Application.Abstractions;
using FoodOrder.Api.Modules.Ordering.Domain;
using FoodOrder.Api.Modules.Ordering.Infrastructure.Abstraction;
using FoodOrder.Api.Modules.Payments.Application.Abstractions;


namespace FoodOrder.Api.Modules.Ordering.Application
{
    public sealed class OrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ICustomerReader _customers;
        private readonly IMenuItemReader _menuItems;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IMenuItemReader menuItems, IOrderRepository orders, ICustomerReader customers, IPaymentProcessor paymentProcessor, ILogger<OrderService> logger)
        {

            _orders = orders;
            _customers = customers;
            _menuItems = menuItems;
            _logger = logger;
            _paymentProcessor = paymentProcessor;

        }


        public async Task<Guid> CreateOrder(Guid customerId, CancellationToken ct)
        {
            var customerExists = await _customers.ExistsAsync(customerId, ct);
            if (!customerExists) throw new KeyNotFoundException("Customer not found.");

            var order = new Order(customerId);

            await _orders.AddAsync(order, ct);
            await _orders.SaveChangesAsync(ct);
            _logger.LogInformation("Created order {OrderId} for customer {CustomerId}", order.OrderId, customerId);

            return order.OrderId;
        }

        public async Task AddItem(Guid orderId, Guid menuItemId, int quantity, CancellationToken ct)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            var order = await _orders.GetByIdWithItemsAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException("Order not found.");

            var menuItem = await _menuItems.GetActiveMenuItemAsync(menuItemId, ct);

            if (menuItem is null)
                throw new KeyNotFoundException("Menu item not found.");


            try
            {
                order.AddItem(menuItem.MenuItemId, menuItem.Name, new Money(menuItem.PriceAmount), quantity);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                throw new DomainException(ex.Message);
            }

            await _orders.SaveChangesAsync(ct);
            _logger.LogInformation(
                "Added {Quantity} item(s) of menu item {MenuItemId} to order {OrderId}",
                quantity,
                menuItemId,
                orderId);
        }





        public async Task Pay(Guid orderId, string paymentReference, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(paymentReference))
                throw new DomainException("PaymentReference is required.");

            var order = await _orders.GetByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException("Order not found.");

            var hasItems = await _orders.HasAnyItemsAsync(orderId, ct);
            if (!hasItems)
                throw new DomainException("Cannot pay for an order with no items.");

            var paymentResult = await _paymentProcessor.ProcessAsync(
                order.OrderId,
                order.Total.Amount,
                paymentReference,
                ct);

            if (!paymentResult.IsSuccess)
                throw new DomainException(paymentResult.ErrorMessage ?? "Payment failed.");

            try
            {
                order.MarkPaid();
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainException(ex.Message);
            }

            await _orders.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Order {OrderId} marked as paid with reference {PaymentReference} and transaction {TransactionId}",
                orderId,
                paymentReference,
                paymentResult.TransactionId);
        }






    }


}
