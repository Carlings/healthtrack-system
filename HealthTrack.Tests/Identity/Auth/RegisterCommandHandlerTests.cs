using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.Commands;
using HealthTrack.Domain.Entities;
using HealthTrack.Tests.Common;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Auth;

public sealed class RegisterCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<IJwtTokenService> _jwtTokenService = null!;
    private Mock<IPasswordHasherService> _passwordHasherService = null!;
    private Mock<IRefreshTokenService> _refreshTokenService = null!;

    private RegisterCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _passwordHasherService = new Mock<IPasswordHasherService>();
        _refreshTokenService = new Mock<IRefreshTokenService>();

        _handler = new RegisterCommandHandler(
            _repository.Object,
            _jwtTokenService.Object,
            _passwordHasherService.Object,
            _refreshTokenService.Object);
    }

    [Test]
    public async Task Handle_WhenEmailIsFree_ShouldCreateUserAndReturnTokens()
    {
        User? addedUser = null;
        RefreshToken? addedRefreshToken = null;

        _repository
            .Setup(x => x.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _passwordHasherService
            .Setup(x => x.HashPassword(It.IsAny<User>(), "Password123!"))
            .Returns("hashed-password");

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access-token");

        _refreshTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _repository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        _repository
            .Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => addedRefreshToken = token)
            .Returns(Task.CompletedTask);

        _repository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RegisterCommand("new@test.com", "Password123!", "John");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.AccessToken, Is.EqualTo("access-token"));
        Assert.That(result.RefreshToken, Is.EqualTo("refresh-token"));
        Assert.That(result.Email, Is.EqualTo("new@test.com"));
        Assert.That(result.Name, Is.EqualTo("John"));

        Assert.That(addedUser, Is.Not.Null);
        Assert.That(addedUser!.Email, Is.EqualTo("new@test.com"));
        Assert.That(addedUser.Name, Is.EqualTo("John"));
        Assert.That(addedUser.PasswordHash, Is.EqualTo("hashed-password"));

        Assert.That(addedRefreshToken, Is.Not.Null);
        Assert.That(addedRefreshToken!.Token, Is.EqualTo("refresh-token"));
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ShouldThrowConflict()
    {
        _repository
            .Setup(x => x.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataFactory.CreateUser(email: "existing@test.com"));

        var command = new RegisterCommand("existing@test.com", "Password123!", "John");

        Assert.ThrowsAsync<ConflictException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}