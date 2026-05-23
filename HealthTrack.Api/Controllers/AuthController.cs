using HealthTrack.Api.Common;
using HealthTrack.Application.Identity.Auth.Commands;
using HealthTrack.Application.Identity.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/auth")]
[AllowAnonymous]
public class AuthController : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(
    [FromBody] RefreshTokenRequestDto request,
    CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new LogoutCommand(request.RefreshToken),
            cancellationToken);

        return NoContent();
    }
}