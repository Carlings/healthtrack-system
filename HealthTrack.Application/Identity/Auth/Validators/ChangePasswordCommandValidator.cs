using FluentValidation;
using HealthTrack.Application.Identity.Users.Commands;

namespace HealthTrack.Application.Identity.Users.Validators;

public sealed class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);

        RuleFor(x => x.NewPassword)
            .Matches("[A-Z]")
            .WithMessage(
                "Password must contain at least one uppercase letter.");

        RuleFor(x => x.NewPassword)
            .Matches("[a-z]")
            .WithMessage(
                "Password must contain at least one lowercase letter.");

        RuleFor(x => x.NewPassword)
            .Matches("[0-9]")
            .WithMessage(
                "Password must contain at least one digit.");
    }
}