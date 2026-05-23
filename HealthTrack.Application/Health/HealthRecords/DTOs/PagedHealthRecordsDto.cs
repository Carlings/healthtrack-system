namespace HealthTrack.Application.Health.HealthRecords.DTOs;

public sealed record PagedHealthRecordsDto(
    IReadOnlyList<HealthRecordDto> Items,
    int Page,
    int PageSize,
    int TotalCount);
