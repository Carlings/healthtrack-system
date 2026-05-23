using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.HealthRecords.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.HealthRecords.Queries.GetPaged;

public sealed record GetPagedHealthRecordsQuery(
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedHealthRecordsDto>;

public sealed class GetPagedHealthRecordsQueryHandler
    : IRequestHandler<GetPagedHealthRecordsQuery, PagedHealthRecordsDto>
{
    private readonly IHealthRecordRepository _healthRecords;
    private readonly ICurrentUserService _currentUser;

    public GetPagedHealthRecordsQueryHandler(
        IHealthRecordRepository healthRecords,
        ICurrentUserService currentUser)
    {
        _healthRecords = healthRecords;
        _currentUser = currentUser;
    }

    public async Task<PagedHealthRecordsDto> Handle(
        GetPagedHealthRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await _healthRecords.GetByUserIdAsync(
            _currentUser.UserId,
            request.From,
            request.To,
            cancellationToken);

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var totalCount = records.Count;

        var items = records
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new HealthRecordDto(
                x.Id,
                x.RecordedAt,
                x.Weight,
                x.Pulse,
                x.Temperature,
                x.SystolicBP,
                x.DiastolicBP,
                x.Steps,
                x.SleepHours))
            .ToList();

        return new PagedHealthRecordsDto(
            items,
            page,
            pageSize,
            totalCount);
    }
}
