using GameBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["DefaultConnection"];
        services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(connectionString));

        var redisUrl = context.Configuration["Redis__Url"] ?? string.Empty;
        var redisToken = context.Configuration["Redis__Token"] ?? string.Empty;

        if (!string.IsNullOrEmpty(redisUrl) && !redisUrl.StartsWith("https://"))
        {
            var redisConfig = new ConfigurationOptions
            {
                EndPoints = { redisUrl },
                Password = redisToken,
                Ssl = true,
                AbortOnConnectFail = false
            };
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConfig));
        }
    })
    .Build();

host.Run();