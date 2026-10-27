using Xunit;
using XForge.DesignSystem.Components.Forms.Upload;

namespace XForge.DesignSystem.Tests.Components.Forms;

/// <summary>
/// Tests for Upload types at 0% coverage: XFUploadStatus, XFUploadItem, XFUploadCompletedEventArgs.
/// Note: XFUploadItem.FromFile requires IBrowserFile (Blazor browser type), tested indirectly.
/// </summary>
public sealed class XFUploadTypesTest
{
    #region XFUploadStatus

    [Fact]
    public void XFUploadStatus_HasAllValues()
    {
        Assert.Equal(5, Enum.GetValues<XFUploadStatus>().Length);
        Assert.True(Enum.IsDefined<XFUploadStatus>(XFUploadStatus.Ready));
        Assert.True(Enum.IsDefined<XFUploadStatus>(XFUploadStatus.Uploading));
        Assert.True(Enum.IsDefined<XFUploadStatus>(XFUploadStatus.Uploaded));
        Assert.True(Enum.IsDefined<XFUploadStatus>(XFUploadStatus.Error));
        Assert.True(Enum.IsDefined<XFUploadStatus>(XFUploadStatus.Canceled));
    }

    #endregion

    #region XFUploadItem

    [Fact]
    public void XFUploadItem_Constructs_WithRequiredProperties()
    {
        var item = new XFUploadItem(
            Id: "abc123",
            Name: "report.pdf",
            Size: 1024,
            ContentType: "application/pdf"
        );
        Assert.Equal("abc123", item.Id);
        Assert.Equal("report.pdf", item.Name);
        Assert.Equal(1024, item.Size);
        Assert.Equal("application/pdf", item.ContentType);
        Assert.Equal(XFUploadStatus.Ready, item.Status);
        Assert.Equal(0, item.Progress);
        Assert.Null(item.Message);
        Assert.Null(item.File);
    }

    [Fact]
    public void XFUploadItem_WithOptionalProperties_AssignsCorrectly()
    {
        var item = new XFUploadItem(
            Id: "def456",
            Name: "image.png",
            Size: 2048,
            ContentType: "image/png",
            Status: XFUploadStatus.Uploading,
            Progress: 50,
            Message: "Uploading..."
        );
        Assert.Equal(XFUploadStatus.Uploading, item.Status);
        Assert.Equal(50, item.Progress);
        Assert.Equal("Uploading...", item.Message);
    }

    [Fact]
    public void XFUploadItem_RecordEquality_Works()
    {
        var a = new XFUploadItem("id1", "file.txt", 100, "text/plain");
        var b = new XFUploadItem("id1", "file.txt", 100, "text/plain");
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void XFUploadItem_DifferentId_NotEqual()
    {
        var a = new XFUploadItem("id1", "file.txt", 100, "text/plain");
        var b = new XFUploadItem("id2", "file.txt", 100, "text/plain");
        Assert.NotEqual(a, b);
    }

    #endregion

    #region XFUploadCompletedEventArgs

    [Fact]
    public void XFUploadCompletedEventArgs_Success_ConstructsCorrectly()
    {
        var item = new XFUploadItem("id1", "doc.pdf", 500, "application/pdf", Status: XFUploadStatus.Uploaded);
        var args = new XFUploadCompletedEventArgs(item, success: true, error: null);
        Assert.Equal(item, args.Item);
        Assert.True(args.Success);
        Assert.Null(args.Error);
    }

    [Fact]
    public void XFUploadCompletedEventArgs_Failure_HasError()
    {
        var item = new XFUploadItem("id2", "big.zip", 10_000_000, "application/zip", Status: XFUploadStatus.Error);
        var args = new XFUploadCompletedEventArgs(item, success: false, error: "File too large");
        Assert.False(args.Success);
        Assert.Equal("File too large", args.Error);
    }

    #endregion
}
