using FluentValidation;
using HealthTrack.Application.Identity.Users.Commands;

namespace HealthTrack.Application.Identity.Users.Validators;

public sealed class UpdateCurrentUserCommandValidator
    : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.BirthDate)
            .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Birth date cannot be in the future.");

        RuleFor(x => x.Height)
            .InclusiveBetween(30, 300)
            .When(x => x.Height.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender.HasValue);
    }
}