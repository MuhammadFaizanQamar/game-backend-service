using GameBackend.Application.Contracts.Auth;
using GameBackend.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : PlayerControllerBase
{
    private readonly RegisterPlayerUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;

    public AuthController(
        RegisterPlayerUseCase registerUseCase,
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _registerUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _loginUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _logoutUseCase.ExecuteAsync(CurrentPlayerId);
        return Ok(new { message = "Logged out successfully" });
    }
}