using FoodOrder.Api.Modules.Ordering.Contracts.Queries;

namespace FoodOrder.Api.Modules.Ordering.Infrastructure.Abstraction
{
    public interface IOrderQueries
    {
        Task<OrderDetailsDto> GetOrderAsync(Guid orderId, CancellationToken ct);
    }

}
