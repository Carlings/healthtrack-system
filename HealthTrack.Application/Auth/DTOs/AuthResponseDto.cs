namespace HealthTrack.Application.Auth.DTOs
{
    public record AuthResponseDto(
        string AccessToken,
        string Email,
        string Name);
}