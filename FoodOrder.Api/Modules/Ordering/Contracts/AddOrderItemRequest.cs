namespace FoodOrder.Api.Modules.Ordering.Contracts
{
    public sealed record AddOrderItemRequest(Guid MenuItemId, int Quantity);
}
