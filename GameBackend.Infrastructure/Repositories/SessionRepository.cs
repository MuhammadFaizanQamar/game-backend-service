using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly GameDbContext _context;

    public SessionRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<GameSession> CreateAsync(GameSession session)
    {
        await _context.GameSessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<GameSession?> GetActiveSessionAsync(Guid playerId, string gameId)
    {
        return await _context.GameSessions
            .FirstOrDefaultAsync(x =>
                x.PlayerId == playerId &&
                x.GameId == gameId &&
                x.Status == SessionStatus.Active);
    }

    public async Task<GameSession> UpdateAsync(GameSession session)
    {
        _context.GameSessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<List<GameSession>> GetPlayerHistoryAsync(Guid playerId, string gameId, int limit)
    {
        return await _context.GameSessions
            .Where(x => x.PlayerId == playerId &&
                        x.GameId == gameId &&
                        x.Status == SessionStatus.Completed)
            .OrderByDescending(x => x.StartedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<PlayerSessionStats> GetPlayerStatsAsync(Guid playerId, string gameId)
    {
        var sessions = await _context.GameSessions
            .Where(x => x.PlayerId == playerId &&
                        x.GameId == gameId &&
                        x.Status == SessionStatus.Completed)
            .ToListAsync();

        if (!sessions.Any())
            return new PlayerSessionStats();

        return new PlayerSessionStats
        {
            TotalGames = sessions.Count,
            BestScore = sessions.Max(x => x.Score),
            AverageScore = sessions.Average(x => x.Score),
            LastPlayedAt = sessions.Max(x => x.EndedAt)
        };
    }
}