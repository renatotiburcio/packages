#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTimelineComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_VerticalItemsWithTimestampAndTone()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Release history");
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderItem0(builder, "Planned", "Jan 2026", "Roadmap approved.");
                RenderItem10(builder, "Released", "Feb 2026", "Feature shipped.");
            });
        });

        Assert.Contains("xf-timeline", cut.Markup);
        Assert.Contains("role=\"list\"", cut.Markup);
        Assert.Contains("aria-label=\"Release history\"", cut.Markup);
        Assert.Contains("aria-label=\"Planned, item 1 de 2\"", cut.Markup);
        Assert.Equal(2, cut.FindAll("[role=listitem]").Count);
        Assert.Contains("Planned", cut.Markup);
        Assert.Contains("Jan 2026", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Render_HorizontalAddsResponsiveLayoutClasses()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Horizontal);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderItem0(builder, "One", "Now", "First");
                RenderItem10(builder, "Two", "Next", "Second");
            });
        });

        Assert.Contains("overflow-x-auto", cut.Markup);
        Assert.Contains("min-w-64", cut.Markup);
        Assert.Contains("h-px w-full", cut.Markup);
    }

    [Fact]
    public void Render_AlternatingPlacesOppositeContent()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.Alternating, true);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderItem0(builder, "One", "Left", "First");
                RenderItem10(builder, "Two", "Right", "Second");
            });
        });

        Assert.Contains("grid-cols-[minmax(0,1fr)_auto_minmax(0,1fr)]", cut.Markup);
        Assert.Contains("col-start-3", cut.Markup);
    }

    [Fact]
    public void Render_CustomIconAndOppositeSlot()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<XFTimelineItem>(0);
                builder.AddAttribute(1, "Title", "Custom");
                builder.AddAttribute(2, "Icon", (RenderFragment)(childBuilder => childBuilder.AddContent(0, "!")));
                builder.AddAttribute(3, "OppositeContent", (RenderFragment)(childBuilder => childBuilder.AddContent(0, "Slot date")));
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder => childBuilder.AddContent(0, "Custom body")));
                builder.CloseComponent();
            });
        });

        Assert.Contains("!", cut.Markup);
        Assert.Contains("Slot date", cut.Markup);
        Assert.Contains("Custom body", cut.Markup);
    }

    [Fact]
    public void Render_DisablesConnector()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.ShowConnector, false);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderItem0(builder, "One", "Now", "First");
                RenderItem10(builder, "Two", "Next", "Second");
            });
        });

        Assert.DoesNotContain("bg-border-light", cut.Markup);
    }

    [Fact]
    public void Render_WithDefaultAndItemAriaLabelFormats_UsesLocalizedLabels()
    {
        var cut = Render<XFTimeline>(parameters =>
        {
            parameters.Add(p => p.DefaultAriaLabel, "Historico operacional");
            parameters.Add(p => p.ItemAriaLabelFormat, "{0}, evento {1} de {2}");
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderItem0(builder, "Criado", "Hoje", "Registro criado.");
                RenderItem10(builder, "Aprovado", "Amanha", "Registro aprovado.");
            });
        });

        Assert.Contains("aria-label=\"Historico operacional\"", cut.Markup);
        Assert.Contains("aria-label=\"Criado, evento 1 de 2\"", cut.Markup);
        Assert.Contains("aria-label=\"Aprovado, evento 2 de 2\"", cut.Markup);
    }

    private static void RenderItem0(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, string title, string timestamp, string text)
    {
        builder.OpenComponent<XFTimelineItem>(0);
        builder.AddAttribute(1, "Title", title);
        builder.AddAttribute(2, "Timestamp", timestamp);
        builder.AddAttribute(3, "Text", text);
        builder.CloseComponent();
    }

    private static void RenderItem10(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, string title, string timestamp, string text)
    {
        builder.OpenComponent<XFTimelineItem>(10);
        builder.AddAttribute(11, "Title", title);
        builder.AddAttribute(12, "Timestamp", timestamp);
        builder.AddAttribute(13, "Text", text);
        builder.CloseComponent();
    }
}
