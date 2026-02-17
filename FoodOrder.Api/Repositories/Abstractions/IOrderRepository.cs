using FoodOrder.Api.Domain.Entities;

namespace FoodOrder.Api.Repositories.Abstractions
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
        Task<Order?> GetByIdWithItemsAsync(Guid orderId, CancellationToken ct);
        Task<bool> HasAnyItemsAsync(Guid orderId, CancellationToken ct);
        Task AddAsync(Order order, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }

}
