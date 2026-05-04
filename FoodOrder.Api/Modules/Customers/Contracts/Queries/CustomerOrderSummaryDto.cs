namespace FoodOrder.Api.Modules.Customers.Contracts.Queries
{
    public sealed record CustomerOrderSummaryDto(
    Guid OrderId,
    string Status,
    int ItemCount,
    decimal Total
);

}
