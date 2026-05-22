using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories;

public sealed class GoalRepository : IGoalRepository
{
    private readonly AppDbContext _context;

    public GoalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Goal>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.Goals
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Goal?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.Goals
            .FirstOrDefaultAsync(
                x => x.Id == id && x.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        Goal goal,
        CancellationToken cancellationToken)
    {
        await _context.Goals.AddAsync(goal, cancellationToken);
    }

    public void Delete(Goal goal)
    {
        _context.Goals.Remove(goal);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}