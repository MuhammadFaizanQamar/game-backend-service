using FluentAssertions;
using GameBackend.Application.Contracts.Auth;
using GameBackend.Application.UseCases.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace GameBackend.Tests.UseCases.Auth;

public class RegisterPlayerUseCaseTests
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly RegisterPlayerUseCase _sut;

    public RegisterPlayerUseCaseTests()
    {
        _playerRepository = Substitute.For<IPlayerRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        _sut = new RegisterPlayerUseCase(
            _playerRepository,
            _passwordHasher,
            _jwtTokenGenerator,
            _refreshTokenRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testplayer",
            Email = "test@game.com",
            Password = "Password123!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns((Player?)null);
        _passwordHasher.Hash(request.Password).Returns("hashed_password");
        _jwtTokenGenerator.GenerateToken(Arg.Any<Guid>(), request.Username).Returns("access_token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh_token");

        // Act
        var result = await _sut.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(request.Username);
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testplayer",
            Email = "existing@game.com",
            Password = "Password123!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns(new Player
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = "existing"
        });

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User already exists");
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_SavesPlayerToRepository()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testplayer",
            Email = "test@game.com",
            Password = "Password123!"
        };

        _playerRepository.GetByEmailAsync(request.Email).Returns((Player?)null);
        _passwordHasher.Hash(request.Password).Returns("hashed_password");
        _jwtTokenGenerator.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>()).Returns("token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh");

        // Act
        await _sut.ExecuteAsync(request);

        // Assert
        await _playerRepository.Received(1).AddAsync(Arg.Is<Player>(p =>
            p.Username == request.Username &&
            p.Email == request.Email &&
            p.PasswordHash == "hashed_password"));
    }
}