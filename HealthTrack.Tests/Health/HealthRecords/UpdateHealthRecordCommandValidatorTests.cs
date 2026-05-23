using HealthTrack.Application.Health.HealthRecords.Commands;
using HealthTrack.Application.Health.HealthRecords.Validators;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class UpdateHealthRecordCommandValidatorTests
{
    private readonly UpdateHealthRecordCommandValidator _validator = new();

    [Test]
    public void Validate_WhenRecordedAtIsInFuture_ShouldFail()
    {
        var command = new UpdateHealthRecordCommand(
            Id: 1,
            RecordedAt: DateTime.UtcNow.AddMinutes(5),
            Weight: 78.5f,
            Pulse: 72,
            Temperature: 36.6f,
            SystolicBP: 120,
            DiastolicBP: 80,
            Steps: 8500,
            SleepHours: 7.5f);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Exists(x => x.PropertyName == nameof(UpdateHealthRecordCommand.RecordedAt)), Is.True);
    }
}
