using HealthTrack.Api.Common;
using HealthTrack.Application.Health.HealthRecords.Commands;
using HealthTrack.Application.Health.HealthRecords.Queries;
using HealthTrack.Application.HealthRecords.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/records")]
public sealed class HealthRecordsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetHealthRecordsByTimeRangeAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetHealthRecordsQuery(from, to),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHealthRecordByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetHealthRecordByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHealthRecordAsync(
        [FromBody] CreateHealthRecordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateHealthRecordAsync(
        int id,
        [FromBody] UpdateHealthRecordCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            command with { Id = id },
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHealthRecordAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new DeleteHealthRecordCommand(id),
            cancellationToken);

        return NoContent();
    }
}