using FluentAssertions;
using GameBackend.Application.UseCases.Players;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace GameBackend.Tests.UseCases.Players;

public class GetPlayerProfileUseCaseTests
{
    private readonly IPlayerRepository _playerRepository;
    private readonly GetPlayerProfileUseCase _sut;

    public GetPlayerProfileUseCaseTests()
    {
        _playerRepository = Substitute.For<IPlayerRepository>();
        _sut = new GetPlayerProfileUseCase(_playerRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPlayerId_ReturnsProfile()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            Username = "testplayer",
            Email = "test@game.com",
            AvatarUrl = "",
            CreatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };

        _playerRepository.GetByIdAsync(playerId).Returns(player);
        _playerRepository.UpdateAsync(Arg.Any<Player>()).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ExecuteAsync(playerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(playerId);
        result.Username.Should().Be("testplayer");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPlayerId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        _playerRepository.GetByIdAsync(playerId).Returns((Player?)null);

        // Act
        var act = async () => await _sut.ExecuteAsync(playerId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Player not found");
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPlayerId_UpdatesLastSeenAt()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var beforeCall = DateTime.UtcNow;
        var player = new Player
        {
            Id = playerId,
            Username = "testplayer",
            Email = "test@game.com",
            LastSeenAt = DateTime.UtcNow.AddHours(-1)
        };

        _playerRepository.GetByIdAsync(playerId).Returns(player);
        _playerRepository.UpdateAsync(Arg.Any<Player>()).Returns(Task.CompletedTask);

        // Act
        await _sut.ExecuteAsync(playerId);

        // Assert
        await _playerRepository.Received(1).UpdateAsync(
            Arg.Is<Player>(p => p.LastSeenAt >= beforeCall));
    }
}