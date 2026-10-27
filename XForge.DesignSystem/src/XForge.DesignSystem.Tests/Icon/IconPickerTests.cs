using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconPickerTests
{
    [Fact]
    public void Parameters_DocumentationPresent()
    {
        Assert.False(string.IsNullOrEmpty(XFIconPickerParameters.ValueDoc));
        Assert.False(string.IsNullOrEmpty(XFIconPickerParameters.PageSizeDoc));
    }

    [Fact]
    public void Registry_Supports_PaginationDefaults()
    {
        var reg = new XFIconRegistry();
        for (int i = 0; i < 30; i++)
            reg.Register(new XFIconInfo($"a{i}", XFIconVariant.Regular, XFIconSize.Size24,
                new[] { "tag" }, "misc", "<svg/>"));
        Assert.Equal(24, reg.Search("tag", page: 0, pageSize: 24).Count());
    }
}