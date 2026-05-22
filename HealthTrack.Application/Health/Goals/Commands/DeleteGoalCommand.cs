using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.Goals.Commands;

public sealed record DeleteGoalCommand(int Id) : IRequest;

public sealed class DeleteGoalCommandHandler
    : IRequestHandler<DeleteGoalCommand>
{
    private readonly IGoalRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public DeleteGoalCommandHandler(
        IGoalRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteGoalCommand request,
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

        _repository.Delete(goal);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}