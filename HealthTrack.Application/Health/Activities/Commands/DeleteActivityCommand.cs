using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.Activities.Commands;

public sealed record DeleteActivityCommand(int Id)
    : IRequest;

public sealed class DeleteActivityCommandHandler
    : IRequestHandler<DeleteActivityCommand>
{
    private readonly IUserActivityRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public DeleteActivityCommandHandler(
        IUserActivityRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteActivityCommand request,
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

        _repository.Delete(activity);

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}