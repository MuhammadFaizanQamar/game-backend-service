using GameBackend.Core.Entities;

namespace GameBackend.Core.Interfaces;

public interface ISessionRepository
{
    Task<GameSession> CreateAsync(GameSession session);
    Task<GameSession?> GetActiveSessionAsync(Guid playerId, string gameId);
    Task<GameSession> UpdateAsync(GameSession session);
    Task<List<GameSession>> GetPlayerHistoryAsync(Guid playerId, string gameId, int limit);
    Task<PlayerSessionStats> GetPlayerStatsAsync(Guid playerId, string gameId);
}

public class PlayerSessionStats
{
    public int TotalGames { get; set; }
    public long BestScore { get; set; }
    public double AverageScore { get; set; }
    public DateTime? LastPlayedAt { get; set; }
}