#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Utilities.DragDrop;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFDragDropZoneComponentTest : Bunit.BunitContext
{
    public XFDragDropZoneComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFDragDropZone<string>>();
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel()
    {
        var cut = Render<XFDragDropZone<string>>(parameters =>
        {
            parameters.Add(p => p.Label, "Drop files here");
        });
        Assert.Contains("Drop files here", cut.Markup);
    }

    [Fact]
    public void Render_WithDescription()
    {
        var cut = Render<XFDragDropZone<string>>(parameters =>
        {
            parameters.Add(p => p.Description, "Accepts PDF and DOCX");
        });
        Assert.Contains("Accepts PDF and DOCX", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFDragDropZone<string>>(parameters =>
        {
            parameters.AddChildContent("Custom zone content");
        });
        Assert.Contains("Custom zone content", cut.Markup);
    }

    [Fact]
    public void Render_WithTone()
    {
        var cut = Render<XFDragDropZone<string>>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
        });
        Assert.Contains("xf-drag-drop", cut.Markup);
    }
}
