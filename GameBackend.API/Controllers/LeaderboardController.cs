using GameBackend.API.RateLimiting;
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

    [HttpPost("{gameId}/scores")]
    public async Task<IActionResult> SubmitScore(string gameId, [FromBody] SubmitScoreRequest request)
    {
        var response = await _submitScoreUseCase.ExecuteAsync(gameId, CurrentPlayerId, request);
        return Ok(response);
    }

    [HttpGet("{gameId}/top")]
    public async Task<IActionResult> GetTop(string gameId, [FromQuery] string name, [FromQuery] int limit = 10)
    {
        var response = await _getTopUseCase.ExecuteAsync(gameId, name, limit);
        return Ok(response);
    }

    [HttpGet("{gameId}/me")]
    public async Task<IActionResult> GetMyRank(string gameId, [FromQuery] string name)
    {
        var response = await _getPlayerRankUseCase.ExecuteAsync(gameId, name, CurrentPlayerId);
        return Ok(response);
    }

    [HttpGet("{gameId}/around-me")]
    public async Task<IActionResult> GetAroundMe(string gameId, [FromQuery] string name, [FromQuery] int range = 5)
    {
        var response = await _getTopUseCase.ExecuteAsync(gameId, name, range);
        return Ok(response);
    }
}