using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces.Repositories;

public interface IUserActivityRepository
{
    Task<IReadOnlyList<UserActivity>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<UserActivity?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken);

    Task<ActivityType?> GetActivityTypeByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ActivityType>> GetActivityTypesAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        UserActivity activity,
        CancellationToken cancellationToken);

    void Delete(UserActivity activity);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}