using HealthTrack.Application.Common.Interfaces.Identity;
using System.Security.Cryptography;

namespace HealthTrack.Infrastructure.Identity;

public sealed class RefreshTokenService
    : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}