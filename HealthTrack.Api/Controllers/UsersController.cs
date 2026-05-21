using HealthTrack.Api.Common;
using HealthTrack.Application.Users.Queries;
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
    }
}