using FluentValidation;
using HealthTrack.Application.Identity.Users.Commands;

namespace HealthTrack.Application.Identity.Users.Validators;

public sealed class UploadAvatarCommandValidator
    : AbstractValidator<UploadAvatarCommand>
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.Stream)
            .NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(HaveAllowedExtension)
            .WithMessage("Unsupported image format. Allowed: jpg, jpeg, png, webp.");

        RuleFor(x => x.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(5 * 1024 * 1024)
            .WithMessage("Avatar size must not exceed 5 MB.");
    }

    private static bool HaveAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return AllowedExtensions.Contains(extension);
    }
}