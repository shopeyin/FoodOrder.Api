namespace FoodOrder.Api.Domain.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; } = null!;

        private Customer() { } // EF

        public Customer(string name)
        {
            CustomerId = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
        }

        internal Customer(Guid id, string name)
        {
            CustomerId = id;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
        }
    }



}
