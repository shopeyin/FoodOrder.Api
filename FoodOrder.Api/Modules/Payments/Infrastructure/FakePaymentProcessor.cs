using FoodOrder.Api.Modules.Payments.Application.Abstractions;

namespace FoodOrder.Api.Modules.Payments.Infrastructure
{
    public sealed class FakePaymentProcessor : IPaymentProcessor
    {
        public Task<PaymentResult> ProcessAsync(
            Guid orderId,
            decimal amount,
            string paymentReference,
            CancellationToken ct)
        {
            return Task.FromResult(
                PaymentResult.Success($"DEV-{Guid.NewGuid():N}")
            );
        }
    }

}
