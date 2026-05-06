using GameBackend.Application.Contracts.Auth;
using GameBackend.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
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

    /// <summary>Register a new player</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _registerUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>Login with existing credentials</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _loginUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    /// <summary>Refresh expired access token</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    /// <summary>Logout and revoke all refresh tokens</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var playerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                            ?? User.FindFirst("sub");
        if (playerIdClaim == null)
            return Unauthorized();

        await _logoutUseCase.ExecuteAsync(Guid.Parse(playerIdClaim.Value));
        return Ok(new { message = "Logged out successfully" });
    }
}