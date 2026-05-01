using GameBackend.Core.Entities;

namespace GameBackend.Core.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken token);
    Task RevokeAllPlayerTokensAsync(Guid playerId);
}