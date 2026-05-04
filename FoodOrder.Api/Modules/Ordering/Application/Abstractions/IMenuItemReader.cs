namespace FoodOrder.Api.Modules.Ordering.Application.Abstraction
{
    public interface IMenuItemReader
    {
        Task<MenuItemSnapshot?> GetActiveMenuItemAsync(Guid menuItemId, CancellationToken ct);
    }

    public sealed record MenuItemSnapshot(Guid MenuItemId, string Name, decimal PriceAmount);

}
