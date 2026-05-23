using HealthTrack.Api.Common;
using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Application.Identity.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/users")]
public sealed class UsersController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUserAsync(
        [FromBody] UpdateCurrentUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatarAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await Mediator.Send(
            new UploadAvatarCommand(
                stream,
                file.FileName,
                file.Length),
            cancellationToken);

        return Ok(new
        {
            avatarUrl = result
        });
    }

    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatarAsync(CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAvatarCommand(), cancellationToken);
        return NoContent();
    }
}