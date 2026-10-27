using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconInspectorTests
{
    [Fact]
    public void Registry_Search_PaginatesResults()
    {
        var r = new XFIconRegistry();
        for (int i = 0; i < 50; i++)
            r.Register(new XFIconInfo($"x{i}", XFIconVariant.Regular, XFIconSize.Size24, new[] { "common" }, "misc", "<svg/>"));
        Assert.Equal(24, r.Search("common", 0, 24).Count());
        Assert.Equal(24, r.Search("common", 1, 24).Count());
    }
}