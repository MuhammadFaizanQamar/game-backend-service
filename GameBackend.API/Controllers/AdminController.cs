using GameBackend.Application.Contracts.Admin;
using GameBackend.Application.Contracts.Common;
using GameBackend.Application.UseCases.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : PlayerControllerBase
{
    private readonly GetAllPlayersUseCase _getAllPlayersUseCase;
    private readonly BanPlayerUseCase _banPlayerUseCase;
    private readonly UnbanPlayerUseCase _unbanPlayerUseCase;
    private readonly UpdatePlayerRoleUseCase _updatePlayerRoleUseCase;
    private readonly DeletePlayerUseCase _deletePlayerUseCase;

    public AdminController(
        GetAllPlayersUseCase getAllPlayersUseCase,
        BanPlayerUseCase banPlayerUseCase,
        UnbanPlayerUseCase unbanPlayerUseCase,
        UpdatePlayerRoleUseCase updatePlayerRoleUseCase,
        DeletePlayerUseCase deletePlayerUseCase)
    {
        _getAllPlayersUseCase = getAllPlayersUseCase;
        _banPlayerUseCase = banPlayerUseCase;
        _unbanPlayerUseCase = unbanPlayerUseCase;
        _updatePlayerRoleUseCase = updatePlayerRoleUseCase;
        _deletePlayerUseCase = deletePlayerUseCase;
    }

    /// <summary>Get all players (paginated)</summary>
    [HttpGet("players")]
    public async Task<IActionResult> GetAllPlayers([FromQuery] PaginationRequest pagination)
    {
        var response = await _getAllPlayersUseCase.ExecuteAsync(pagination);
        return Ok(response);
    }

    /// <summary>Ban a player — revokes all tokens immediately</summary>
    [HttpPost("players/{id}/ban")]
    public async Task<IActionResult> BanPlayer(Guid id)
    {
        await _banPlayerUseCase.ExecuteAsync(id);
        return Ok(new { message = "Player banned successfully" });
    }

    /// <summary>Unban a player</summary>
    [HttpPost("players/{id}/unban")]
    public async Task<IActionResult> UnbanPlayer(Guid id)
    {
        await _unbanPlayerUseCase.ExecuteAsync(id);
        return Ok(new { message = "Player unbanned successfully" });
    }

    /// <summary>Update player role (Player/Admin)</summary>
    [HttpPut("players/{id}/role")]
    public async Task<IActionResult> UpdatePlayerRole(Guid id, [FromBody] UpdatePlayerRoleRequest request)
    {
        await _updatePlayerRoleUseCase.ExecuteAsync(id, request);
        return Ok(new { message = "Player role updated successfully" });
    }

    /// <summary>Delete a player permanently</summary>
    [HttpDelete("players/{id}")]
    public async Task<IActionResult> DeletePlayer(Guid id)
    {
        await _deletePlayerUseCase.ExecuteAsync(id);
        return Ok(new { message = "Player deleted successfully" });
    }
}