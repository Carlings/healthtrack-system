using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(User user);
    }
}
