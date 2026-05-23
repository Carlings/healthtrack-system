using HealthTrack.Api.Common;
using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Application.Identity.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers
{
    [Route("api/users")]
    public sealed class UsersController : BaseApiController
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUserAsync(
            [FromBody] UpdateCurrentUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ChangePasswordCommand command,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                command,
                cancellationToken);

            return NoContent();
        }
    }
}