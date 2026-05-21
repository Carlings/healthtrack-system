namespace HealthTrack.Application.Common.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
