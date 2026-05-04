namespace FoodOrder.Api.Modules.Ordering.Contracts.Queries
{
    public sealed record OrderDetailsDto(
     Guid OrderId,
     Guid CustomerId,
     string Status,
     decimal Total,
     IReadOnlyList<OrderItemDto> Items
 );

}
