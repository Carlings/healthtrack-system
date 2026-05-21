using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Auth.Interfaces
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(User user);
    }
}
