using FoodOrder.Api.Contracts.Orders.Queries;

namespace FoodOrder.Api.Repositories.Abstractions
{
    public interface IOrderQueries
    {
        Task<OrderDetailsDto> GetOrderAsync(Guid orderId, CancellationToken ct);
    }

}
