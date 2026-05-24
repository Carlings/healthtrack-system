using FluentValidation;
using HealthTrack.Application.Identity.Auth.Commands;

namespace HealthTrack.Application.Identity.Auth.Validators
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .Matches("[A-Z]")
                .WithMessage(
                    "Password must contain at least one uppercase letter.");

            RuleFor(x => x.Password)
                .Matches("[a-z]")
                .WithMessage(
                    "Password must contain at least one lowercase letter.");

            RuleFor(x => x.Password)
                .Matches("[0-9]")
                .WithMessage(
                    "Password must contain at least one digit.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
