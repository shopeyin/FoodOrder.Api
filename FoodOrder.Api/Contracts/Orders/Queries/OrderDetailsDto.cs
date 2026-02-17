namespace FoodOrder.Api.Contracts.Orders.Queries
{
    public sealed record OrderDetailsDto(
     Guid OrderId,
     Guid CustomerId,
     string Status,
     decimal Total,
     IReadOnlyList<OrderItemDto> Items
 );

}
