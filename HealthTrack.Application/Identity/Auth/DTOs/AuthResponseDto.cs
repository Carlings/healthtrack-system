namespace HealthTrack.Application.Identity.Auth.DTOs
{
    public record AuthResponseDto(
        string AccessToken,
        string Email,
        string Name);
}