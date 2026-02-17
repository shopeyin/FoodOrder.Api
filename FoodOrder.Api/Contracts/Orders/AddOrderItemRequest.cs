namespace FoodOrder.Api.Contracts.Orders
{
    public sealed record AddOrderItemRequest(Guid MenuItemId, int Quantity);
}
