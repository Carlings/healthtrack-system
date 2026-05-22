using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.HealthRecords.Commands;

public sealed record DeleteHealthRecordCommand(int Id)
    : IRequest;

public sealed class DeleteHealthRecordCommandHandler
    : IRequestHandler<DeleteHealthRecordCommand>
{
    private readonly IHealthRecordRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public DeleteHealthRecordCommandHandler(
        IHealthRecordRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteHealthRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(
            request.Id,
            _currentUser.UserId,
            cancellationToken);

        if (record is null)
        {
            throw new NotFoundException(
                "HealthRecord",
                request.Id);
        }

        _repository.Delete(record);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}