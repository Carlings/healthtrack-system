using HealthTrack.Application.Common.Interfaces.Events;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.HealthRecords.Commands;
using HealthTrack.Domain.Entities;
using Moq;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class CreateHealthRecordCommandHandlerTests
{
    private Mock<IHealthRecordRepository> _records = null!;
    private Mock<ICurrentUserService> _currentUser = null!;
    private Mock<IHealthEventsPublisher> _eventsPublisher = null!;
    private CreateHealthRecordCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _records = new Mock<IHealthRecordRepository>();
        _currentUser = new Mock<ICurrentUserService>();
        _eventsPublisher = new Mock<IHealthEventsPublisher>();

        _currentUser.Setup(x => x.UserId).Returns(7);

        _handler = new CreateHealthRecordCommandHandler(
            _records.Object,
            _currentUser.Object,
            _eventsPublisher.Object);
    }

    [Test]
    public async Task Handle_WhenRecordCreated_ShouldPublishHealthRecordCreatedEvent()
    {
        var command = new CreateHealthRecordCommand(
            new DateTime(2026, 05, 23, 10, 30, 00, DateTimeKind.Utc),
            72.5f,
            115,
            36.8f,
            145,
            92,
            7000,
            7.2f);

        await _handler.Handle(command, CancellationToken.None);

        _records.Verify(x => x.AddAsync(
            It.Is<HealthRecord>(r =>
                r.UserId == 7 &&
                r.Pulse == 115 &&
                r.SystolicBP == 145 &&
                r.DiastolicBP == 92),
            It.IsAny<CancellationToken>()), Times.Once);

        _eventsPublisher.Verify(x => x.PublishHealthRecordCreatedAsync(
            7,
            command.RecordedAt,
            115,
            145,
            92,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
