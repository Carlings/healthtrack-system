using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Tests.Common;
using Moq;

namespace HealthTrack.Tests.Identity.Users;

public sealed class ChangePasswordCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<ICurrentUserService> _currentUser = null!;
    private Mock<IPasswordHasherService> _passwordHasher = null!;

    private ChangePasswordCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _currentUser = new Mock<ICurrentUserService>();
        _passwordHasher = new Mock<IPasswordHasherService>();

        _handler = new ChangePasswordCommandHandler(
            _repository.Object,
            _currentUser.Object,
            _passwordHasher.Object);
    }

    [Test]
    public async Task Handle_WhenCurrentPasswordIsValid_ShouldChangePasswordAndIncrementTokenVersion()
    {
        var user = TestDataFactory.CreateUser(tokenVersion: 3);

        _currentUser.Setup(x => x.UserId).Returns(user.Id);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.VerifyPassword(user, user.PasswordHash, "OldPassword123!"))
            .Returns(true);

        _passwordHasher
            .Setup(x => x.HashPassword(user, "NewPassword123!"))
            .Returns("new-hash");

        _repository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ChangePasswordCommand("OldPassword123!", "NewPassword123!");

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(user.PasswordHash, Is.EqualTo("new-hash"));
        Assert.That(user.TokenVersion, Is.EqualTo(4));
    }

    [Test]
    public void Handle_WhenCurrentPasswordIsInvalid_ShouldThrow()
    {
        var user = TestDataFactory.CreateUser();

        _currentUser.Setup(x => x.UserId).Returns(user.Id);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.VerifyPassword(user, user.PasswordHash, "WrongPassword!"))
            .Returns(false);

        var command = new ChangePasswordCommand("WrongPassword!", "NewPassword123!");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenUserNotFound_ShouldThrow()
    {
        _currentUser.Setup(x => x.UserId).Returns(999);

        _repository
            .Setup(x => x.GetTrackedByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HealthTrack.Domain.Entities.User?)null);

        var command = new ChangePasswordCommand("OldPassword123!", "NewPassword123!");

        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}