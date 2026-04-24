using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Infrastructure.Repositories;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly GameDbContext _context;

    public LeaderboardRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Leaderboard?> GetByGameIdAndNameAsync(string gameId, string name)
    {
        return await _context.Leaderboards
            .FirstOrDefaultAsync(x => x.GameId == gameId && x.Name == name);
    }

    public async Task<Leaderboard> CreateAsync(Leaderboard leaderboard)
    {
        await _context.Leaderboards.AddAsync(leaderboard);
        await _context.SaveChangesAsync();
        return leaderboard;
    }

    public async Task<LeaderboardEntry?> GetEntryByPlayerAsync(Guid leaderboardId, Guid playerId)
    {
        return await _context.LeaderboardEntries
            .FirstOrDefaultAsync(x => x.LeaderboardId == leaderboardId && x.PlayerId == playerId);
    }

    public async Task<LeaderboardEntry> AddEntryAsync(LeaderboardEntry entry)
    {
        await _context.LeaderboardEntries.AddAsync(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<LeaderboardEntry> UpdateEntryAsync(LeaderboardEntry entry)
    {
        _context.LeaderboardEntries.Update(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<List<LeaderboardEntry>> GetTopEntriesAsync(Guid leaderboardId, int limit)
    {
        return await _context.LeaderboardEntries
            .Where(x => x.LeaderboardId == leaderboardId)
            .Include(x => x.Player)
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetPlayerRankAsync(Guid leaderboardId, Guid playerId)
    {
        var entry = await _context.LeaderboardEntries
            .FirstOrDefaultAsync(x => x.LeaderboardId == leaderboardId && x.PlayerId == playerId);

        if (entry == null) return -1;

        var rank = await _context.LeaderboardEntries
            .CountAsync(x => x.LeaderboardId == leaderboardId && x.Score > entry.Score);

        return rank + 1;
    }

    public async Task<List<LeaderboardEntry>> GetEntriesAroundPlayerAsync(
        Guid leaderboardId, Guid playerId, int range)
    {
        var entry = await _context.LeaderboardEntries
            .FirstOrDefaultAsync(x => x.LeaderboardId == leaderboardId && x.PlayerId == playerId);

        if (entry == null) return new List<LeaderboardEntry>();

        var rank = await GetPlayerRankAsync(leaderboardId, playerId);
        var skip = Math.Max(0, rank - range - 1);

        return await _context.LeaderboardEntries
            .Where(x => x.LeaderboardId == leaderboardId)
            .Include(x => x.Player)
            .OrderByDescending(x => x.Score)
            .Skip(skip)
            .Take(range * 2 + 1)
            .ToListAsync();
    }
}