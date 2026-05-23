using HealthTrack.Application.Health.Activities.Queries.GetPaged;
using HealthTrack.Application.Health.Activities.Validators;

namespace HealthTrack.Tests.Health.Activities;

public sealed class GetPagedActivitiesQueryValidatorTests
{
    private readonly GetPagedActivitiesQueryValidator _validator = new();

    [Test]
    public void Validate_WhenPageAndPageSizeValid_ShouldPass()
    {
        var query = new GetPagedActivitiesQuery(Page: 1, PageSize: 10);

        var result = _validator.Validate(query);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenPageInvalid_ShouldFail()
    {
        var query = new GetPagedActivitiesQuery(Page: 0, PageSize: 10);

        var result = _validator.Validate(query);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(x => x.PropertyName == "Page"), Is.True);
    }

    [Test]
    public void Validate_WhenPageSizeInvalid_ShouldFail()
    {
        var query = new GetPagedActivitiesQuery(Page: 1, PageSize: 101);

        var result = _validator.Validate(query);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(x => x.PropertyName == "PageSize"), Is.True);
    }
}
