namespace FoodOrder.Api.Modules.Payments.Application.Abstractions
{
    public interface IPaymentProcessor
    {
        Task<PaymentResult> ProcessAsync(Guid orderId, decimal amount, string paymentReference,
            CancellationToken ct);
    }

    public sealed record PaymentResult(
        bool IsSuccess,
        string? TransactionId,
        string? ErrorMessage)
    {
        public static PaymentResult Success(string transactionId)
            => new(true, transactionId, null);

        public static PaymentResult Failed(string errorMessage)
            => new(false, null, errorMessage);
    }

}
