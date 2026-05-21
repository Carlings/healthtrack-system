namespace HealthTrack.Application.Identity.Users.DTOs
{
    public sealed record CurrentUserDto(
        int Id,
        string Email,
        string Name,
        DateTime? BirthDate,
        int? Height,
        string? Gender,
        DateTime CreatedAt);
}
