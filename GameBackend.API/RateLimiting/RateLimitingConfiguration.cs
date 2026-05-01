using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace GameBackend.API.RateLimiting;

public static class RateLimitingConfiguration
{
    public const string AuthPolicy = "auth";
    public const string GeneralPolicy = "general";
    public const string LeaderboardPolicy = "leaderboard";

    public static IServiceCollection AddGameBackendRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Strict limit for auth endpoints — prevent brute force
            options.AddFixedWindowLimiter(AuthPolicy, opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // General limit for most endpoints
            options.AddFixedWindowLimiter(GeneralPolicy, opt =>
            {
                opt.PermitLimit = 60;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // Leaderboard reads — slightly higher limit
            options.AddFixedWindowLimiter(LeaderboardPolicy, opt =>
            {
                opt.PermitLimit = 30;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // Return 429 Too Many Requests
            options.RejectionStatusCode = 429;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"error\":\"Too many requests. Please try again later.\",\"statusCode\":429}",
                    cancellationToken);
            };
        });

        return services;
    }
}