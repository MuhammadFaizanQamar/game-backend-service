using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _context;

    public PlayerRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByEmailAsync(string email)
    {
        var player = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);

        if (player != null)
        {
            // Read raw value directly from DB
            var rawRole = await _context.Players
                .Where(x => x.Email == email)
                .Select(x => (int)x.Role)
                .FirstOrDefaultAsync();
            Console.WriteLine($"Raw role value from DB: {rawRole}");
        }

        return player;
    }

    public async Task<Player?> GetByUsernameAsync(string username)
    {
        return await _context.Players
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task AddAsync(Player player)
    {
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Player> Players, int TotalCount)> GetAllAsync(int skip, int limit)
    {
        var query = _context.Players.OrderBy(x => x.CreatedAt);
        var totalCount = await query.CountAsync();
        var players = await query.Skip(skip).Take(limit).ToListAsync();
        return (players, totalCount);
    }

    public async Task DeleteAsync(Guid id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }
    }
}