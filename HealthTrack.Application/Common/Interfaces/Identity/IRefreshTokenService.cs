namespace HealthTrack.Application.Common.Interfaces.Identity;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
}