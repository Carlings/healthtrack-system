using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces.Repositories;

public interface IGoalRepository
{
    Task<IReadOnlyList<Goal>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<Goal?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Goal goal,
        CancellationToken cancellationToken);

    void Delete(Goal goal);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}