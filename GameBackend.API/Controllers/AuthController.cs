using GameBackend.Application.Contracts.Auth;
using GameBackend.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterPlayerUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;

    public AuthController(RegisterPlayerUseCase registerUseCase, LoginUseCase loginUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _registerUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Register), response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}