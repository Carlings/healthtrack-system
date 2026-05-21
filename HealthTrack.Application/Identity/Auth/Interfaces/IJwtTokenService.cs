using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Identity.Auth.Interfaces
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(User user);
    }
}
