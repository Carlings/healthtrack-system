using FluentValidation;
using HealthTrack.Application.Health.Goals.Commands;

namespace HealthTrack.Application.Health.Goals.Validators;

public sealed class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.TargetWeight)
            .GreaterThan(0)
            .LessThan(1000);

        RuleFor(x => x.TargetSteps)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100000);
    }
}