using FluentAssertions;
using FoodOrder.Api.Data;
using FoodOrder.Api.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace FoodOrdering.Api.IntegrationTests.Orders;

#region Create Order

public sealed class CreateOrderTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _output;
    public CreateOrderTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;

        _client = factory.CreateClient();
        _output = output;
    }

    public async Task InitializeAsync()
    {
        _output.WriteLine("-----------RESET CALLED--------------");
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Print_Db_Connection_String()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderDbContext>();

        var connectionString = db.Database.GetDbConnection().ConnectionString;
        var databaseName = db.Database.GetDbConnection().Database;

        _output.WriteLine("=== EF CONNECTION STRING ===");
        _output.WriteLine(connectionString);
        _output.WriteLine($"Database: {databaseName}");
        _output.WriteLine("============================");

        Console.WriteLine("Pause here. Connect with SSMS, then press Enter.");
        Console.ReadLine();
    }

    [Fact]
    public async Task Post_orders_returns_201_created_and_order_id()
    {
        var response = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = TestDataSeeder.CustomerId
        });
        _output.WriteLine("POST ORDER CALLED");
        _output.WriteLine("=== SQL CONTAINER CONNECTION STRING ===");
        _output.WriteLine(_factory.GetConnectionString());
        _output.WriteLine("=======================================");
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        body.Should().NotBeNull();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderDbContext>();

        var order = await db.Orders.FindAsync(body!.OrderId);

        order.Should().NotBeNull();
        order!.CustomerId.Should().Be(TestDataSeeder.CustomerId);
    }

    private sealed record CreateOrderResponse(Guid OrderId);
}

#endregion

//#region Add Item

//public sealed class AddOrderItemTests : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly HttpClient _client;

//    public AddOrderItemTests(CustomWebApplicationFactory factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task Post_order_items_adds_item_to_order()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        var addItemResponse = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 2
//        });

//        addItemResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

//        var getResponse = await _client.GetAsync($"/orders/{created.OrderId}");
//        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();

//        order.Should().NotBeNull();
//        order!.Items.Should().HaveCount(1);
//        order.Items[0].Quantity.Should().Be(2);
//    }

//    private sealed record CreateOrderResponse(Guid OrderId);

//    private sealed record OrderResponse(Guid OrderId, List<OrderItemResponse> Items);

//    private sealed record OrderItemResponse(
//        Guid OrderItemId,
//        Guid MenuItemId,
//        string Name,
//        decimal UnitPrice,
//        int Quantity,
//        decimal LineTotal
//    );
//}

//#endregion

//#region Pay Order

//public sealed class PayOrderTests : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly HttpClient _client;

//    public PayOrderTests(CustomWebApplicationFactory factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task Post_pay_marks_order_as_paid()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 1
//        });

//        var payResponse = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
//        {
//            paymentReference = "TEST-123"
//        });

//        payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

//        var getResponse = await _client.GetAsync($"/orders/{created.OrderId}");
//        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();

//        order.Should().NotBeNull();
//        order!.Status.Should().Be("Paid");
//    }

//    private sealed record CreateOrderResponse(Guid OrderId);

//    private sealed record OrderResponse(Guid OrderId, string Status);
//}

//#endregion


//#region Negative - Add Item

//public sealed class AddOrderItemNegativeTests : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly HttpClient _client;

//    public AddOrderItemNegativeTests(CustomWebApplicationFactory factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task Post_order_items_returns_404_when_order_does_not_exist()
//    {
//        var missingOrderId = Guid.NewGuid();

//        var response = await _client.PostAsJsonAsync($"/orders/{missingOrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 2
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//    }

//    [Fact]
//    public async Task Post_order_items_returns_404_when_menu_item_does_not_exist()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = Guid.NewGuid(),
//            quantity = 2
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//    }

//    [Fact]
//    public async Task Post_order_items_returns_400_when_quantity_is_zero()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 0
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    }

//    private sealed record CreateOrderResponse(Guid OrderId);
//}

//#endregion

//#region Negative - Pay Order

//public sealed class PayOrderNegativeTests : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly HttpClient _client;

//    public PayOrderNegativeTests(CustomWebApplicationFactory factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task Post_pay_returns_400_when_order_has_no_items()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        var response = await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/pay", new
//        {
//            paymentReference = "TEST-EMPTY"
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    }

//    [Fact]
//    public async Task Post_pay_returns_400_when_order_is_already_paid()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 1
//        });

//        var firstPay = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
//        {
//            paymentReference = "TEST-FIRST"
//        });

//        firstPay.StatusCode.Should().Be(HttpStatusCode.NoContent);

//        var secondPay = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
//        {
//            paymentReference = "TEST-SECOND"
//        });

//        secondPay.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    }

//    [Fact]
//    public async Task Post_pay_returns_404_when_order_does_not_exist()
//    {
//        var response = await _client.PostAsJsonAsync($"/orders/{Guid.NewGuid()}/pay", new
//        {
//            paymentReference = "TEST-MISSING"
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//    }

//    [Fact]
//    public async Task Post_pay_returns_400_when_payment_reference_is_missing()
//    {
//        var createResponse = await _client.PostAsJsonAsync("/orders", new
//        {
//            customerId = TestDataSeeder.CustomerId
//        });

//        var created = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

//        await _client.PostAsJsonAsync($"/orders/{created!.OrderId}/items", new
//        {
//            menuItemId = TestDataSeeder.MenuItemId,
//            quantity = 1
//        });

//        var response = await _client.PostAsJsonAsync($"/orders/{created.OrderId}/pay", new
//        {
//            paymentReference = ""
//        });

//        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    }

//    private sealed record CreateOrderResponse(Guid OrderId);
//}

//#endregion

