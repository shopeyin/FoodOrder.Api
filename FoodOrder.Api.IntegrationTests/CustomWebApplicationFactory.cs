using FoodOrder.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace FoodOrder.Api.IntegrationTests;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbSqlContainer = new MsSqlBuilder().Build();
    private readonly string _databaseName = "FoodOrderIntegrationTests";

    private string TestDatabaseConnectionString
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(_dbSqlContainer.GetConnectionString())
            {
                InitialCatalog = _databaseName
            };

            return builder.ConnectionString;
        }
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<FoodOrderDbContext>));

            services.AddDbContext<FoodOrderDbContext>(options =>
            {
                options.UseSqlServer(TestDatabaseConnectionString);
            });
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
    }

    public string GetConnectionString()
    {
        return TestDatabaseConnectionString;
    }

    public async Task InitializeAsync()
    {
        await _dbSqlContainer.StartAsync();

    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbSqlContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory>>();


        logger.LogInformation("Resetting test database...");
       // Console.WriteLine($"EF Database Name: {db.Database.GetDbConnection().Database}");

        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        TestDataSeeder.Seed(db);

        logger.LogInformation("Test database reset complete.");
    }
}
