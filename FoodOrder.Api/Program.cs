using FoodOrder.Api.Application.Customers;
using FoodOrder.Api.Application.Orders;
using FoodOrder.Api.Application.Restaurants;
using FoodOrder.Api.Common.Exceptions;
using FoodOrder.Api.Data;
using FoodOrder.Api.Repositories.Abstractions;
using FoodOrder.Api.Repositories.Efcore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
});
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
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("Applying database migrations. Attempt {Attempt}/{MaxRetries}", attempt, maxRetries);

                await db.Database.MigrateAsync();
                await SeedData.EnsureSeededAsync(db);

                logger.LogInformation("Database migration and seeding completed successfully.");
                break;
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    logger.LogError(ex, "Database migration failed after {MaxRetries} attempts.", maxRetries);
                    throw;
                }

                logger.LogWarning(ex,
                    "Database not ready yet. Attempt {Attempt}/{MaxRetries}. Retrying in {DelaySeconds} seconds...",
                    attempt,
                    maxRetries,
                    delay.TotalSeconds);

                await Task.Delay(delay);
            }
        }
    }
}

app.UseHttpLogging();
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }
