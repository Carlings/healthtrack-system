using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using HealthTrack.Application.Common.Interfaces.Identity;


namespace HealthTrack.Api.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public int UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User
                    ?? throw new UnauthorizedAccessException("No authenticated user context.");

                var rawId =
                    user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                    user.FindFirstValue("userId");

                if (string.IsNullOrWhiteSpace(rawId))
                {
                    throw new UnauthorizedAccessException("User id claim is missing.");
                }

                if (!int.TryParse(rawId, out var userId))
                {
                    throw new UnauthorizedAccessException("User id claim has invalid format.");
                }

                return userId;
            }
        }

        public string Email
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User
                    ?? throw new UnauthorizedAccessException("No authenticated user context.");

                return user.FindFirstValue(ClaimTypes.Email)
                       ?? user.FindFirstValue(JwtRegisteredClaimNames.Email)
                       ?? string.Empty;
            }
        }
    }
}