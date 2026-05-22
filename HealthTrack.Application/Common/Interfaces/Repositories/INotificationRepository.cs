using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetAllByUserIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<Notification?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken);

    void Delete(Notification notification);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}