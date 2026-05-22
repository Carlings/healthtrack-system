using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Goals.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Health.Goals.Commands;

public sealed record CreateGoalCommand(
    float TargetWeight,
    int TargetSteps) : IRequest<GoalDto>;

public sealed class CreateGoalCommandHandler
    : IRequestHandler<CreateGoalCommand, GoalDto>
{
    private readonly IGoalRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateGoalCommandHandler(
        IGoalRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<GoalDto> Handle(
        CreateGoalCommand request,
        CancellationToken cancellationToken)
    {
        var goal = new Goal
        {
            UserId = _currentUser.UserId,
            TargetWeight = request.TargetWeight,
            TargetSteps = request.TargetSteps,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(goal, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new GoalDto(
            goal.Id,
            goal.TargetWeight,
            goal.TargetSteps,
            goal.CreatedAt);
    }
}