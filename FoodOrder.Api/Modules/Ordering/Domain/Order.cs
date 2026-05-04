using FoodOrder.Api.Modules.Customers.Domain;
using FoodOrder.Api.Common.ValueObjects;

namespace FoodOrder.Api.Modules.Ordering.Domain
{
    public class Order
    {
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }

        public List<OrderItem> Items { get; private set; } = new();

        private Order() { } // EF

        public Order(Guid customerId)
        {
            OrderId = Guid.NewGuid();
            CustomerId = customerId;
            Status = OrderStatus.Created;
        }

        //public void AddItem(MenuItem menuItem, int quantity)
        //{
        //    EnsureEditable();
        //    Items.Add(new OrderItem(menuItem.MenuItemId, menuItem.Name, menuItem.Price, quantity));
        //}
        public void AddItem(Guid menuItemId, string nameSnapshot, Money unitPrice, int quantity)
        {
            EnsureEditable();

            Items.Add(new OrderItem(
                menuItemId,
                nameSnapshot,
                unitPrice,
                quantity));
        }

        public void UpdateItemQuantity(Guid orderItemId, int quantity)
        {
            EnsureEditable();
            var item = Items.Single(i => i.OrderItemId == orderItemId);
            item.SetQuantity(quantity);
        }

        public void RemoveItem(Guid orderItemId)
        {
            EnsureEditable();
            Items.RemoveAll(i => i.OrderItemId == orderItemId);
        }

        public Money Total =>
            Items.Aggregate(new Money(0m), (acc, i) => acc + i.LineTotal);

        public void MarkPaid()
        {
            if (Status != OrderStatus.Created) throw new InvalidOperationException("Only Created orders can be paid.");
            Status = OrderStatus.Paid;
        }

        public void MarkDelivered()
        {
            if (Status != OrderStatus.Paid) throw new InvalidOperationException("Only Paid orders can be delivered.");
            Status = OrderStatus.Delivered;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered) throw new InvalidOperationException("Delivered orders cannot be cancelled.");
            Status = OrderStatus.Cancelled;
        }

        private void EnsureEditable()
        {
            if (Status != OrderStatus.Created)
                throw new InvalidOperationException("Order cannot be modified unless Created.");
        }
    }


}
