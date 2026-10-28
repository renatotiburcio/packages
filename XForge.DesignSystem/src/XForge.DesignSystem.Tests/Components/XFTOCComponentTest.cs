using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Navigation.TOC;

namespace XForge.DesignSystem.Tests.Components;

public class XFTOCComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_UsesNavAndHashLinks()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/components/toc");

        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.AriaLabel, "Sections");
        });

        Assert.Contains("aria-label=\"Sections\"", cut.Markup);
        Assert.Contains("href=\"/components/toc#overview\"", cut.Markup);
        Assert.Contains("Overview", cut.Markup);
    }

    [Fact]
    public void Render_ActiveItemUsesAriaCurrent()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.ActiveId, "details");
        });

        Assert.Contains("aria-current=\"location\"", cut.Markup);
        Assert.Contains("aria-label=\"Details, seção atual\"", cut.Markup);
        Assert.Contains("Details", cut.Markup);
    }

    [Fact]
    public async Task Click_UpdatesActiveIdAndRaisesItemClicked()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/components/toc");

        string? active = null;
        XFTOCItem? clicked = null;
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.ActiveIdChanged, id => active = id);
            parameters.Add(p => p.ItemClicked, item => clicked = item);
        });

        await cut.Find("a[href=\"/components/toc#details\"]").ClickAsync(new MouseEventArgs());

        Assert.Equal("details", active);
        Assert.NotNull(clicked);
        Assert.Equal("Details", clicked!.Text);
    }

    [Fact]
    public void Render_HorizontalDoesNotRenderNestedChildren()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Orientation, XFTOCOrientation.Horizontal);
        });

        Assert.Contains("overflow-x-auto", cut.Markup);
        Assert.DoesNotContain("Child section", cut.Markup);
    }

    [Fact]
    public void Render_DisabledItemHasNoFocusableLink()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
        Assert.Contains("aria-label=\"Disabled, indisponível\"", cut.Markup);
        Assert.Contains("tabindex=\"-1\"", cut.Markup);
    }

    [Fact]
    public void Render_WithLocalizedStateFormats_UsesCustomItemLabels()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.ActiveId, "overview");
            parameters.Add(p => p.ActiveItemAriaLabelFormat, "{0}, selecionado");
            parameters.Add(p => p.DisabledItemAriaLabelFormat, "{0}, bloqueado");
        });

        Assert.Contains("aria-label=\"Súmario\"", cut.Markup);
        Assert.Contains("aria-label=\"Overview, selecionado\"", cut.Markup);
        Assert.Contains("aria-label=\"Disabled, bloqueado\"", cut.Markup);
    }

    [Fact]
    public void Render_MarkerDot_ShowsDotByDefault()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Marker, XFTOCMarker.Dot);
        });

        Assert.Contains("rounded-full", cut.Markup);
        Assert.Contains("bg-surface-300", cut.Markup);
    }

    [Fact]
    public void Render_MarkerNumber_ShowsNumbers()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Marker, XFTOCMarker.Number);
        });

        Assert.Contains(">1<", cut.Markup);
        Assert.Contains(">2<", cut.Markup);
        Assert.Contains(">3<", cut.Markup);
    }

    [Fact]
    public void Render_MarkerNumber_StartNumberApplied()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Marker, XFTOCMarker.Number);
            parameters.Add(p => p.StartNumber, 5);
        });

        Assert.Contains(">5<", cut.Markup);
        Assert.Contains(">6<", cut.Markup);
        Assert.Contains(">7<", cut.Markup);
    }

    [Fact]
    public void Render_MarkerNone_HidesMarker()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Marker, XFTOCMarker.None);
        });

        Assert.DoesNotContain("rounded-full", cut.Markup);
        Assert.Contains("Overview", cut.Markup);
    }

    [Fact]
    public void Render_MarkerIcon_ShowsIcons()
    {
        var items = new[]
        {
            new XFTOCItem { Id = "home", Text = "Home", Icon = "house" },
            new XFTOCItem { Id = "settings", Text = "Settings", Icon = "gear" }
        };

        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Marker, XFTOCMarker.Icon);
        });

        Assert.Contains("house", cut.Markup);
        Assert.Contains("gear", cut.Markup);
    }

    [Fact]
    public void Render_MarkerIcon_NoIconFallsBackToNothing()
    {
        var items = new[]
        {
            new XFTOCItem { Id = "no-icon", Text = "No icon" }
        };

        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Marker, XFTOCMarker.Icon);
        });

        Assert.Contains("No icon", cut.Markup);
        Assert.DoesNotContain("rounded-full", cut.Markup);
    }

    [Fact]
    public void Render_ItemTemplate_OverridesDefaultRendering()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.ItemTemplate, item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-item");
                builder.AddContent(2, $"[{item.Text}]");
                builder.CloseElement();
            });
        });

        Assert.Contains("custom-item", cut.Markup);
        Assert.Contains("[Overview]", cut.Markup);
        Assert.DoesNotContain("rounded-full", cut.Markup);
    }

    [Fact]
    public void Render_MarkerTemplate_OverridesDefaultMarker()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.MarkerTemplate, item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-marker");
                builder.AddContent(2, "*");
                builder.CloseElement();
            });
        });

        Assert.Contains("custom-marker", cut.Markup);
        Assert.Contains("*", cut.Markup);
    }

    [Fact]
    public void Render_EmptyText_WhenNoItems()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<XFTOCItem>());
            parameters.Add(p => p.EmptyText, "Nada aqui");
        });

        Assert.Contains("Nada aqui", cut.Markup);
    }

    [Fact]
    public void Render_DefaultAriaLabel_UsesSúmario()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
        });

        Assert.Contains("aria-label=\"Súmario\"", cut.Markup);
    }

    [Fact]
    public void Render_ActiveNumberMarker_HighlightsActiveNumber()
    {
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, CreateItems());
            parameters.Add(p => p.Marker, XFTOCMarker.Number);
            parameters.Add(p => p.ActiveId, "details");
        });

        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    private static IReadOnlyList<XFTOCItem> CreateItems() => new[]
    {
        new XFTOCItem { Id = "overview", Text = "Overview" },
        new XFTOCItem
        {
            Id = "details",
            Text = "Details",
            Children = new[] { new XFTOCItem { Id = "child", Text = "Child section" } }
        },
        new XFTOCItem { Id = "disabled", Text = "Disabled", Disabled = true },
    };
}
