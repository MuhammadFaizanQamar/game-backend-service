namespace GameBackend.Core.Interfaces;

public interface ILeaderboardNotificationService
{
    Task NotifyScoreUpdatedAsync(string gameId, string leaderboardName, object score);
}