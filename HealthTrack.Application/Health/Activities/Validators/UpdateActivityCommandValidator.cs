using FluentValidation;
using HealthTrack.Application.Health.Activities.Commands;

namespace HealthTrack.Application.Health.Activities.Validators;

public sealed class UpdateActivityCommandValidator
    : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.ActivityTypeId)
            .GreaterThan(0);

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 1440);

        RuleFor(x => x.ActivityDate)
            .LessThanOrEqualTo(DateTime.UtcNow);
    }
}