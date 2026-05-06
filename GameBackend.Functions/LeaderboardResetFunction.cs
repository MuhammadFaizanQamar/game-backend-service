using GameBackend.Core.Entities;
using GameBackend.Infrastructure.Persistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace GameBackend.Functions;

public class LeaderboardResetFunction
{
    private readonly GameDbContext _context;
    private readonly ILogger<LeaderboardResetFunction> _logger;
    private readonly IConnectionMultiplexer? _redis;

    public LeaderboardResetFunction(
        GameDbContext context,
        ILogger<LeaderboardResetFunction> logger,
        IConnectionMultiplexer? redis = null)
    {
        _context = context;
        _logger = logger;
        _redis = redis;
    }

    // Runs every Monday at 00:00 UTC
    [Function("LeaderboardWeeklyReset")]
    public async Task RunWeekly(
        [TimerTrigger("0 0 0 * * 1")] TimerInfo timerInfo)
    {
        _logger.LogInformation("Weekly leaderboard reset started at {Time}", DateTime.UtcNow);
        await ResetLeaderboardsByPeriod(ResetPeriod.Weekly);
    }

    // Runs every day at 00:00 UTC
    [Function("LeaderboardDailyReset")]
    public async Task RunDaily(
        [TimerTrigger("0 0 0 * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation("Daily leaderboard reset started at {Time}", DateTime.UtcNow);
        await ResetLeaderboardsByPeriod(ResetPeriod.Daily);
    }

    // Runs on 1st of every month at 00:00 UTC
    [Function("LeaderboardMonthlyReset")]
    public async Task RunMonthly(
        [TimerTrigger("0 0 0 1 * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation("Monthly leaderboard reset started at {Time}", DateTime.UtcNow);
        await ResetLeaderboardsByPeriod(ResetPeriod.Monthly);
    }

    private async Task ResetLeaderboardsByPeriod(ResetPeriod period)
    {
        var leaderboards = await _context.Leaderboards
            .Where(x => x.ResetPeriod == period)
            .ToListAsync();

        if (!leaderboards.Any())
        {
            _logger.LogInformation("No {Period} leaderboards found to reset", period);
            return;
        }

        foreach (var leaderboard in leaderboards)
        {
            var entriesDeleted = await _context.LeaderboardEntries
                .Where(x => x.LeaderboardId == leaderboard.Id)
                .ExecuteDeleteAsync();

            _logger.LogInformation(
                "Reset leaderboard {Name} ({GameId}) — deleted {Count} entries",
                leaderboard.Name, leaderboard.GameId, entriesDeleted);

            // Invalidate Redis cache
            if (_redis != null)
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync($"leaderboard:{leaderboard.Id}:top");
                _logger.LogInformation("Cache invalidated for leaderboard {Id}", leaderboard.Id);
            }
        }

        _logger.LogInformation(
            "{Period} reset complete — {Count} leaderboards reset",
            period, leaderboards.Count);
    }
}