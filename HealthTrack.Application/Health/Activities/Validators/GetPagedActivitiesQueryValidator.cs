using FluentValidation;
using HealthTrack.Application.Health.Activities.Queries.GetPaged;

namespace HealthTrack.Application.Health.Activities.Validators;

public sealed class GetPagedActivitiesQueryValidator : AbstractValidator<GetPagedActivitiesQuery>
{
    public GetPagedActivitiesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
