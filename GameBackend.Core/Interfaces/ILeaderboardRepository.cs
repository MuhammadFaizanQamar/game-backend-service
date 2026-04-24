using GameBackend.Core.Entities;

namespace GameBackend.Core.Interfaces;

public interface ILeaderboardRepository
{
    Task<Leaderboard?> GetByGameIdAndNameAsync(string gameId, string name);
    Task<Leaderboard> CreateAsync(Leaderboard leaderboard);
    Task<LeaderboardEntry?> GetEntryByPlayerAsync(Guid leaderboardId, Guid playerId);
    Task<LeaderboardEntry> AddEntryAsync(LeaderboardEntry entry);
    Task<LeaderboardEntry> UpdateEntryAsync(LeaderboardEntry entry);
    Task<List<LeaderboardEntry>> GetTopEntriesAsync(Guid leaderboardId, int limit);
    Task<int> GetPlayerRankAsync(Guid leaderboardId, Guid playerId);
    Task<List<LeaderboardEntry>> GetEntriesAroundPlayerAsync(Guid leaderboardId, Guid playerId, int range);
}