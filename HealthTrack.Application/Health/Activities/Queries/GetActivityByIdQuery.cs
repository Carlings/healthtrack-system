using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Queries;

public sealed record GetActivityByIdQuery(int Id)
    : IRequest<UserActivityDto>;

public sealed class GetActivityByIdQueryHandler
    : IRequestHandler<GetActivityByIdQuery, UserActivityDto>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetActivityByIdQueryHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<UserActivityDto> Handle(
        GetActivityByIdQuery request,
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

        return new UserActivityDto(
            activity.Id,
            activity.ActivityTypeId,
            activity.ActivityType.Name,
            activity.DurationMinutes,
            activity.CaloriesBurned,
            activity.ActivityDate);
    }
}