using HealthTrack.Api.Controllers;
using HealthTrack.Application.Health.HealthRecords.DTOs;
using HealthTrack.Application.Health.HealthRecords.Queries.GetPaged;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class HealthRecordsControllerTests
{
    private Mock<ISender> _sender = null!;
    private HealthRecordsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new HealthRecordsController();

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
    public async Task GetPagedHealthRecordsAsync_WhenCalled_ShouldSendQueryAndReturnOk()
    {
        var expected = new PagedHealthRecordsDto(
            Items:
            [
                new HealthRecordDto(
                    1,
                    new DateTime(2026, 05, 23, 10, 30, 00),
                    72.5f,
                    68,
                    36.6f,
                    120,
                    80,
                    7523,
                    7.5f)
            ],
            Page: 2,
            PageSize: 10,
            TotalCount: 31);

        _sender
            .Setup(x => x.Send(
                It.Is<GetPagedHealthRecordsQuery>(q =>
                    q.From == new DateTime(2026, 05, 01) &&
                    q.To == new DateTime(2026, 05, 23) &&
                    q.Page == 2 &&
                    q.PageSize == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var actionResult = await _controller.GetPagedHealthRecordsAsync(
            from: new DateTime(2026, 05, 01),
            to: new DateTime(2026, 05, 23),
            page: 2,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());

        var ok = (OkObjectResult)actionResult;
        Assert.That(ok.Value, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetPagedHealthRecordsAsync_WhenPageArgumentsOmitted_ShouldUseDefaults()
    {
        _sender
            .Setup(x => x.Send(
                It.Is<GetPagedHealthRecordsQuery>(q =>
                    q.From == null &&
                    q.To == null &&
                    q.Page == 1 &&
                    q.PageSize == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedHealthRecordsDto([], 1, 10, 0));

        await _controller.GetPagedHealthRecordsAsync(
            from: null,
            to: null,
            cancellationToken: CancellationToken.None);

        _sender.Verify(x => x.Send(
            It.Is<GetPagedHealthRecordsQuery>(q =>
                q.From == null &&
                q.To == null &&
                q.Page == 1 &&
                q.PageSize == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
