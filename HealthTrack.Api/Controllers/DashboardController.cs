using HealthTrack.Api.Common;
using HealthTrack.Application.Health.Dashboard.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/dashboard")]
public sealed class DashboardController : BaseApiController
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverviewAsync(
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetDashboardOverviewQuery(),
            cancellationToken);

        return Ok(result);
    }
}