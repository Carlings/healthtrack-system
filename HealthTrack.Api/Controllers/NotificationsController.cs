using System.Text.Json;
using HealthTrack.Api.Common;
using HealthTrack.Application.Health.Notifications.Commands;
using HealthTrack.Application.Health.Notifications.Queries;
using HealthTrack.Infrastructure.Realtime;
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

    [HttpGet("stream")]
    public async Task StreamNotificationsAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        var userId = UserId;

        var broker = HttpContext.RequestServices.GetRequiredService<INotificationRealtimeBroker>();
        var (subscriptionId, reader) = broker.Subscribe(userId);

        try
        {
            await Response.WriteAsync(": connected\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var readTask = reader.ReadAsync(cancellationToken).AsTask();
                var delayTask = Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
                var completed = await Task.WhenAny(readTask, delayTask);

                if (completed == delayTask)
                {
                    await Response.WriteAsync(": keepalive\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                    continue;
                }

                var message = await readTask;
                var payload = JsonSerializer.Serialize(message);

                await Response.WriteAsync($"event: {message.Event}\n", cancellationToken);
                await Response.WriteAsync($"data: {payload}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        finally
        {
            broker.Unsubscribe(userId, subscriptionId);
        }
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
