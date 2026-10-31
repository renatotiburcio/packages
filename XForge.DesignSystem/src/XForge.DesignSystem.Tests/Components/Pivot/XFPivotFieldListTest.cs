using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Pivot;

namespace XForge.DesignSystem.Tests.Components.Pivot;

public class XFPivotFieldListTest : XForgeTestContext
{
    public XFPivotFieldListTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFPivotFieldList>();
        Assert.Contains("xf-pivotfieldlist", cut.Markup);
    }

    [Fact]
    public void Render_WithFields_ShowsFields()
    {
        var cut = Render<XFPivotFieldList>(p => p.Add(x => x.AvailableFields, new List<string> { "Name", "Age", "City" }));
        Assert.Contains("Name", cut.Markup);
        Assert.Contains("Age", cut.Markup);
        Assert.Contains("City", cut.Markup);
    }
}
