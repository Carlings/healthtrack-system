using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Identity.Auth.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(
            User user,
            string hashedPassword,
            string providedPassword);
    }
}
