namespace FoodOrder.Api.Modules.Customers.Contracts.Queries
{
    public sealed record PagedCustomerOrdersDto(
     Guid CustomerId,
     int Page,
     int PageSize,
     int TotalCount,
     IReadOnlyList<CustomerOrderSummaryDto> Orders
 );

}
