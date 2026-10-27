using XForge.Analyzers;
using FluentAssertions;
using Xunit;
using Microsoft.CodeAnalysis;

namespace XForge.Tests.Analyzers;

public class XFCatalogedAnalyzersTests
{
    [Fact]
    public void All_ContainsFourAnalyzers()
    {
        XFCatalogedAnalyzers.All.Should().HaveCount(4);
    }

    [Fact]
    public void All_IdsAreUnique()
    {
        var ids = XFCatalogedAnalyzers.All.Select(a => a.Id).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void All_HaveXF001()
    {
        XFCatalogedAnalyzers.All.Should().Contain(a => a.Id == "XF001");
    }

    [Fact]
    public void All_HaveNonEmptyTitle()
    {
        XFCatalogedAnalyzers.All.Should().OnlyContain(a => !string.IsNullOrWhiteSpace(a.Title));
    }

    [Fact]
    public void All_HaveValidSeverity()
    {
        XFCatalogedAnalyzers.All.Should().OnlyContain(a =>
            a.DefaultSeverity == DiagnosticSeverity.Info ||
            a.DefaultSeverity == DiagnosticSeverity.Warning ||
            a.DefaultSeverity == DiagnosticSeverity.Error ||
            a.DefaultSeverity == DiagnosticSeverity.Hidden);
    }
}