namespace FoodOrder.Api.Modules.Ordering.Application.Abstractions
{
    public interface ICustomerReader
    {
        Task<bool> ExistsAsync(Guid customerId, CancellationToken ct);
    }

}
