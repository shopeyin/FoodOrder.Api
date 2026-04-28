using FoodOrder.Api.Domain.ValueObjects;

namespace FoodOrder.Api.Domain.Entities
{
    public class Restaurant
    {
        public Guid RestaurantId { get; private set; }
        public string Name { get; private set; } = null!;

        private readonly List<MenuItem> _menuItems = new();
        public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

        private Restaurant() { } // EF

        public Restaurant(string name)
        {
            RestaurantId = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("Name required.")
                : name.Trim();
        }

        public MenuItem AddMenuItem(string name, Money price)
        {
            if (_menuItems.Any(x => x.Name == name && x.IsActive))
                throw new InvalidOperationException("An active menu item with this name already exists.");

            var menuItem = new MenuItem(RestaurantId, name, price);
            _menuItems.Add(menuItem);

            return menuItem;
        }

        public void ChangeMenuItemPrice(Guid menuItemId, Money newPrice)
        {
            var item = _menuItems.SingleOrDefault(x => x.MenuItemId == menuItemId)
                ?? throw new InvalidOperationException("Menu item not found.");

            item.ChangePrice(newPrice);
        }

        public void DeactivateMenuItem(Guid menuItemId)
        {
            var item = _menuItems.SingleOrDefault(x => x.MenuItemId == menuItemId)
                ?? throw new InvalidOperationException("Menu item not found.");

            item.Deactivate();
        }
    }





}
