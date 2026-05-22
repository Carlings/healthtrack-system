using FluentValidation;
using HealthTrack.Application.Health.Goals.Commands;

namespace HealthTrack.Application.Health.Goals.Validators;

public sealed class CreateGoalCommandValidator : AbstractValidator<CreateGoalCommand>
{
    public CreateGoalCommandValidator()
    {
        RuleFor(x => x.TargetWeight)
            .GreaterThan(0)
            .LessThan(1000);

        RuleFor(x => x.TargetSteps)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100000);
    }
}