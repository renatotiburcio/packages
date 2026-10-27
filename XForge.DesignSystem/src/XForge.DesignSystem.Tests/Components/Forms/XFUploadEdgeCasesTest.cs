using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Maps;
using XForge.DesignSystem.Components.Forms.Upload;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Tests for XFUploadStatus enum coverage and XFUploadItem record with edge cases
/// to push coverage higher on these types.
/// </summary>
public sealed class XFUploadEdgeCasesTest
{
    [Fact]
    public void XFUploadStatus_AllValues_AreAccessible()
    {
        var values = Enum.GetValues<XFUploadStatus>();
        Assert.Equal(5, values.Length);
        foreach (var status in values)
        {
            var name = Enum.GetName(status);
            Assert.NotNull(name);
            Assert.True(Enum.TryParse<XFUploadStatus>(name, out var parsed));
            Assert.Equal(status, parsed);
        }
    }

    [Fact]
    public void XFUploadItem_WithErrorStatus_HasCorrectState()
    {
        var item = new XFUploadItem(
            Id: "err-1",
            Name: "failed.doc",
            Size: 0,
            ContentType: "application/msword",
            Status: XFUploadStatus.Error,
            Progress: 0,
            Message: "Network error"
        );
        Assert.Equal(XFUploadStatus.Error, item.Status);
        Assert.Equal("Network error", item.Message);
    }

    [Fact]
    public void XFUploadItem_WithCanceledStatus()
    {
        var item = new XFUploadItem(
            Id: "cancel-1",
            Name: "cancel.xlsx",
            Size: 5000,
            ContentType: "application/vnd.ms-excel",
            Status: XFUploadStatus.Canceled
        );
        Assert.Equal(XFUploadStatus.Canceled, item.Status);
    }

    [Fact]
    public void XFUploadItem_WithUploadedStatus()
    {
        var item = new XFUploadItem(
            Id: "done-1",
            Name: "done.pdf",
            Size: 1024,
            ContentType: "application/pdf",
            Status: XFUploadStatus.Uploaded,
            Progress: 100
        );
        Assert.Equal(XFUploadStatus.Uploaded, item.Status);
        Assert.Equal(100, item.Progress);
    }
}
