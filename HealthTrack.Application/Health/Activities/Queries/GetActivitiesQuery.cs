using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Queries;

public sealed record GetActivitiesQuery
    : IRequest<IReadOnlyList<UserActivityDto>>;

public sealed class GetActivitiesQueryHandler
    : IRequestHandler<GetActivitiesQuery, IReadOnlyList<UserActivityDto>>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetActivitiesQueryHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<UserActivityDto>> Handle(
        GetActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        var activities = await _repository.GetByUserIdAsync(
            _currentUser.UserId,
            cancellationToken);

        return [.. activities
            .OrderByDescending(x => x.ActivityDate)
            .Select(x => new UserActivityDto(
                x.Id,
                x.ActivityTypeId,
                x.ActivityType.Name,
                x.DurationMinutes,
                x.CaloriesBurned,
                x.ActivityDate))];
    }
}