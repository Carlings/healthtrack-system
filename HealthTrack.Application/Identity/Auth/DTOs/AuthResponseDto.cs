namespace HealthTrack.Application.Identity.Auth.DTOs
{
    public sealed record AuthResponseDto(
        string AccessToken,
        string RefreshToken,
        string Email,
        string Name);
}