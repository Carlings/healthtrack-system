using HealthTrack.Api.Common;
using HealthTrack.Application.Identity.Auth.Commands;
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
}