using FoodOrder.Api.Common.ErrorHandling;
using FoodOrder.Api.Data;
using FoodOrder.Api.Domain.Entities;
using FoodOrder.Api.Domain.ValueObjects;
using FoodOrder.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace FoodOrder.Api.Application.Orders
{
    public sealed class OrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ICustomerRepository _customers;
        private readonly IMenuItemRepository _menuItems;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IMenuItemRepository menuItems,
            IOrderRepository orders,
            ICustomerRepository customers,
            ILogger<OrderService> logger)
        {

            _orders = orders;
            _customers = customers;
            _menuItems = menuItems;
            _logger = logger;

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

            var menuItem = await _menuItems.GetByIdAsync(menuItemId, ct);
            if (menuItem is null)
                throw new KeyNotFoundException("Menu item not found.");

            if (!menuItem.IsActive)
                throw new DomainException("Menu item is inactive.");

            try
            {
                order.AddItem(menuItem, quantity);
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

            try
            {
                order.MarkPaid();
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainException(ex.Message);
            }

            await _orders.SaveChangesAsync(ct);
            _logger.LogInformation("Order {OrderId} marked as paid with reference {PaymentReference}", orderId, paymentReference);
        }



    }


}
