namespace HealthTrack.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
