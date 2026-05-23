using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.Commands;
using HealthTrack.Domain.Entities;
using HealthTrack.Tests.Common;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Identity.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    private Mock<IUserRepository> _repository = null!;
    private Mock<IJwtTokenService> _jwtTokenService = null!;
    private Mock<IRefreshTokenService> _refreshTokenService = null!;

    private RefreshTokenCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUserRepository>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _refreshTokenService = new Mock<IRefreshTokenService>();

        _handler = new RefreshTokenCommandHandler(
            _repository.Object,
            _jwtTokenService.Object,
            _refreshTokenService.Object);
    }

    [Test]
    public async Task Handle_WhenTokenIsValid_ShouldRotateRefreshToken()
    {
        var user = TestDataFactory.CreateUser();
        var oldToken = TestDataFactory.CreateRefreshToken(
            user,
            token: "old-refresh-token",
            isRevoked: false,
            expiresAt: DateTime.UtcNow.AddDays(1));

        RefreshToken? newToken = null;

        _repository
            .Setup(x => x.GetRefreshTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldToken);

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(user))
            .Returns("access-token");

        _refreshTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _repository
            .Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => newToken = token)
            .Returns(Task.CompletedTask);

        _repository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RefreshTokenCommand("old-refresh-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.AccessToken, Is.EqualTo("access-token"));
        Assert.That(result.RefreshToken, Is.EqualTo("new-refresh-token"));
        Assert.That(oldToken.IsRevoked, Is.True);

        Assert.That(newToken, Is.Not.Null);
        Assert.That(newToken!.Token, Is.EqualTo("new-refresh-token"));
        Assert.That(newToken.UserId, Is.EqualTo(user.Id));
    }

    [Test]
    public void Handle_WhenTokenNotFound_ShouldThrow()
    {
        _repository
            .Setup(x => x.GetRefreshTokenAsync("missing-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var command = new RefreshTokenCommand("missing-token");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenTokenIsExpired_ShouldThrow()
    {
        var user = TestDataFactory.CreateUser();
        var expiredToken = TestDataFactory.CreateRefreshToken(
            user,
            token: "expired-token",
            isRevoked: false,
            expiresAt: DateTime.UtcNow.AddMinutes(-5));

        _repository
            .Setup(x => x.GetRefreshTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        var command = new RefreshTokenCommand("expired-token");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}