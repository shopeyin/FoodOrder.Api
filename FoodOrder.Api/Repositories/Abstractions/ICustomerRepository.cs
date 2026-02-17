namespace FoodOrder.Api.Repositories.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsAsync(Guid customerId, CancellationToken ct);
    }

}
