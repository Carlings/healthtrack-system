using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Notification>> GetAllByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(
                x => x.Id == id && x.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken)
    {
        await _context.Notifications.AddAsync(
            notification,
            cancellationToken);
    }

    public void Delete(Notification notification)
    {
        _context.Notifications.Remove(notification);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}