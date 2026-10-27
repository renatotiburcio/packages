using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;

namespace XForge.DesignSystem.Tests.Components;

public class XFFooterComponentTest : BunitContext
{
    public XFFooterComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_ShowsDefaultStatusText()
    {
        var cut = Render<XFFooter>();
        Assert.Contains("Estavel", cut.Markup);
    }

    [Fact]
    public void Render_ShowsStatusText()
    {
        var cut = Render<XFFooter>(parameters =>
            parameters.Add(p => p.StatusText, "Online"));
        Assert.Contains("Online", cut.Markup);
    }

    [Fact]
    public void Render_WhenAppNameSet_ShowsAppName()
    {
        var cut = Render<XFFooter>(parameters =>
            parameters.Add(p => p.AppName, "XForge ERP"));
        Assert.Contains("XForge ERP", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotShowStatusDot_NoDotRendered()
    {
        var cut = Render<XFFooter>(parameters =>
            parameters.Add(p => p.ShowStatusDot, false));
        Assert.DoesNotContain("bg-success", cut.Markup);
    }

    [Fact]
    public void Render_WhenUserNameSet_ShowsUserName()
    {
        var cut = Render<XFFooter>(parameters =>
            parameters.Add(p => p.UserName, "Renato"));
        Assert.Contains("Renato", cut.Markup);
    }
}