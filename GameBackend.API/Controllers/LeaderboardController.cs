using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Application.UseCases.Leaderboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/leaderboards")]
[Authorize]
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
        try
        {
            var response = await _submitScoreUseCase.ExecuteAsync(gameId, CurrentPlayerId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("{gameId}/top")]
    public async Task<IActionResult> GetTop(string gameId, [FromQuery] string name, [FromQuery] int limit = 10)
    {
        try
        {
            var response = await _getTopUseCase.ExecuteAsync(gameId, name, limit);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex, 404);
        }
    }

    [HttpGet("{gameId}/me")]
    public async Task<IActionResult> GetMyRank(string gameId, [FromQuery] string name)
    {
        try
        {
            var response = await _getPlayerRankUseCase.ExecuteAsync(gameId, name, CurrentPlayerId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex, 404);
        }
    }

    [HttpGet("{gameId}/around-me")]
    public async Task<IActionResult> GetAroundMe(string gameId, [FromQuery] string name, [FromQuery] int range = 5)
    {
        try
        {
            var leaderboard = await _getTopUseCase.ExecuteAsync(gameId, name, range);
            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            return HandleError(ex, 404);
        }
    }
}