using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces.Identity
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(User user);
    }
}
