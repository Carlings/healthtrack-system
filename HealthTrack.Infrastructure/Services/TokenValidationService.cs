using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HealthTrack.Infrastructure.Services
{
    public sealed class TokenValidationService : ITokenValidationService
    {
        private readonly AppDbContext _context;

        public TokenValidationService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsValidAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var tokenVersionClaim = principal.FindFirstValue(
                "tokenVersion");

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return false;
            }

            if (!int.TryParse(tokenVersionClaim, out var tokenVersion))
            {
                return false;
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken);

            if (user is null)
            {
                return false;
            }

            return user.TokenVersion == tokenVersion;
        }
    }
}
