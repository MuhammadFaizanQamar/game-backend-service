using FluentAssertions;
using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Application.UseCases.Leaderboards;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace GameBackend.Tests.UseCases.Leaderboards;

public class SubmitScoreUseCaseTests
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICacheService _cacheService;
    private readonly SubmitScoreUseCase _sut;

    public SubmitScoreUseCaseTests()
    {
        _leaderboardRepository = Substitute.For<ILeaderboardRepository>();
        _playerRepository = Substitute.For<IPlayerRepository>();
        _cacheService = Substitute.For<ICacheService>();

        _sut = new SubmitScoreUseCase(
            _leaderboardRepository,
            _playerRepository,
            _cacheService);
    }

    [Fact]
    public async Task ExecuteAsync_NewPlayer_CreatesEntry()
    {
        // Arrange
        var gameId = "TopicStack";
        var playerId = Guid.NewGuid();
        var leaderboard = new Leaderboard
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            Name = "Global"
        };
        var player = new Player { Id = playerId, Username = "testplayer" };
        var request = new SubmitScoreRequest
        {
            Name = "Global",
            ScoreType = "xp",
            Score = 1000,
            ResetPeriod = ResetPeriod.None
        };

        _leaderboardRepository.GetByGameIdAndNameAsync(gameId, request.Name)
            .Returns(leaderboard);
        _leaderboardRepository.GetEntryByPlayerAsync(leaderboard.Id, playerId)
            .Returns((LeaderboardEntry?)null);
        _leaderboardRepository.AddEntryAsync(Arg.Any<LeaderboardEntry>())
            .Returns(x => x.Arg<LeaderboardEntry>());
        _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId)
            .Returns(1);
        _playerRepository.GetByIdAsync(playerId).Returns(player);

        // Act
        var result = await _sut.ExecuteAsync(gameId, playerId, request);

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(1000);
        result.Rank.Should().Be(1);
        await _leaderboardRepository.Received(1).AddEntryAsync(Arg.Any<LeaderboardEntry>());
    }

    [Fact]
    public async Task ExecuteAsync_LowerScore_DoesNotUpdateEntry()
    {
        // Arrange
        var gameId = "TopicStack";
        var playerId = Guid.NewGuid();
        var leaderboard = new Leaderboard { Id = Guid.NewGuid(), GameId = gameId, Name = "Global" };
        var existingEntry = new LeaderboardEntry
        {
            Id = Guid.NewGuid(),
            LeaderboardId = leaderboard.Id,
            PlayerId = playerId,
            Score = 2000
        };
        var player = new Player { Id = playerId, Username = "testplayer" };
        var request = new SubmitScoreRequest
        {
            Name = "Global",
            ScoreType = "xp",
            Score = 500,
            ResetPeriod = ResetPeriod.None
        };

        _leaderboardRepository.GetByGameIdAndNameAsync(gameId, request.Name)
            .Returns(leaderboard);
        _leaderboardRepository.GetEntryByPlayerAsync(leaderboard.Id, playerId)
            .Returns(existingEntry);
        _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId)
            .Returns(1);
        _playerRepository.GetByIdAsync(playerId).Returns(player);

        // Act
        var result = await _sut.ExecuteAsync(gameId, playerId, request);

        // Assert
        result.Score.Should().Be(2000);
        await _leaderboardRepository.DidNotReceive().UpdateEntryAsync(Arg.Any<LeaderboardEntry>());
    }

    [Fact]
    public async Task ExecuteAsync_HigherScore_UpdatesEntry()
    {
        // Arrange
        var gameId = "TopicStack";
        var playerId = Guid.NewGuid();
        var leaderboard = new Leaderboard { Id = Guid.NewGuid(), GameId = gameId, Name = "Global" };
        var existingEntry = new LeaderboardEntry
        {
            Id = Guid.NewGuid(),
            LeaderboardId = leaderboard.Id,
            PlayerId = playerId,
            Score = 500
        };
        var player = new Player { Id = playerId, Username = "testplayer" };
        var request = new SubmitScoreRequest
        {
            Name = "Global",
            ScoreType = "xp",
            Score = 2000,
            ResetPeriod = ResetPeriod.None
        };

        _leaderboardRepository.GetByGameIdAndNameAsync(gameId, request.Name)
            .Returns(leaderboard);
        _leaderboardRepository.GetEntryByPlayerAsync(leaderboard.Id, playerId)
            .Returns(existingEntry);
        _leaderboardRepository.UpdateEntryAsync(Arg.Any<LeaderboardEntry>())
            .Returns(x => x.Arg<LeaderboardEntry>());
        _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId)
            .Returns(1);
        _playerRepository.GetByIdAsync(playerId).Returns(player);

        // Act
        var result = await _sut.ExecuteAsync(gameId, playerId, request);

        // Assert
        result.Score.Should().Be(2000);
        await _leaderboardRepository.Received(1).UpdateEntryAsync(
            Arg.Is<LeaderboardEntry>(e => e.Score == 2000));
    }
}