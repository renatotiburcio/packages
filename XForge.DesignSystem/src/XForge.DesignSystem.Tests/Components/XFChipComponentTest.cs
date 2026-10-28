using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chip;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFChipComponentTest : XForgeTestContext
{
    public XFChipComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_ShowsContent()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Acme Corp");
        });

        Assert.Contains("xf-chip", cut.Markup);
        Assert.Contains("Acme Corp", cut.Markup);
        Assert.Contains("rounded-full", cut.Markup);
    }

    [Fact]
    public void Render_WithTone_Primary_RendersPrimaryColors()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Primary");
            parameters.Add(p => p.Tone, XFSemanticColor.Primary);
        });

        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-primary-contrast", cut.Markup);
    }

    [Fact]
    public void Render_OutlineVariant_RendersBorder()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Outline");
            parameters.Add(p => p.Variant, XFChipVariant.Outline);
        });

        Assert.Contains("border border-border-light", cut.Markup);
    }

    [Fact]
    public void Render_WithSize_Lg_RendersLgClasses()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Large");
            parameters.Add(p => p.Size, XFSize.Lg);
        });

        Assert.Contains("h-9", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void Render_Dismissible_ShowsDismissButton()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Removable");
            parameters.Add(p => p.Dismissible, true);
        });

        Assert.Contains("chip.dismiss", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public async Task Click_Dismissible_FiresDismissEvent()
    {
        var dismissed = false;
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Tag");
            parameters.Add(p => p.Dismissible, true);
            parameters.Add(p => p.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true));
        });

        var dismissButton = cut.Find("button[aria-label='chip.dismiss']");
        await dismissButton.ClickAsync();

        Assert.True(dismissed);
    }

    [Fact]
    public async Task Click_Clickable_FiresClickEvent()
    {
        var clicked = false;
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Clickable");
            parameters.Add(p => p.Clickable, true);
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => clicked = true));
        });

        await cut.Find(".xf-chip").ClickAsync();

        Assert.True(clicked);
    }

    [Fact]
    public void Render_Selected_ShowsSelectedRing()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Selected");
            parameters.Add(p => p.Selected, true);
        });

        Assert.Contains("ring-2 ring-primary", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_AppliesDisabledStyles()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Disabled");
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithStartIcon_ShowsIcon()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "With Icon");
            parameters.Add(p => p.StartIcon, (RenderFragment)(b => b.AddContent(0, "★")));
        });

        Assert.Contains("★", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_Invisible_DoesNotRender()
    {
        var cut = Render<XFChip>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Hidden");
            parameters.Add(p => p.Visible, false);
        });

        Assert.DoesNotContain("Hidden", cut.Markup);
    }

    [Fact]
    public void Render_ChipList_ShowsItems()
    {
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "React", "Blazor", "Vue" });
        });

        Assert.Contains("xf-chip-list", cut.Markup);
        Assert.Contains("React", cut.Markup);
        Assert.Contains("Blazor", cut.Markup);
        Assert.Contains("Vue", cut.Markup);
    }

    [Fact]
    public void Render_ChipList_WithTitle_ShowsTitle()
    {
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Tag1" });
            parameters.Add(p => p.Title, "Frameworks");
        });

        Assert.Contains("Frameworks", cut.Markup);
    }

    [Fact]
    public void Render_ChipList_WithChildContent_RendersContent()
    {
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(b =>
            {
                b.AddMarkupContent(0, "<span>Custom chip content</span>");
            }));
        });

        Assert.Contains("Custom chip content", cut.Markup);
    }

    [Fact]
    public void XFChipTone_HasExpectedValues()
    {
        Assert.Equal(8, Enum.GetValues<XFSemanticColor>().Length);
    }

    [Fact]
    public void XFChipVariant_HasExpectedValues()
    {
        Assert.Equal(4, Enum.GetValues<XFChipVariant>().Length);
    }
}
