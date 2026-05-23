using FluentValidation;
using HealthTrack.Application.Health.HealthRecords.Commands;

namespace HealthTrack.Application.Health.HealthRecords.Validators;

public sealed class CreateHealthRecordCommandValidator
    : AbstractValidator<CreateHealthRecordCommand>
{
    public CreateHealthRecordCommandValidator()
    {
        RuleFor(x => x.RecordedAt)
            .LessThanOrEqualTo(_ => DateTime.UtcNow)
            .WithMessage("RecordedAt cannot be in the future.");

        RuleFor(x => x.Weight)
            .GreaterThan(0);

        RuleFor(x => x.Pulse)
            .InclusiveBetween(20, 300);

        RuleFor(x => x.Temperature)
            .InclusiveBetween(30, 45);

        RuleFor(x => x.Steps)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SleepHours)
            .InclusiveBetween(0, 24);
    }
}
