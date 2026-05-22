using HealthTrack.Api.Common;
using HealthTrack.Application.Health.Goals.Commands;
using HealthTrack.Application.Health.Goals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/goals")]
public sealed class GoalsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetGoalsAsync(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetGoalsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoalAsync(
        [FromBody] CreateGoalCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGoalAsync(
        int id,
        [FromBody] UpdateGoalCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            command with { Id = id },
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGoalAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new DeleteGoalCommand(id),
            cancellationToken);

        return NoContent();
    }
}