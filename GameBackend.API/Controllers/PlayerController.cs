using GameBackend.Application.Contracts.Players;
using GameBackend.Application.UseCases.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/players")]
[Authorize]
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
        try
        {
            var response = await _getProfileUseCase.ExecuteAsync(CurrentPlayerId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var response = await _updateProfileUseCase.ExecuteAsync(CurrentPlayerId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(Guid id)
    {
        try
        {
            var response = await _getProfileUseCase.ExecuteAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleError(ex, 404);
        }
    }
}