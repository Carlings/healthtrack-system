using HealthTrack.Api.Controllers;
using HealthTrack.Application.Health.Activities.DTOs;
using HealthTrack.Application.Health.Activities.Queries.GetPaged;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HealthTrack.Tests.Health.Activities;

public sealed class ActivitiesControllerTests
{
    private Mock<ISender> _sender = null!;
    private ActivitiesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new ActivitiesController();

        var services = new ServiceCollection()
            .AddSingleton(_sender.Object)
            .BuildServiceProvider();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = services
            }
        };
    }

    [Test]
    public async Task GetPagedActivitiesAsync_WhenCalled_ShouldSendQueryAndReturnOk()
    {
        var expected = new PagedActivitiesDto(
            Items:
            [
                new UserActivityDto(
                    1,
                    2,
                    "Running",
                    45,
                    450,
                    new DateTime(2026, 05, 23, 10, 30, 00))
            ],
            Page: 2,
            PageSize: 10,
            TotalCount: 31);

        _sender
            .Setup(x => x.Send(
                It.Is<GetPagedActivitiesQuery>(q =>
                    q.Page == 2 &&
                    q.PageSize == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var actionResult = await _controller.GetPagedActivitiesAsync(
            page: 2,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());

        var ok = (OkObjectResult)actionResult;
        Assert.That(ok.Value, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetPagedActivitiesAsync_WhenPageArgumentsOmitted_ShouldUseDefaults()
    {
        _sender
            .Setup(x => x.Send(
                It.Is<GetPagedActivitiesQuery>(q =>
                    q.Page == 1 &&
                    q.PageSize == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedActivitiesDto([], 1, 10, 0));

        await _controller.GetPagedActivitiesAsync(cancellationToken: CancellationToken.None);

        _sender.Verify(x => x.Send(
            It.Is<GetPagedActivitiesQuery>(q =>
                q.Page == 1 &&
                q.PageSize == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
