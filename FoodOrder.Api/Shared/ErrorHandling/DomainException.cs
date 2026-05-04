namespace FoodOrder.Api.Common.ErrorHandling
{
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

}
