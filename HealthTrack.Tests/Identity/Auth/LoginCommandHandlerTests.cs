using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.Commands;
using HealthTrack.Domain.Entities;
using HealthTrack.Tests.Common;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Auth;

public sealed class LoginCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<IJwtTokenService> _jwtTokenService = null!;
    private Mock<IPasswordHasherService> _passwordHasherService = null!;
    private Mock<IRefreshTokenService> _refreshTokenService = null!;

    private LoginCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _passwordHasherService = new Mock<IPasswordHasherService>();
        _refreshTokenService = new Mock<IRefreshTokenService>();

        _handler = new LoginCommandHandler(
            _repository.Object,
            _jwtTokenService.Object,
            _passwordHasherService.Object,
            _refreshTokenService.Object);
    }

    [Test]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnTokens()
    {
        var user = TestDataFactory.CreateUser();

        _repository
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherService
            .Setup(x => x.VerifyPassword(user, user.PasswordHash, "Password123!"))
            .Returns(true);

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(user))
            .Returns("access-token");

        _refreshTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var command = new LoginCommand(user.Email, "Password123!");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.AccessToken, Is.EqualTo("access-token"));
        Assert.That(result.RefreshToken, Is.EqualTo("refresh-token"));
        Assert.That(result.Email, Is.EqualTo(user.Email));
        Assert.That(result.Name, Is.EqualTo(user.Name));
    }

    [Test]
    public void Handle_WhenUserDoesNotExist_ShouldThrowUnauthorized()
    {
        _repository
            .Setup(x => x.GetByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("missing@test.com", "Password123!");

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenPasswordIsInvalid_ShouldThrowUnauthorized()
    {
        var user = TestDataFactory.CreateUser();

        _repository
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherService
            .Setup(x => x.VerifyPassword(user, user.PasswordHash, "WrongPassword!"))
            .Returns(false);

        var command = new LoginCommand(user.Email, "WrongPassword!");

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}