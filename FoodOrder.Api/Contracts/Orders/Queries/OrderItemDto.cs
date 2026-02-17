namespace FoodOrder.Api.Contracts.Orders.Queries
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
