using FluentValidation;
using HealthTrack.Application.Health.HealthRecords.Queries.GetPaged;

namespace HealthTrack.Application.Health.HealthRecords.Validators;

public sealed class GetPagedHealthRecordsQueryValidator : AbstractValidator<GetPagedHealthRecordsQuery>
{
    public GetPagedHealthRecordsQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From.Value <= x.To.Value)
            .WithMessage("'From' must be less than or equal to 'To'.");

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
