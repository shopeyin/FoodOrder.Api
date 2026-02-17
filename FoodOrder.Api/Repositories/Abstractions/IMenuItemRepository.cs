using FoodOrder.Api.Domain.Entities;

namespace FoodOrder.Api.Repositories.Abstractions
{
    public interface IMenuItemRepository
    {
        Task<MenuItem?> GetByIdAsync(Guid menuItemId, CancellationToken ct);
    }

}
