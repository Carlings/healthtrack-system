using FluentValidation;
using HealthTrack.Application.Health.Notifications.Commands;

namespace HealthTrack.Application.Health.Notifications.Validators;

public sealed class CreateNotificationCommandValidator
    : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Type).
            IsInEnum();
    }
}