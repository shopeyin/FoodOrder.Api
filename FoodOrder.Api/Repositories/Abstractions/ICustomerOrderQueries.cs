using FoodOrder.Api.Contracts.Customers.Queries;

namespace FoodOrder.Api.Repositories.Abstractions
{
    public interface ICustomerOrderQueries
    {
        Task<PagedCustomerOrdersDto> GetCustomerOrdersAsync(Guid customerId, int page, int pageSize, CancellationToken ct);
    }

}
