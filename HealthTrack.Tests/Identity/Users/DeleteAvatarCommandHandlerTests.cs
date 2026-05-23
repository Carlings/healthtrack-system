using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Common.Interfaces.Storage;
using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Users;

public sealed class DeleteAvatarCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<ICurrentUserService> _currentUser = null!;
    private Mock<IAvatarStorageService> _storage = null!;
    private Mock<ILogger<DeleteAvatarCommandHandler>> _logger = null!;

    private DeleteAvatarCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _currentUser = new Mock<ICurrentUserService>();
        _storage = new Mock<IAvatarStorageService>();
        _logger = new Mock<ILogger<DeleteAvatarCommandHandler>>();


        _handler = new DeleteAvatarCommandHandler(
            _repository.Object,
            _currentUser.Object,
            _storage.Object,
            _logger.Object);
    }

    [Test]
    public async Task Handle_WhenAvatarExists_ShouldRemoveIt()
    {
        var user = TestDataFactory.CreateUser(1, avatarUrl: "/avatars/avatar.jpg");
        _currentUser.Setup(x => x.UserId).Returns(user.Id);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _storage
            .Setup(x => x.DeleteAsync("/avatars/avatar.jpg", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        Assert.That(user.AvatarUrl, Is.Null);
        _storage.Verify(x => x.DeleteAsync("/avatars/avatar.jpg", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenAvatarDoesNotExist_ShouldDoNothing()
    {
        var user = TestDataFactory.CreateUser(1, avatarUrl: null);
        _currentUser.Setup(x => x.UserId).Returns(user.Id);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        _repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _storage.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}