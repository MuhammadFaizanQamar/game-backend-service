using GameBackend.Core.Entities;

namespace GameBackend.Core.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByEmailAsync(string email);
    Task<Player?> GetByUsernameAsync(string username);
    Task AddAsync(Player player);
}