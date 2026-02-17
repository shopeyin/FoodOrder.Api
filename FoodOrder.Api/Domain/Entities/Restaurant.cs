namespace FoodOrder.Api.Domain.Entities
{
    public class Restaurant
    {
        public Guid RestaurantId { get; private set; }
        public string Name { get; private set; } = null!;

        private Restaurant() { } // EF

        public Restaurant(string name)
        {
            RestaurantId = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
        }

        internal Restaurant(Guid id, string name)
        {
            RestaurantId = id;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
        }
    }



}
