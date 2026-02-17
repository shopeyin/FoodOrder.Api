namespace FoodOrder.Api.Domain.ValueObjects
{
    public sealed class Quantity
    {
        public int Value { get; private set; }

        private Quantity() { } // EF

        public Quantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            Value = value;
        }
    }

}
