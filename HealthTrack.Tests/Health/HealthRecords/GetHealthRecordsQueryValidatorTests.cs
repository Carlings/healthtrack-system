using HealthTrack.Application.Health.HealthRecords.Validators;
using HealthTrack.Application.HealthRecords.Queries.GetList;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class GetHealthRecordsQueryValidatorTests
{
    private readonly GetHealthRecordsQueryValidator _validator = new();

    [Test]
    public void Validate_WhenRangeIsValid_ShouldBeValid()
    {
        var query = new GetHealthRecordsQuery(
            new DateTime(2026, 05, 01),
            new DateTime(2026, 05, 22));

        var result = _validator.Validate(query);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenFromIsGreaterThanTo_ShouldFail()
    {
        var query = new GetHealthRecordsQuery(
            new DateTime(2026, 05, 22),
            new DateTime(2026, 05, 01));

        var result = _validator.Validate(query);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.GreaterThan(0));
    }
}