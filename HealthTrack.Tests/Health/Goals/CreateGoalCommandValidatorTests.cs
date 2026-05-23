using HealthTrack.Application.Health.Goals.Commands;
using HealthTrack.Application.Health.Goals.Validators;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.Goals;

public sealed class CreateGoalCommandValidatorTests
{
    private readonly CreateGoalCommandValidator _validator = new();

    [Test]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var command = new CreateGoalCommand(
            72f,
            12000);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenTargetWeightIsInvalid_ShouldFail()
    {
        var command = new CreateGoalCommand(
            0,
            12000);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors.Exists(
                x => x.PropertyName == nameof(CreateGoalCommand.TargetWeight)),
            Is.True);
    }

    [Test]
    public void Validate_WhenTargetStepsIsInvalid_ShouldFail()
    {
        var command = new CreateGoalCommand(
            72,
            0);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors.Exists(
                x => x.PropertyName == nameof(CreateGoalCommand.TargetSteps)),
            Is.True);
    }
}