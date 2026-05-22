using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.HealthRecords.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.HealthRecords.Queries;

public sealed record GetHealthRecordByIdQuery(int Id)
    : IRequest<HealthRecordDto>;

public sealed class GetHealthRecordByIdQueryHandler
    : IRequestHandler<GetHealthRecordByIdQuery, HealthRecordDto>
{
    private readonly IHealthRecordRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetHealthRecordByIdQueryHandler(
        IHealthRecordRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<HealthRecordDto> Handle(
        GetHealthRecordByIdQuery request,
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