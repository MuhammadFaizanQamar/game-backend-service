using GameBackend.Core.Entities;

namespace GameBackend.Core.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByEmailAsync(string email);
    Task<Player?> GetByUsernameAsync(string username);
    Task<Player?> GetByIdAsync(Guid id);
    Task AddAsync(Player player);
    Task UpdateAsync(Player player);
    Task<(List<Player> Players, int TotalCount)> GetAllAsync(int skip, int limit);
    Task DeleteAsync(Guid id);
}