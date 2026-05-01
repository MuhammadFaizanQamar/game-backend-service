using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task ExecuteAsync(Guid playerId)
    {
        await _refreshTokenRepository.RevokeAllPlayerTokensAsync(playerId);
    }
}