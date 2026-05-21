using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HealthTrack.Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(
            User user,
            string hashedPassword,
            string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                hashedPassword,
                providedPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}
