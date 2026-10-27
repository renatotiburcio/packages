using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Avatar;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFAvatarGroupExtendedTest : BunitContext
{
    public XFAvatarGroupExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFAvatarGroup> RenderAvatarGroup(
        Action<ComponentParameterCollectionBuilder<XFAvatarGroup>>? configure = null)
    {
        return Render<XFAvatarGroup>(p =>
        {
            p.AddChildContent("<span>Avatar</span>");
            configure?.Invoke(p);
        });
    }

    // Stacked true (default) renders negative spacing (line 23)
    [Fact]
    public void Extended_StackedTrue_RendersNegativeSpacing()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.Stacked, true));
        Assert.Contains("-space-x-1", cut.Markup);
    }

    // Stacked false renders gap (line 23)
    [Fact]
    public void Extended_StackedFalse_RendersGap()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.Stacked, false));
        Assert.Contains("gap-2", cut.Markup);
    }

    // OverflowClass Xs (line 29)
    [Fact]
    public void Extended_OverflowSizeXs()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.Size, XFAvatarSize.Xs);
            p.Add(x => x.OverflowCount, 3);
        });
        Assert.Contains("h-6", cut.Markup);
        Assert.Contains("min-w-6", cut.Markup);
        Assert.Contains("text-[10px]", cut.Markup);
    }

    // OverflowClass Sm (line 30)
    [Fact]
    public void Extended_OverflowSizeSm()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.Size, XFAvatarSize.Sm);
            p.Add(x => x.OverflowCount, 3);
        });
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("min-w-8", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // OverflowClass Lg (line 31)
    [Fact]
    public void Extended_OverflowSizeLg()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.Size, XFAvatarSize.Lg);
            p.Add(x => x.OverflowCount, 3);
        });
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("min-w-12", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // OverflowClass Xl (line 32)
    [Fact]
    public void Extended_OverflowSizeXl()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.Size, XFAvatarSize.Xl);
            p.Add(x => x.OverflowCount, 3);
        });
        Assert.Contains("h-16", cut.Markup);
        Assert.Contains("min-w-16", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // OverflowClass Xxl (line 33)
    [Fact]
    public void Extended_OverflowSizeXxl()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.Size, XFAvatarSize.Xxl);
            p.Add(x => x.OverflowCount, 3);
        });
        Assert.Contains("h-20", cut.Markup);
        Assert.Contains("min-w-20", cut.Markup);
        Assert.Contains("text-lg", cut.Markup);
    }

    // OverflowClass default Md (line 34)
    [Fact]
    public void Extended_OverflowSizeDefault()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.OverflowCount, 3));
        Assert.Contains("h-10", cut.Markup);
        Assert.Contains("min-w-10", cut.Markup);
    }

    // OverflowCount 0 renders no overflow (line 13)
    [Fact]
    public void Extended_OverflowCountZero_NoOverflow()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.OverflowCount, 0));
        Assert.DoesNotContain("+0", cut.Markup);
    }

    // OverflowCount > 0 renders overflow label (line 13-15)
    [Fact]
    public void Extended_OverflowCountPositive_RendersLabel()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.OverflowCount, 5));
        Assert.Contains("+5", cut.Markup);
    }

    // Visible false renders nothing (line 4)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // OverflowLabel custom (line 41-43)
    [Fact]
    public void Extended_CustomOverflowLabel()
    {
        var cut = RenderAvatarGroup(p =>
        {
            p.Add(x => x.OverflowCount, 3);
            p.Add(x => x.OverflowLabel, "Custom label");
        });
        Assert.Contains("Custom label", cut.Markup);
    }

    // ResolvedOverflowLabel single format (line 46)
    [Fact]
    public void Extended_OverflowLabel_SingleFormat()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.OverflowCount, 1));
        Assert.Contains("usuario adicional", cut.Markup);
    }

    // ResolvedOverflowLabel multiple format (line 46)
    [Fact]
    public void Extended_OverflowLabel_MultipleFormat()
    {
        var cut = RenderAvatarGroup(p => p.Add(x => x.OverflowCount, 2));
        Assert.Contains("usuarios adicionais", cut.Markup);
    }
}
