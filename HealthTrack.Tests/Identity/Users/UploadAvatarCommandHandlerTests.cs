using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Common.Interfaces.Storage;
using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Users;

public sealed class UploadAvatarCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<ICurrentUserService> _currentUser = null!;
    private Mock<IAvatarStorageService> _storage = null!;
    private Mock<ILogger<UploadAvatarCommandHandler>> _logger = null!;

    private UploadAvatarCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _currentUser = new Mock<ICurrentUserService>();
        _storage = new Mock<IAvatarStorageService>();
        _logger = new Mock<ILogger<UploadAvatarCommandHandler>>();

        _handler = new UploadAvatarCommandHandler(
            _repository.Object,
            _currentUser.Object,
            _storage.Object,
            _logger.Object);
    }

    [Test]
    public async Task Handle_WhenUserHasOldAvatar_ShouldReplaceIt()
    {
        var user = TestDataFactory.CreateUser(
            1,
            avatarUrl: "/avatars/old.jpg");

        _currentUser
            .Setup(x => x.UserId)
            .Returns(user.Id);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _storage
            .Setup(x => x.UploadAsync(
                It.IsAny<Stream>(),
                "new.jpg",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("/avatars/new.jpg");

        _storage
            .Setup(x => x.DeleteAsync(
                "/avatars/old.jpg",
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repository
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await using var stream =
            new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await _handler.Handle(
            new UploadAvatarCommand(
                stream,
                "new.jpg",
                stream.Length),
            CancellationToken.None);

        Assert.That(result, Is.EqualTo("/avatars/new.jpg"));

        Assert.That(
            user.AvatarUrl,
            Is.EqualTo("/avatars/new.jpg"));

        _storage.Verify(
            x => x.DeleteAsync(
                "/avatars/old.jpg",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}