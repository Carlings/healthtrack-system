using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Goals.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Goals.Commands;

public sealed record UpdateGoalCommand(
    int Id,
    float TargetWeight,
    int TargetSteps) : IRequest<GoalDto>;

public sealed class UpdateGoalCommandHandler
    : IRequestHandler<UpdateGoalCommand, GoalDto>
{
    private readonly IGoalRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateGoalCommandHandler(
        IGoalRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<GoalDto> Handle(
        UpdateGoalCommand request,
        CancellationToken cancellationToken)
    {
        var goal = await _repository.GetByIdAsync(
            request.Id,
            _currentUser.UserId,
            cancellationToken);

        if (goal is null)
        {
            throw new NotFoundException("Goal", request.Id);
        }

        goal.TargetWeight = request.TargetWeight;
        goal.TargetSteps = request.TargetSteps;

        await _repository.SaveChangesAsync(cancellationToken);

        return new GoalDto(
            goal.Id,
            goal.TargetWeight,
            goal.TargetSteps,
            goal.CreatedAt);
    }
}