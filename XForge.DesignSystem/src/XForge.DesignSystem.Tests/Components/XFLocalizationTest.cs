#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Navigation.Tabs;
#endregion

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for i18n/localization defaults (S-G008).
/// Verifies all user-facing strings have PT-BR defaults via [Parameter] properties.
/// </summary>
public class XFLocalizationTest : Bunit.BunitContext
{
    #region XFTab — CloseTabLabel

    [Fact]
    public void Tab_CloseButton_HasDefaultPtBrLabel()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.AllowClose, true);
            parameters.Add(p => p.Tabs, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTab>(0);
                builder.AddAttribute(1, nameof(XFTab.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTab.Label), "Aba 1");
                builder.AddAttribute(3, nameof(XFTab.Closable), true);
                builder.CloseComponent();
            }));
            parameters.Add(p => p.Panels, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(content => content.AddContent(0, "Painel 1")));
                builder.CloseComponent();
            }));
        });

        Assert.Contains("aria-label=\"Fechar aba\"", cut.Markup);
    }

    [Fact]
    public void Tab_CloseButton_UsesCustomCloseLabel()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.AllowClose, true);
            parameters.Add(p => p.Tabs, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTab>(0);
                builder.AddAttribute(1, nameof(XFTab.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTab.Label), "Tab 1");
                builder.AddAttribute(3, nameof(XFTab.Closable), true);
                builder.AddAttribute(4, nameof(XFTab.CloseTabLabel), "Close tab");
                builder.CloseComponent();
            }));
            parameters.Add(p => p.Panels, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(content => content.AddContent(0, "Panel 1")));
                builder.CloseComponent();
            }));
        });

        Assert.Contains("aria-label=\"Close tab\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Fechar aba\"", cut.Markup);
    }

    #endregion

    #region XFPageProgressProvider — AriaLabel and LoadingText

    [Fact]
    public void PageProgress_HasDefaultPtBrAriaLabel()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(50);
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("Progresso da página", bar.GetAttribute("aria-label"));
    }

    [Fact]
    public void PageProgress_HasDefaultPtBrLoadingText()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(null); // Indeterminate
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("Carregando", bar.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void PageProgress_UsesCustomAriaLabel()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>(p =>
        {
            p.Add(x => x.AriaLabel, "Page loading");
        });

        service.Go(50);
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("Page loading", bar.GetAttribute("aria-label"));
    }

    [Fact]
    public void PageProgress_UsesCustomLoadingText()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>(p =>
        {
            p.Add(x => x.LoadingText, "Loading...");
        });

        service.Go(null); // Indeterminate
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("Loading...", bar.GetAttribute("aria-valuetext"));
    }

    #endregion

    #region XFGauge — AriaLabel fallback

    [Fact]
    public void Gauge_HasDefaultPtBrAriaLabel()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-label=\"Medidor\"", cut.Markup);
    }

    [Fact]
    public void Gauge_UsesCustomAriaLabel()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.AriaLabel, "Speedometer");
        });

        Assert.Contains("aria-label=\"Speedometer\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Medidor\"", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — AriaLabel fallback

    [Fact]
    public void LinearGauge_HasDefaultPtBrAriaLabel()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-label=\"Medidor linear\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_UsesCustomAriaLabel()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.AriaLabel, "Progress bar");
        });

        Assert.Contains("aria-label=\"Progress bar\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Medidor linear\"", cut.Markup);
    }

    #endregion

    #region XFTimelineItem — DefaultItemText fallback

    [Fact]
    public void TimelineItem_FallsBackToDefaultPtBrText()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                // Item with no Title, no Timestamp, no Text — should fallback to "Evento"
                builder.OpenComponent<XFTimelineItem>(0);
                builder.CloseComponent();
            }));
        });

        Assert.Contains("aria-label=\"Evento, item 1 de 1\"", cut.Markup);
    }

    [Fact]
    public void TimelineItem_UsesParentCustomDefaultItemText()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.DefaultItemText, "Incidente");
            parameters.Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTimelineItem>(0);
                builder.CloseComponent();
            }));
        });

        Assert.Contains("aria-label=\"Incidente, item 1 de 1\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Evento", cut.Markup);
    }

    #endregion
}
