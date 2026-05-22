using FluentValidation;
using HealthTrack.Application.HealthRecords.Queries.GetList;

namespace HealthTrack.Application.Health.HealthRecords.Validators;

public sealed class GetHealthRecordsQueryValidator : AbstractValidator<GetHealthRecordsQuery>
{
    public GetHealthRecordsQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From.Value <= x.To.Value)
            .WithMessage("'From' must be less than or equal to 'To'.");
    }
}