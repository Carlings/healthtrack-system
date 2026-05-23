using HealthTrack.Api.Common;
using HealthTrack.Application.Health.Activities.Commands;
using HealthTrack.Application.Health.Activities.Queries;
using HealthTrack.Application.Health.Activities.Queries.GetPaged;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/activities")]
public sealed class ActivitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetActivitiesAsync(
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetActivitiesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedActivitiesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetPagedActivitiesQuery(page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetActivityByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetActivityByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetActivityTypesAsync(
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetActivityTypesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivityAsync(
        [FromBody] CreateActivityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateActivityAsync(
        int id,
        [FromBody] UpdateActivityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            command with { Id = id },
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteActivityAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new DeleteActivityCommand(id),
            cancellationToken);

        return NoContent();
    }
}
