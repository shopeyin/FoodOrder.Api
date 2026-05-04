namespace FoodOrder.Api.Modules.Customers.Domain
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }

        private Customer() { } // EF

        public Customer(string name)
        {
            CustomerId = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required.") : name;
        }
    }

}
