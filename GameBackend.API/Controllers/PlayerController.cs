using GameBackend.API.RateLimiting;
using GameBackend.Application.Contracts.Players;
using GameBackend.Application.UseCases.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/players")]
[Authorize]
[EnableRateLimiting(RateLimitingConfiguration.GeneralPolicy)]
public class PlayerController : PlayerControllerBase
{
    private readonly GetPlayerProfileUseCase _getProfileUseCase;
    private readonly UpdatePlayerProfileUseCase _updateProfileUseCase;

    public PlayerController(
        GetPlayerProfileUseCase getProfileUseCase,
        UpdatePlayerProfileUseCase updateProfileUseCase)
    {
        _getProfileUseCase = getProfileUseCase;
        _updateProfileUseCase = updateProfileUseCase;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var response = await _getProfileUseCase.ExecuteAsync(CurrentPlayerId);
        return Ok(response);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var response = await _updateProfileUseCase.ExecuteAsync(CurrentPlayerId, request);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(Guid id)
    {
        var response = await _getProfileUseCase.ExecuteAsync(id);
        return Ok(response);
    }
}