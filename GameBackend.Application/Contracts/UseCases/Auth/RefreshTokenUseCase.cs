using GameBackend.Application.Contracts.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class RefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IPlayerRepository playerRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _playerRepository = playerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> ExecuteAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (refreshToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token has been revoked");

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired");

        var player = await _playerRepository.GetByIdAsync(refreshToken.PlayerId);
        if (player == null)
            throw new UnauthorizedAccessException("Player not found");

        // Revoke old token
        refreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        // Generate new tokens
        var newAccessToken = _jwtTokenGenerator.GenerateToken(
            player.Id,
            player.Username,
            player.Role.ToString());
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            PlayerId = player.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        return new AuthResponse
        {
            PlayerId = player.Id,
            Username = player.Username,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}