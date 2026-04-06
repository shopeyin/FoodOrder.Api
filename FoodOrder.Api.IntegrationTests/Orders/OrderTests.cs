using FluentAssertions;
using FoodOrder.Api.IntegrationTests;
using System.Net;
using System.Net.Http.Json;

namespace FoodOrdering.Api.IntegrationTests.Orders;

#region Create Order

public sealed class CreateOrderTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateOrderTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_orders_returns_201_created_and_order_id()
    {
        var response = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        body.Should().NotBeNull();
        body!.OrderId.Should().NotBe(Guid.Empty);
    }

    private sealed record CreateOrderResponse(Guid OrderId);
}

#endregion

#region Add Item

public sealed class AddOrderItemTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AddOrderItemTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_order_items_adds_item_to_order()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        var addItemResponse = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 2
        });

        addItemResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/orders/{created.OrderId}");
        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();

        order.Should().NotBeNull();
        order!.Items.Should().HaveCount(1);
        order.Items[0].Quantity.Should().Be(2);
    }

    private sealed record CreateOrderResponse(Guid OrderId);

    private sealed record OrderResponse(Guid OrderId, List<OrderItemResponse> Items);

    private sealed record OrderItemResponse(
        Guid OrderItemId,
        Guid MenuItemId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
    );
}

#endregion

#region Pay Order

public sealed class PayOrderTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PayOrderTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_pay_marks_order_as_paid()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 1
        });

        var payResponse = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
        {
            paymentReference = "TEST-123"
        });

        payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/orders/{created.OrderId}");
        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();

        order.Should().NotBeNull();
        order!.Status.Should().Be("Paid");
    }

    private sealed record CreateOrderResponse(Guid OrderId);

    private sealed record OrderResponse(Guid OrderId, string Status);
}

#endregion


#region Negative - Add Item

public sealed class AddOrderItemNegativeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AddOrderItemNegativeTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_order_items_returns_404_when_order_does_not_exist()
    {
        var missingOrderId = Guid.NewGuid();

        var response = await _client.PostAsJsonAsync($"/orders/{missingOrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 2
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_order_items_returns_404_when_menu_item_does_not_exist()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = Guid.NewGuid(),
            quantity = 2
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_order_items_returns_400_when_quantity_is_zero()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 0
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record CreateOrderResponse(Guid OrderId);
}

#endregion

#region Negative - Pay Order

public sealed class PayOrderNegativeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PayOrderNegativeTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_pay_returns_400_when_order_has_no_items()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/pay", new
        {
            paymentReference = "TEST-EMPTY"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_pay_returns_400_when_order_is_already_paid()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 1
        });

        var firstPay = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
        {
            paymentReference = "TEST-FIRST"
        });

        firstPay.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var secondPay = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
        {
            paymentReference = "TEST-SECOND"
        });

        secondPay.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_pay_returns_404_when_order_does_not_exist()
    {
        var response = await _client.PostAsJsonAsync($"/orders/{Guid.NewGuid()}/pay", new
        {
            paymentReference = "TEST-MISSING"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_pay_returns_400_when_payment_reference_is_missing()
    {
        var createResponse = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
        {
            menuItemId = TestDataSeeder.MenuItemId,
            quantity = 1
        });

        var response = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
        {
            paymentReference = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record CreateOrderResponse(Guid OrderId);
}

#endregion

