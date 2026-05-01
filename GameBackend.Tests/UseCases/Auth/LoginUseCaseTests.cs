using FluentAssertions;
using GameBackend.Application.Contracts.Auth;
using GameBackend.Application.UseCases.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace GameBackend.Tests.UseCases.Auth;

public class LoginUseCaseTests
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _playerRepository = Substitute.For<IPlayerRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        _sut = new LoginUseCase(
            _playerRepository,
            _passwordHasher,
            _jwtTokenGenerator,
            _refreshTokenRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = "testplayer",
            Email = "test@game.com",
            PasswordHash = "hashed_password"
        };

        var request = new LoginRequest
        {
            Email = "test@game.com",
            Password = "Password123!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns(player);
        _passwordHasher.Verify(request.Password, player.PasswordHash).Returns(true);
        _jwtTokenGenerator.GenerateToken(player.Id, player.Username).Returns("access_token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh_token");

        // Act
        var result = await _sut.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PlayerId.Should().Be(player.Id);
        result.Username.Should().Be(player.Username);
        result.AccessToken.Should().Be("access_token");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@game.com",
            Password = "Password123!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns((Player?)null);

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = "testplayer",
            Email = "test@game.com",
            PasswordHash = "hashed_password"
        };

        var request = new LoginRequest
        {
            Email = "test@game.com",
            Password = "WrongPassword!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns(player);
        _passwordHasher.Verify(request.Password, player.PasswordHash).Returns(false);

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }
}