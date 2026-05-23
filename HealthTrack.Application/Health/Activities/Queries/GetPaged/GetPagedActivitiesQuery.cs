using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Queries.GetPaged;

public sealed record GetPagedActivitiesQuery(
    int Page = 1,
    int PageSize = 10) : IRequest<PagedActivitiesDto>;

public sealed class GetPagedActivitiesQueryHandler
    : IRequestHandler<GetPagedActivitiesQuery, PagedActivitiesDto>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetPagedActivitiesQueryHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedActivitiesDto> Handle(
        GetPagedActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        var activities = await _repository.GetByUserIdAsync(
            _currentUser.UserId,
            cancellationToken);

        var ordered = activities
            .OrderByDescending(x => x.ActivityDate)
            .ToList();

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UserActivityDto(
                x.Id,
                x.ActivityTypeId,
                x.ActivityType.Name,
                x.DurationMinutes,
                x.CaloriesBurned,
                x.ActivityDate))
            .ToList();

        return new PagedActivitiesDto(
            items,
            page,
            pageSize,
            totalCount);
    }
}
