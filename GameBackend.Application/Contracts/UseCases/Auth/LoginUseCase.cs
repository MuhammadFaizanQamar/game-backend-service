using GameBackend.Application.Contracts.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginUseCase(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        var player = await _playerRepository.GetByEmailAsync(request.Email);
        if (player == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var isValid = _passwordHasher.Verify(request.Password, player.PasswordHash);
        if (!isValid)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (player.IsBanned)
            throw new UnauthorizedAccessException("Your account has been banned");

        // Revoke all existing refresh tokens on new login
        await _refreshTokenRepository.RevokeAllPlayerTokensAsync(player.Id);

        var accessToken = _jwtTokenGenerator.GenerateToken(
            player.Id,
            player.Username,
            player.Role.ToString());
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            PlayerId = player.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        return new AuthResponse
        {
            PlayerId = player.Id,
            Username = player.Username,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}