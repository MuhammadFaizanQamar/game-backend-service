using GameBackend.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GameBackend.API.Hubs;

public class LeaderboardNotificationService : ILeaderboardNotificationService
{
    private readonly IHubContext<LeaderboardHub> _hubContext;

    public LeaderboardNotificationService(IHubContext<LeaderboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyScoreUpdatedAsync(string gameId, string leaderboardName, object score)
    {
        await _hubContext.Clients
            .Group($"{gameId}:{leaderboardName}")
            .SendAsync("ScoreUpdated", score);
    }
}