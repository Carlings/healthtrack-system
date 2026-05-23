using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories;

public sealed class HealthRecordRepository : IHealthRecordRepository
{
    private readonly AppDbContext _context;

    public HealthRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<HealthRecord>> GetByUserIdAsync(
        int userId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = _context.HealthRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(x => x.RecordedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.RecordedAt <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<HealthRecord?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.HealthRecords
            .FirstOrDefaultAsync(
                x => x.Id == id && x.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        HealthRecord record,
        CancellationToken cancellationToken)
    {
        await _context.HealthRecords.AddAsync(
            record,
            cancellationToken);
    }

    public void Delete(HealthRecord record)
    {
        _context.HealthRecords.Remove(record);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<HealthRecord?> GetLastBeforeAsync(
        int userId,
        DateTime before,
        CancellationToken cancellationToken)
    {
        return await _context.HealthRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RecordedAt <= before)
            .OrderByDescending(x => x.RecordedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}