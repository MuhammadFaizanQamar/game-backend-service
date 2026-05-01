using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly GameDbContext _context;

    public RefreshTokenRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.Player)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllPlayerTokensAsync(Guid playerId)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.PlayerId == playerId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync();
    }
}