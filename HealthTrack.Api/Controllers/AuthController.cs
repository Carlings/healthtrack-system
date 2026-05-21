using HealthTrack.Api.Common;
using HealthTrack.Application.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/auth")]
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