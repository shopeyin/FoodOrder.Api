using FoodOrder.Api.Domain.ValueObjects;

namespace FoodOrder.Api.Domain.Entities
{
    public class MenuItem
    {
        public Guid MenuItemId { get; private set; }
        public Guid RestaurantId { get; private set; }

        public string Name { get; private set; } = null!;
        public decimal PriceAmount { get; private set; }

        public bool IsActive { get; private set; }

        private MenuItem() { } // EF

        public MenuItem(Guid restaurantId, string name, Money price)
        {
            MenuItemId = Guid.NewGuid();
            RestaurantId = restaurantId;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
            PriceAmount = price.Amount;
            IsActive = true;
        }

        internal MenuItem(Guid id, Guid restaurantId, string name, Money price)
        {
            MenuItemId = id;
            RestaurantId = restaurantId;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
            PriceAmount = price.Amount;
            IsActive = true;
        }

        public void ChangePrice(Money newPrice)
        {
            if (!IsActive) throw new InvalidOperationException("Cannot change price of an inactive menu item");
            PriceAmount = newPrice.Amount;
        }
        public Money Price => new Money(PriceAmount);

        public void Deactivate()
        {
            IsActive = false;
        }
    }



}
