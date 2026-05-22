using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories;

public sealed class UserActivityRepository : IUserActivityRepository
{
    private readonly AppDbContext _context;

    public UserActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserActivity>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserActivities
            .AsNoTracking()
            .Include(x => x.ActivityType)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ActivityDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserActivity?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserActivities
            .Include(x => x.ActivityType)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.UserId == userId,
                cancellationToken);
    }

    public async Task<ActivityType?> GetActivityTypeByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.ActivityTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ActivityType>> GetActivityTypesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.ActivityTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        UserActivity activity,
        CancellationToken cancellationToken)
    {
        await _context.UserActivities.AddAsync(
            activity,
            cancellationToken);
    }

    public void Delete(UserActivity activity)
    {
        _context.UserActivities.Remove(activity);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}