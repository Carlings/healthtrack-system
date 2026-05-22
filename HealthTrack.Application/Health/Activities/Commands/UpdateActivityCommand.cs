using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Commands;

public sealed record UpdateActivityCommand(
    int Id,
    int ActivityTypeId,
    int DurationMinutes,
    DateTime ActivityDate)
    : IRequest<UserActivityDto>;

public sealed class UpdateActivityCommandHandler
    : IRequestHandler<UpdateActivityCommand, UserActivityDto>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateActivityCommandHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<UserActivityDto> Handle(
        UpdateActivityCommand request,
        CancellationToken cancellationToken)
    {
        var activity = await _repository.GetByIdAsync(
            request.Id,
            _currentUser.UserId,
            cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException(
                "UserActivity",
                request.Id);
        }

        var activityType = await _repository.GetActivityTypeByIdAsync(
            request.ActivityTypeId,
            cancellationToken);

        if (activityType is null)
        {
            throw new NotFoundException(
                "ActivityType",
                request.ActivityTypeId);
        }

        activity.ActivityTypeId = request.ActivityTypeId;
        activity.DurationMinutes = request.DurationMinutes;
        activity.ActivityDate = request.ActivityDate;

        activity.CaloriesBurned =
            (int)((activityType.CaloriesPerHour / 60f)
            * request.DurationMinutes);

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