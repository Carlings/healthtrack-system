using HealthTrack.Application.Identity.Users.Commands;
using HealthTrack.Application.Identity.Users.Validators;
using HealthTrack.Domain.Entities;

namespace HealthTrack.Tests.Identity.Users;

public sealed class UpdateCurrentUserCommandValidatorTests
{
    private readonly UpdateCurrentUserCommandValidator _validator = new();

    [Test]
    public void Validate_WhenPartialUpdateIsValid_ShouldBeValid()
    {
        var command = new UpdateCurrentUserCommand(
            "John Updated",
            null,
            181,
            Gender.Male);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenBirthDateIsInFuture_ShouldFail()
    {
        var command = new UpdateCurrentUserCommand(
            "John",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            180,
            Gender.Male);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Validate_WhenHeightIsInvalid_ShouldFail()
    {
        var command = new UpdateCurrentUserCommand(
            "John",
            null,
            999,
            Gender.Male);

        var result = _validator.Validate(command);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.GreaterThan(0));
        });
    }
}