using GameBackend.API;
using GameBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace GameBackend.Tests.Integration;

public class GameBackendWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("gamebackend_test")
        .WithUsername("testuser")
        .WithPassword("testpass_local_only")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ServiceBus:ConnectionString"] = null,
                // Disable YARP routing in tests — Auth endpoints handled locally
                ["ReverseProxy:Routes:auth-route:Match:Path"] = null,
                ["ReverseProxy:Clusters:auth-cluster:Destinations:auth-service:Address"] = null
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add test DbContext
            services.AddDbContext<GameDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));

            // Build service provider and migrate
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            db.Database.Migrate();
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}