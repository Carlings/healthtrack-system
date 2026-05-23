using FluentValidation;
using HealthTrack.Application.Identity.Auth.Commands;

namespace HealthTrack.Application.Identity.Auth.Validators;

public sealed class RefreshTokenCommandValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}