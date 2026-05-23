using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.Commands;
using HealthTrack.Domain.Entities;
using HealthTrack.Tests.Common;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Auth;

public sealed class LogoutCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private LogoutCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _handler = new LogoutCommandHandler(_repository.Object);
    }

    [Test]
    public async Task Handle_WhenTokenExists_ShouldRevokeIt()
    {
        var user = TestDataFactory.CreateUser();
        var token = TestDataFactory.CreateRefreshToken(user, token: "logout-token");

        _repository
            .Setup(x => x.GetRefreshTokenAsync("logout-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _repository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new LogoutCommand("logout-token");

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(token.IsRevoked, Is.True);
    }

    [Test]
    public async Task Handle_WhenTokenDoesNotExist_ShouldDoNothing()
    {
        _repository
            .Setup(x => x.GetRefreshTokenAsync("missing-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var command = new LogoutCommand("missing-token");

        await _handler.Handle(command, CancellationToken.None);

        _repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}