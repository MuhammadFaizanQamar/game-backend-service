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
        return await _context.Players
            .FirstOrDefaultAsync(x => x.Email == email);
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
}