using FoodOrder.Api.Modules.Customers.Contracts.Queries;

namespace FoodOrder.Api.Modules.Ordering.Application.Abstractions
{
    public interface ICustomerOrderQueries
    {
        Task<PagedCustomerOrdersDto> GetCustomerOrdersAsync(Guid customerId, int page, int pageSize, CancellationToken ct);
    }

}
