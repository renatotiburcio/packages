#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Avatar;
using XForge.DesignSystem.Components.DataDisplay.Badge;
using XForge.DesignSystem.Components.DataDisplay.Card;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Components.DataDisplay.Code;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Components.DataDisplay.Image;
using XForge.DesignSystem.Components.DataDisplay.Carousel;
using XForge.DesignSystem.Components.DataDisplay.List;
using XForge.DesignSystem.Components.DataDisplay.Table;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Components.DataDisplay.TreeView;
using XForge.DesignSystem.Components.DataDisplay.VirtualList;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFDisplayTest : BunitContext
{
    public XFDisplayTest() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void XFAvatar_Render_WithImageSrc()
    {
        var cut = Render<XFAvatar>(p => p.Add(x => x.ImageSrc, "img.png"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAvatar_Render_WithInitials()
    {
        var cut = Render<XFAvatar>(p => p.Add(x => x.Initials, "AB"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAvatar_Render_WithSize()
    {
        var cut = Render<XFAvatar>(p => p.Add(x => x.Size, XFAvatarSize.Lg));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAvatar_Render_WithShape()
    {
        var cut = Render<XFAvatar>(p => p.Add(x => x.Shape, XFAvatarShape.Square));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBadge_Render_WithText()
    {
        var cut = Render<XFBadge>(p => p.Add(x => x.Text, "New"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBadge_Render_WithVariant()
    {
        var cut = Render<XFBadge>(p => p.Add(x => x.Variant, XFBadgeVariant.Solid));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBadge_Render_WithTone()
    {
        var cut = Render<XFBadge>(p => p.Add(x => x.Color, XFBadgeColor.Success));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBadge_Render_WithDot()
    {
        var cut = Render<XFBadge>(p => p.Add(x => x.Dot, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFChip_Render_WithText()
    {
        var cut = Render<XFChip>(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Chip"))));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFChip_Render_WithVariant()
    {
        var cut = Render<XFChip>(p => p.Add(x => x.Variant, XFChipVariant.Outline));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFChip_Render_WithTone()
    {
        var cut = Render<XFChip>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFChip_Render_WithDismissible()
    {
        var cut = Render<XFChip>(p => p.Add(x => x.Dismissible, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFImage_Render_WithSrc()
    {
        var cut = Render<XFImage>(p => p.Add(x => x.Src, "photo.jpg"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFImage_Render_WithAlt()
    {
        var cut = Render<XFImage>(p => p.Add(x => x.Alt, "Alt text"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFImage_Render_WithLazy()
    {
        var cut = Render<XFImage>(p => p.Add(x => x.Lazy, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFImage_Render_WithFit()
    {
        var cut = Render<XFImage>(p => p.Add(x => x.Fit, XFImageFit.Contain));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFFigure_Render_WithCaption()
    {
        var cut = Render<XFFigure>(p => p.Add(x => x.Caption, "Fig. 1"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCode_Render_WithText()
    {
        var cut = Render<XFCode>(p => p.Add(x => x.Text, "var x = 1;"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCode_Render_WithLanguage()
    {
        var cut = Render<XFCode>(p => p.Add(x => x.Language, "csharp"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCode_Render_Inline()
    {
        var cut = Render<XFCode>(p => p.Add(x => x.Variant, XFCodeVariant.Inline));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFGauge_Render_WithValue()
    {
        var cut = Render<XFGauge>(p => p.Add(x => x.Value, 75));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFGauge_Render_WithMinMax()
    {
        var cut = Render<XFGauge>(p => { p.Add(x => x.Min, 0); p.Add(x => x.Max, 200); });
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFGauge_Render_WithSize()
    {
        var cut = Render<XFGauge>(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCarousel_Render_WithAutoplay()
    {
        var cut = Render<XFCarousel>(p => p.Add(x => x.Autoplay, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCarousel_Render_WithInterval()
    {
        var cut = Render<XFCarousel>(p => p.Add(x => x.Interval, 3000));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCarousel_Render_WithIndicators()
    {
        var cut = Render<XFCarousel>(p => p.Add(x => x.ShowIndicators, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCarousel_Render_WithControls()
    {
        var cut = Render<XFCarousel>(p => p.Add(x => x.ShowControls, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTimeline_Render_WithOrientation()
    {
        var cut = Render<XFTimeline>(p => p.Add(x => x.Orientation, XFOrientation.Horizontal));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFVirtualList_Render()
    {
        var cut = Render<XFVirtualList<string>>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFList_Render()
    {
        var cut = Render<XFList>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTable_Render()
    {
        var cut = Render<XFTable>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFListItem_Render_WithSelected()
    {
        var cut = Render<XFListItem>(p => p.Add(x => x.Selected, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFListItem_Render_WithDisabled()
    {
        var cut = Render<XFListItem>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCard_Render_WithVariant()
    {
        var cut = Render<XFCard>(p => p.Add(x => x.Variant, XFCardVariant.Outlined));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFCard_Render_WithTone()
    {
        var cut = Render<XFCard>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFChipList_Render_WithItems()
    {
        var items = new[] { "item1", "item2" };
        var cut = Render<XFChipList>(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAvatarGroup_Render_WithOverflowCount()
    {
        var cut = Render<XFAvatarGroup>(p => p.Add(x => x.OverflowCount, 5));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAvatarGroup_Render_WithSize()
    {
        var cut = Render<XFAvatarGroup>(p => p.Add(x => x.Size, XFAvatarSize.Sm));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTreeView_Render()
    {
        var cut = Render<XFTreeView>();
        Assert.NotNull(cut);
    }
}
