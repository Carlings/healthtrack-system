using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.HealthRecords.DTOs;
using MediatR;

namespace HealthTrack.Application.HealthRecords.Queries.GetList;

public sealed record GetHealthRecordsQuery(
    DateTime? From,
    DateTime? To) : IRequest<IReadOnlyList<HealthRecordDto>>;

public sealed class GetHealthRecordsQueryHandler
    : IRequestHandler<GetHealthRecordsQuery, IReadOnlyList<HealthRecordDto>>
{
    private readonly IHealthRecordRepository _healthRecords;
    private readonly ICurrentUserService _currentUser;

    public GetHealthRecordsQueryHandler(
        IHealthRecordRepository healthRecords,
        ICurrentUserService currentUser)
    {
        _healthRecords = healthRecords;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<HealthRecordDto>> Handle(
        GetHealthRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await _healthRecords.GetByUserIdAsync(
            _currentUser.UserId,
            request.From,
            request.To,
            cancellationToken);

        return [.. records
            .Select(x => new HealthRecordDto(
                x.Id,
                x.RecordedAt,
                x.Weight,
                x.Pulse,
                x.Temperature,
                x.SystolicBP,
                x.DiastolicBP,
                x.Steps,
                x.SleepHours))];
    }
}