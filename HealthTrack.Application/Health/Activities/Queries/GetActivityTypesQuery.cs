using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Activities.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Queries;

public sealed record GetActivityTypesQuery
    : IRequest<IReadOnlyList<ActivityTypeDto>>;

public sealed class GetActivityTypesQueryHandler
    : IRequestHandler<GetActivityTypesQuery, IReadOnlyList<ActivityTypeDto>>
{
    private readonly IUserActivityRepository _repository;

    public GetActivityTypesQueryHandler(
        IUserActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ActivityTypeDto>> Handle(
        GetActivityTypesQuery request,
        CancellationToken cancellationToken)
    {
        var activityTypes = await _repository.GetActivityTypesAsync(
            cancellationToken);

        return [.. activityTypes
            .Select(x => new ActivityTypeDto(
                x.Id,
                x.Name,
                x.CaloriesPerHour))];
    }
}