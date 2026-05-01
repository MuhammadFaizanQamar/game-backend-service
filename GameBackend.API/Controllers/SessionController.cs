using GameBackend.API.RateLimiting;
using GameBackend.Application.Contracts.Sessions;
using GameBackend.Application.UseCases.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
[EnableRateLimiting(RateLimitingConfiguration.GeneralPolicy)]
public class SessionController : PlayerControllerBase
{
    private readonly StartSessionUseCase _startSessionUseCase;
    private readonly EndSessionUseCase _endSessionUseCase;
    private readonly GetSessionHistoryUseCase _getHistoryUseCase;
    private readonly GetSessionStatsUseCase _getStatsUseCase;

    public SessionController(
        StartSessionUseCase startSessionUseCase,
        EndSessionUseCase endSessionUseCase,
        GetSessionHistoryUseCase getHistoryUseCase,
        GetSessionStatsUseCase getStatsUseCase)
    {
        _startSessionUseCase = startSessionUseCase;
        _endSessionUseCase = endSessionUseCase;
        _getHistoryUseCase = getHistoryUseCase;
        _getStatsUseCase = getStatsUseCase;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request)
    {
        var response = await _startSessionUseCase.ExecuteAsync(CurrentPlayerId, request);
        return Ok(response);
    }

    [HttpPost("{gameId}/end")]
    public async Task<IActionResult> EndSession(string gameId, [FromBody] EndSessionRequest request)
    {
        var response = await _endSessionUseCase.ExecuteAsync(CurrentPlayerId, gameId, request);
        return Ok(response);
    }

    [HttpGet("{gameId}/history")]
    public async Task<IActionResult> GetHistory(string gameId, [FromQuery] int limit = 20)
    {
        var response = await _getHistoryUseCase.ExecuteAsync(CurrentPlayerId, gameId, limit);
        return Ok(response);
    }

    [HttpGet("{gameId}/stats")]
    public async Task<IActionResult> GetStats(string gameId)
    {
        var response = await _getStatsUseCase.ExecuteAsync(CurrentPlayerId, gameId);
        return Ok(response);
    }
}