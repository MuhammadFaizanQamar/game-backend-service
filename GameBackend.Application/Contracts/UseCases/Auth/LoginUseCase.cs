using GameBackend.Application.Contracts.Auth;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUseCase(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        // 1. Find user
        var player = await _playerRepository.GetByEmailAsync(request.Email);
        if (player == null)
            throw new Exception("Invalid credentials");

        // 2. Verify password
        var isValid = _passwordHasher.Verify(request.Password, player.PasswordHash);
        if (!isValid)
            throw new Exception("Invalid credentials");

        // 3. Generate token
        var token = _jwtTokenGenerator.GenerateToken(player.Id, player.Username);

        // 4. Return response
        return new AuthResponse
        {
            PlayerId = player.Id,
            Username = player.Username,
            Token = token
        };
    }
}