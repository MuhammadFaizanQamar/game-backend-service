using Microsoft.AspNetCore.SignalR;

namespace GameBackend.API.Hubs;

public class LeaderboardHub : Hub
{
    public async Task JoinLeaderboard(string gameId, string leaderboardName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{gameId}:{leaderboardName}");
    }

    public async Task LeaveLeaderboard(string gameId, string leaderboardName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{gameId}:{leaderboardName}");
    }
}