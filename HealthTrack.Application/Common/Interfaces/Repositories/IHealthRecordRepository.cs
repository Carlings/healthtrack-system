using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Interfaces.Repositories
{
    public interface IHealthRecordRepository
    {
        Task<IReadOnlyList<HealthRecord>> GetByUserIdAsync(
            int userId,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken);

        Task<HealthRecord?> GetByIdAsync(
            int id,
            int userId,
            CancellationToken cancellationToken);

        Task AddAsync(
            HealthRecord record,
            CancellationToken cancellationToken);

        void Delete(HealthRecord record);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
