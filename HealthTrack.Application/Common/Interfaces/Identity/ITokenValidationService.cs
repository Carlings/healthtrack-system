using System.Security.Claims;

namespace HealthTrack.Application.Common.Interfaces.Identity;

public interface ITokenValidationService
{
    Task<bool> IsValidAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken);
}