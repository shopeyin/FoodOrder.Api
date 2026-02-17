using FoodOrder.Api.Application.Customers;
using FoodOrder.Api.Application.Orders;
using FoodOrder.Api.Application.Restaurants;
using FoodOrder.Api.Common.Exceptions;
using FoodOrder.Api.Data;
using FoodOrder.Api.Repositories.Abstractions;
using FoodOrder.Api.Repositories.Efcore;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderQueries, EfOrderQueries>();
builder.Services.AddScoped<ICustomerOrderQueries, EfCustomerOrderQueries>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IMenuItemRepository, EfMenuItemRepository>();
builder.Services.AddScoped<ICustomerRepository, EfCustomerRepository>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddTransient<ExceptionMiddleware>();

builder.Services.AddDbContext<FoodOrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

    if (env.IsDevelopment())
    {
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderDbContext>();

        // Ensures DB + schema are created from migrations
        await db.Database.MigrateAsync();

        // Seeds only if empty
        await SeedData.EnsureSeededAsync(db);
    }
}


app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
