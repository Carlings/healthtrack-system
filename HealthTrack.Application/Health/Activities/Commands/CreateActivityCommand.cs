using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Commands;

public sealed record CreateActivityCommand(
    int ActivityTypeId,
    int DurationMinutes,
    DateTime ActivityDate)
    : IRequest<UserActivityDto>;

public sealed class CreateActivityCommandHandler
    : IRequestHandler<CreateActivityCommand, UserActivityDto>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateActivityCommandHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<UserActivityDto> Handle(
        CreateActivityCommand request,
        CancellationToken cancellationToken)
    {
        var activityType = await _repository.GetActivityTypeByIdAsync(
            request.ActivityTypeId,
            cancellationToken);

        if (activityType is null)
        {
            throw new NotFoundException(
                "ActivityType",
                request.ActivityTypeId);
        }

        var caloriesBurned = (int)Math.Round(
            (activityType.CaloriesPerHour / 60f)
            * request.DurationMinutes);

        var activity = new UserActivity
        {
            UserId = _currentUser.UserId,
            ActivityTypeId = request.ActivityTypeId,
            DurationMinutes = request.DurationMinutes,
            CaloriesBurned = caloriesBurned,
            ActivityDate = request.ActivityDate
        };

        await _repository.AddAsync(
            activity,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new UserActivityDto(
            activity.Id,
            activity.ActivityTypeId,
            activityType.Name,
            activity.DurationMinutes,
            activity.CaloriesBurned,
            activity.ActivityDate);
    }
}