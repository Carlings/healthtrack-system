using FluentValidation;
using HealthTrack.Application.Identity.Auth.Commands;

namespace HealthTrack.Application.Identity.Auth.Validators
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
