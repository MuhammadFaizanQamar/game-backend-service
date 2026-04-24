using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.API.Controllers;

public abstract class PlayerControllerBase : ControllerBase
{
    protected Guid CurrentPlayerId
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("sub");

            if (claim == null)
                throw new UnauthorizedAccessException("Player ID not found in token");

            return Guid.Parse(claim.Value);
        }
    }

    protected string CurrentUsername
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.Name)
                        ?? User.FindFirst("unique_name");

            return claim?.Value ?? string.Empty;
        }
    }

    protected IActionResult HandleError(Exception ex, int statusCode = 400)
    {
        return statusCode switch
        {
            401 => Unauthorized(new { error = ex.Message }),
            404 => NotFound(new { error = ex.Message }),
            _ => BadRequest(new { error = ex.Message })
        };
    }
}