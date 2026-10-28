using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Utilities.DragDrop;

namespace XForge.DesignSystem.Tests.Components;

public class XFDragDropComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithDefaults_RendersAccessibleDropZone()
    {
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.Label, "Upload data");
            parameters.Add(p => p.Description, "Drop CSV files.");
        });

        Assert.Contains("xf-drag-drop", cut.Markup);
        Assert.Contains("role=\"button\"", cut.Markup);
        Assert.Contains("Upload data", cut.Markup);
        Assert.Contains("Drop CSV files.", cut.Markup);
        Assert.Contains("data-state=\"idle\"", cut.Markup);
    }

    [Fact]
    public void DragEnter_WhenAccepted_SetsDragOverState()
    {
        XFDragDropState? state = null;
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.AcceptTypes, new[] { "Files" });
            parameters.Add(p => p.StateChanged, EventCallback.Factory.Create<XFDragDropState>(this, value => state = value));
        });

        cut.Find(".xf-drag-drop").DragEnter(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "Files" } } });

        Assert.Equal(XFDragDropState.DragOver, state);
        Assert.Contains("data-state=\"dragover\"", cut.Markup);
    }

    [Fact]
    public void DragEnter_WhenRejected_SetsRejectedState()
    {
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.AcceptTypes, new[] { "Files" });
            parameters.Add(p => p.RejectUnknownTypes, true);
        });

        cut.Find(".xf-drag-drop").DragEnter(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "text/plain" } } });

        Assert.Contains("data-state=\"rejected\"", cut.Markup);
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void Drop_RaisesAcceptedPayloadAndResetsState()
    {
        XFDragDropEventArgs? received = null;
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.AcceptTypes, new[] { "Files" });
            parameters.Add(p => p.OnDrop, EventCallback.Factory.Create<XFDragDropEventArgs>(this, args => received = args));
        });

        var element = cut.Find(".xf-drag-drop");
        element.DragEnter(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "Files" } } });
        element.Drop(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "Files" } } });

        Assert.NotNull(received);
        Assert.True(received!.Accepted);
        Assert.Equal("Files", Assert.Single(received.Types));
        Assert.Contains("data-state=\"idle\"", cut.Markup);
    }

    [Fact]
    public void Drop_WithTypedPayload_RaisesPayloadDrop()
    {
        XFDragDropPayloadEventArgs<DropPayload>? received = null;
        var payload = new DropPayload("upload-zone", 3);
        var cut = Render<XFDragDropZone<DropPayload>>(parameters =>
        {
            parameters.Add(p => p.Payload, payload);
            parameters.Add(p => p.AcceptTypes, new[] { "Files" });
            parameters.Add(p => p.PayloadDrop, EventCallback.Factory.Create<XFDragDropPayloadEventArgs<DropPayload>>(this, args => received = args));
        });

        cut.Find(".xf-drag-drop").Drop(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "Files" } } });

        Assert.NotNull(received);
        Assert.True(received!.Accepted);
        Assert.Equal(payload, received.Payload);
        Assert.Equal("Files", Assert.Single(received.Types));
    }

    [Fact]
    public void Render_WhenDisabled_RemovesTabStopAndIgnoresDrag()
    {
        XFDragDropState? state = null;
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.StateChanged, EventCallback.Factory.Create<XFDragDropState>(this, value => state = value));
        });

        cut.Find(".xf-drag-drop").DragEnter(new DragEventArgs { DataTransfer = new DataTransfer { Types = new[] { "Files" } } });

        Assert.Null(state);
        Assert.Contains("tabindex=\"-1\"", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithDefaultLabel_UsesLocalizedFallback()
    {
        var cut = Render<XFDragDrop>(parameters =>
        {
            parameters.Add(p => p.DefaultLabel, "Arraste arquivos aqui");
        });

        Assert.Contains("Arraste arquivos aqui", cut.Markup);
        Assert.Contains("aria-labelledby", cut.Markup);
    }

    [Fact]
    public void Render_ZoneWithDefaultLabel_UsesLocalizedFallback()
    {
        var cut = Render<XFDragDropZone<DropPayload>>(parameters =>
        {
            parameters.Add(p => p.DefaultLabel, "Solte registros aqui");
            parameters.Add(p => p.Payload, new DropPayload("records", 1));
        });

        Assert.Contains("Solte registros aqui", cut.Markup);
        Assert.Contains("aria-labelledby", cut.Markup);
    }

    private sealed record DropPayload(string Zone, int Priority);
}
