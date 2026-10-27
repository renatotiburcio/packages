using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Utilities.DragDrop;

namespace XForge.DesignSystem.Tests.Components.Utilities;

/// <summary>
/// Tests for DragDrop types at 0% coverage: XFDragDropState, XFDragDropEventArgs, XFDragDropPayloadEventArgs.
/// </summary>
public sealed class XFDragDropTypesTest
{
    #region XFDragDropState

    [Fact]
    public void XFDragDropState_HasAllValues()
    {
        Assert.Equal(3, Enum.GetValues<XFDragDropState>().Length);
        Assert.True(Enum.IsDefined<XFDragDropState>(XFDragDropState.Idle));
        Assert.True(Enum.IsDefined<XFDragDropState>(XFDragDropState.DragOver));
        Assert.True(Enum.IsDefined<XFDragDropState>(XFDragDropState.Rejected));
    }

    #endregion

    #region XFDragDropEventArgs

    [Fact]
    public void XFDragDropEventArgs_Constructs_WithValues()
    {
        var dragArgs = new DragEventArgs
        {
            DataTransfer = new DataTransfer
            {
                Types = new[] { "text/plain", "application/json" },
                Files = new[] { "file1.txt" }
            }
        };
        var args = new XFDragDropEventArgs(dragArgs, "zone-1", accepted: true);
        Assert.Equal(dragArgs, args.Source);
        Assert.Equal("zone-1", args.ZoneId);
        Assert.True(args.Accepted);
    }

    [Fact]
    public void XFDragDropEventArgs_Types_ReturnsFromSource()
    {
        var dragArgs = new DragEventArgs
        {
            DataTransfer = new DataTransfer { Types = new[] { "text/html" } }
        };
        var args = new XFDragDropEventArgs(dragArgs, "zone-2", accepted: false);
        Assert.Single(args.Types);
        Assert.Equal("text/html", args.Types[0]);
    }

    [Fact]
    public void XFDragDropEventArgs_NullDataTransfer_TypesIsEmpty()
    {
        var dragArgs = new DragEventArgs { DataTransfer = null };
        var args = new XFDragDropEventArgs(dragArgs, "zone-3", accepted: true);
        Assert.Empty(args.Types);
        Assert.Empty(args.Files);
    }

    [Fact]
    public void XFDragDropEventArgs_AcceptedFalse_IsNotAccepted()
    {
        var dragArgs = new DragEventArgs();
        var args = new XFDragDropEventArgs(dragArgs, "zone-4", accepted: false);
        Assert.False(args.Accepted);
    }

    #endregion

    #region XFDragDropPayloadEventArgs

    [Fact]
    public void XFDragDropPayloadEventArgs_Constructs_WithPayload()
    {
        var dragArgs = new DragEventArgs
        {
            DataTransfer = new DataTransfer { Types = new[] { "text/plain" } }
        };
        var source = new XFDragDropEventArgs(dragArgs, "zone-5", accepted: true);
        var payloadArgs = new XFDragDropPayloadEventArgs<string>(source, "dropped-text");
        Assert.Equal(source, payloadArgs.Source);
        Assert.Equal("dropped-text", payloadArgs.Payload);
        Assert.True(payloadArgs.Accepted);
        Assert.Equal("zone-5", payloadArgs.ZoneId);
    }

    [Fact]
    public void XFDragDropPayloadEventArgs_NullPayload_IsAllowed()
    {
        var dragArgs = new DragEventArgs();
        var source = new XFDragDropEventArgs(dragArgs, "zone-6", accepted: false);
        var payloadArgs = new XFDragDropPayloadEventArgs<int?>(source, null);
        Assert.Null(payloadArgs.Payload);
        Assert.False(payloadArgs.Accepted);
    }

    [Fact]
    public void XFDragDropPayloadEventArgs_DelegatesToSource()
    {
        var dragArgs = new DragEventArgs
        {
            DataTransfer = new DataTransfer
            {
                Types = new[] { "files" },
                Files = new[] { "img.png" }
            }
        };
        var source = new XFDragDropEventArgs(dragArgs, "zone-7", accepted: true);
        var payloadArgs = new XFDragDropPayloadEventArgs<object>(source, new object());
        Assert.Equal(source.ZoneId, payloadArgs.ZoneId);
        Assert.Equal(source.Accepted, payloadArgs.Accepted);
        Assert.Equal(source.Types, payloadArgs.Types);
        Assert.Equal(source.Files, payloadArgs.Files);
    }

    #endregion
}
