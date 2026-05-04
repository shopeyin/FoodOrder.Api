namespace FoodOrder.Api.Modules.Ordering.Contracts.Queries
{
    public sealed record OrderItemDto(
     Guid OrderItemId,
     Guid MenuItemId,
     string Name,
     decimal UnitPrice,
     int Quantity,
     decimal LineTotal
 );

}
