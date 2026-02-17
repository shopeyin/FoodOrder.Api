using FoodOrder.Api.Domain.ValueObjects;

namespace FoodOrder.Api.Domain.Entities
{
    public class OrderItem
    {
        public Guid OrderItemId { get; private set; }
        public Guid OrderId { get; private set; }

        public Guid MenuItemId { get; private set; }
        public string NameSnapshot { get; private set; } = null!;
        public decimal UnitPriceAmount { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { } // EF

        internal OrderItem(Guid menuItemId, string nameSnapshot, Money unitPrice, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0.");

            OrderItemId = Guid.NewGuid();
            MenuItemId = menuItemId;
            NameSnapshot = nameSnapshot;
            UnitPriceAmount = unitPrice.Amount;
            Quantity = quantity;
        }

        public Money UnitPrice => new Money(UnitPriceAmount);
        public Money LineTotal => new Money(UnitPriceAmount).Multiply(Quantity);

        internal void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0.");
            Quantity = quantity;
        }
    }



}
