using Microsoft.Extensions.DependencyInjection;

namespace XForge.Validation.Tests;

public class EnsureTests
{
    [Fact]
    public void NotNull_throws_for_null()
    {
        string? value = null;

        var exception = Assert.Throws<GuardException>(() => Ensure.Arg(value, nameof(value)).NotNull());

        Assert.Equal("value", exception.ParameterName);
        Assert.Contains("NOT_NULL", exception.Message);
    }

    [Fact]
    public void NotNull_passes_for_value()
    {
        Ensure.Arg("ok", "value").NotNull();
    }

    [Fact]
    public void NotEmpty_throws_for_empty_string()
    {
        Assert.Throws<GuardException>(() => Ensure.Arg(string.Empty, "value").NotEmpty());
        Ensure.Arg("x", "value").NotEmpty();
    }

    [Fact]
    public void NotNullOrWhiteSpace_throws_for_whitespace()
    {
        Assert.Throws<GuardException>(() => Ensure.Arg("   ", "value").NotNullOrWhiteSpace());
        Ensure.Arg("x", "value").NotNullOrWhiteSpace();
    }

    [Fact]
    public void Range_validations()
    {
        Ensure.Arg(10, "value").GreaterThan(5).LessThan(20).InRange(5, 20);

        Assert.Throws<GuardException>(() => Ensure.Arg(3, "value").GreaterThan(5));
        Assert.Throws<GuardException>(() => Ensure.Arg(25, "value").InRange(5, 20));
    }

    [Fact]
    public void Matches_validates_format()
    {
        Ensure.Arg("abc-123", "code").Matches("^[a-z]+-[0-9]+$");
        Assert.Throws<GuardException>(() => Ensure.Arg("INVALID", "code").Matches("^[a-z]+-[0-9]+$"));
    }

    [Fact]
    public void IsIn_restricts_values()
    {
        Ensure.Arg("a", "value").IsIn("a", "b");
        Assert.Throws<GuardException>(() => Ensure.Arg("c", "value").IsIn("a", "b"));
    }

    [Fact]
    public void That_and_Against()
    {
        Ensure.That(true, "deve passar");
        Assert.Throws<GuardException>(() => Ensure.That(false, "falhou"));

        Ensure.Against(false, "não deve lançar");
        Assert.Throws<GuardException>(() => Ensure.Against(true, "falhou"));
    }
}
