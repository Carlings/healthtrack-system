using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Goals.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Goals.Queries;

public sealed record GetGoalsQuery : IRequest<IReadOnlyList<GoalDto>>;

public sealed class GetGoalsQueryHandler
    : IRequestHandler<GetGoalsQuery, IReadOnlyList<GoalDto>>
{
    private readonly IGoalRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetGoalsQueryHandler(
        IGoalRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<GoalDto>> Handle(
        GetGoalsQuery request,
        CancellationToken cancellationToken)
    {
        var goals = await _repository.GetByUserIdAsync(
            _currentUser.UserId,
            cancellationToken);

        return goals
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GoalDto(
                x.Id,
                x.TargetWeight,
                x.TargetSteps,
                x.CreatedAt))
            .ToList();
    }
}