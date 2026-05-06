// using GameBackend.API.RateLimiting;
// using GameBackend.Application.Contracts.Auth;
// using GameBackend.Application.UseCases.Auth;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.RateLimiting;
//
// namespace GameBackend.API.Controllers;
//
// [ApiController]
// [Route("api/auth")]
// [EnableRateLimiting(RateLimitingConfiguration.AuthPolicy)]
// public class AuthController : PlayerControllerBase
// {
//     private readonly RegisterPlayerUseCase _registerUseCase;
//     private readonly LoginUseCase _loginUseCase;
//     private readonly RefreshTokenUseCase _refreshTokenUseCase;
//     private readonly LogoutUseCase _logoutUseCase;
//
//     public AuthController(
//         RegisterPlayerUseCase registerUseCase,
//         LoginUseCase loginUseCase,
//         RefreshTokenUseCase refreshTokenUseCase,
//         LogoutUseCase logoutUseCase)
//     {
//         _registerUseCase = registerUseCase;
//         _loginUseCase = loginUseCase;
//         _refreshTokenUseCase = refreshTokenUseCase;
//         _logoutUseCase = logoutUseCase;
//     }
//
//     /// <summary>Register a new player account</summary>
//     /// <remarks>Creates a new player and returns JWT access and refresh tokens</remarks>
//     [HttpPost("register")]
//     public async Task<IActionResult> Register([FromBody] RegisterRequest request)
//     {
//         var response = await _registerUseCase.ExecuteAsync(request);
//         return CreatedAtAction(nameof(Register), response);
//     }
//
//     /// <summary>Login with existing credentials</summary>
//     /// <remarks>Returns JWT access token (15 min) and refresh token (7 days)</remarks>
//     [HttpPost("login")]
//     public async Task<IActionResult> Login([FromBody] LoginRequest request)
//     {
//         var response = await _loginUseCase.ExecuteAsync(request);
//         return Ok(response);
//     }
//
//     /// <summary>Refresh an expired access token</summary>
//     /// <remarks>Exchange a valid refresh token for a new access token</remarks>
//     [HttpPost("refresh")]
//     public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
//     {
//         var response = await _refreshTokenUseCase.ExecuteAsync(request);
//         return Ok(response);
//     }
//
//     /// <summary>Logout and revoke all refresh tokens</summary>
//     [HttpPost("logout")]
//     [Authorize]
//     public async Task<IActionResult> Logout()
//     {
//         await _logoutUseCase.ExecuteAsync(CurrentPlayerId);
//         return Ok(new { message = "Logged out successfully" });
//     }
// }