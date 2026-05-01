using GameBackend.API.RateLimiting;
using GameBackend.Application.Contracts.Common;
using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Application.UseCases.Leaderboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/leaderboards")]
[Authorize]
[EnableRateLimiting(RateLimitingConfiguration.LeaderboardPolicy)]
public class LeaderboardController : PlayerControllerBase
{
    private readonly SubmitScoreUseCase _submitScoreUseCase;
    private readonly GetTopLeaderboardUseCase _getTopUseCase;
    private readonly GetPlayerRankUseCase _getPlayerRankUseCase;

    public LeaderboardController(
        SubmitScoreUseCase submitScoreUseCase,
        GetTopLeaderboardUseCase getTopUseCase,
        GetPlayerRankUseCase getPlayerRankUseCase)
    {
        _submitScoreUseCase = submitScoreUseCase;
        _getTopUseCase = getTopUseCase;
        _getPlayerRankUseCase = getPlayerRankUseCase;
    }

    /// <summary>Submit a score to a leaderboard</summary>
    /// <remarks>Auto-creates the leaderboard if it doesn't exist. Only updates if new score is higher than existing.</remarks>
    [HttpPost("{gameId}/scores")]
    public async Task<IActionResult> SubmitScore(string gameId, [FromBody] SubmitScoreRequest request)
    {
        var response = await _submitScoreUseCase.ExecuteAsync(gameId, CurrentPlayerId, request);
        return Ok(response);
    }

    /// <summary>Get your own rank on a leaderboard</summary>
    [HttpGet("{gameId}/me")]
    public async Task<IActionResult> GetMyRank(string gameId, [FromQuery] string name)
    {
        var response = await _getPlayerRankUseCase.ExecuteAsync(gameId, name, CurrentPlayerId);
        return Ok(response);
    }

    /// <summary>Get players ranked around you</summary>
    [HttpGet("{gameId}/around-me")]
    public async Task<IActionResult> GetAroundMe(
        string gameId,
        [FromQuery] string name,
        [FromQuery] int range = 5)
    {
        var pagination = new PaginationRequest { Page = 1, Limit = range * 2 + 1 };
        var response = await _getTopUseCase.ExecuteAsync(gameId, name, pagination);
        return Ok(response);
    }

    /// <summary>Get top players on a leaderboard</summary>
    /// <remarks>Results are cached for 60 seconds in Redis</remarks>
    [HttpGet("{gameId}/top")]
    public async Task<IActionResult> GetTop(
        string gameId,
        [FromQuery] string name,
        [FromQuery] PaginationRequest pagination)
    {
        var response = await _getTopUseCase.ExecuteAsync(gameId, name, pagination);
        return Ok(response);
    }
}