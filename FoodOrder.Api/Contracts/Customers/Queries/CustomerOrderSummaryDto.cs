namespace FoodOrder.Api.Contracts.Customers.Queries
{
    public sealed record CustomerOrderSummaryDto(
    Guid OrderId,
    string Status,
    int ItemCount,
    decimal Total
);

}
