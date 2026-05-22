using HealthTrack.Api.Common;
using HealthTrack.Application.Health.Notifications.Commands;
using HealthTrack.Application.Health.Notifications.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Controllers;

[Route("api/notifications")]
public sealed class NotificationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUnreadNotificationsAsync(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetUnreadNotificationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotificationsAsync(
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetNotificationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetNotificationByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetNotificationByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotificationAsync(
        [FromBody] CreateNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkNotificationAsReadAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new MarkNotificationAsReadCommand(id),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteNotificationAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new DeleteNotificationCommand(id),
            cancellationToken);

        return NoContent();
    }
}