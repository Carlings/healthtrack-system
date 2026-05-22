using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.HealthRecords.Commands;

public sealed record UpdateHealthRecordCommand(
    int Id,
    DateTime RecordedAt,
    float Weight,
    int Pulse,
    float Temperature,
    int SystolicBP,
    int DiastolicBP,
    int Steps,
    float SleepHours)
    : IRequest;

public sealed class UpdateHealthRecordCommandHandler
    : IRequestHandler<UpdateHealthRecordCommand>
{
    private readonly IHealthRecordRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateHealthRecordCommandHandler(
        IHealthRecordRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        UpdateHealthRecordCommand request,
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

        record.RecordedAt = request.RecordedAt;
        record.Weight = request.Weight;
        record.Pulse = request.Pulse;
        record.Temperature = request.Temperature;
        record.SystolicBP = request.SystolicBP;
        record.DiastolicBP = request.DiastolicBP;
        record.Steps = request.Steps;
        record.SleepHours = request.SleepHours;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}