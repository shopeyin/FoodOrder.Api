namespace FoodOrder.Api.Common.ValueObjects
{
    public readonly struct Money
    {
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            Amount = amount;
        }

        public Money Multiply(int qty)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be > 0.");
            return new Money(Amount * qty);
        }

        public static Money operator +(Money a, Money b) => new Money(a.Amount + b.Amount);
    }


}
