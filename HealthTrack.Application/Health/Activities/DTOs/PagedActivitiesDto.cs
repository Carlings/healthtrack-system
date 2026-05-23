namespace HealthTrack.Application.Health.Activities.DTOs;

public sealed record PagedActivitiesDto(
    IReadOnlyList<UserActivityDto> Items,
    int Page,
    int PageSize,
    int TotalCount);
