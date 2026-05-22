using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.HealthRecords.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Health.HealthRecords.Commands;

public sealed record CreateHealthRecordCommand(
    DateTime RecordedAt,
    float Weight,
    int Pulse,
    float Temperature,
    int SystolicBP,
    int DiastolicBP,
    int Steps,
    float SleepHours)
    : IRequest<HealthRecordDto>;

public sealed class CreateHealthRecordCommandHandler
    : IRequestHandler<CreateHealthRecordCommand, HealthRecordDto>
{
    private readonly IHealthRecordRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateHealthRecordCommandHandler(
        IHealthRecordRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<HealthRecordDto> Handle(
        CreateHealthRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = new HealthRecord
        {
            UserId = _currentUser.UserId,
            RecordedAt = request.RecordedAt,
            Weight = request.Weight,
            Pulse = request.Pulse,
            Temperature = request.Temperature,
            SystolicBP = request.SystolicBP,
            DiastolicBP = request.DiastolicBP,
            Steps = request.Steps,
            SleepHours = request.SleepHours
        };

        await _repository.AddAsync(record, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return new HealthRecordDto(
            record.Id,
            record.RecordedAt,
            record.Weight,
            record.Pulse,
            record.Temperature,
            record.SystolicBP,
            record.DiastolicBP,
            record.Steps,
            record.SleepHours);
    }
}