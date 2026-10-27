using Microsoft.AspNetCore.Components.Forms;

namespace XForge.DesignSystem.Components.Forms.Upload
{
public enum XFUploadStatus
    {
        Ready,
        Uploading,
        Uploaded,
        Error,
        Canceled
    }

    public sealed record XFUploadItem(
        string Id,
        string Name,
        long Size,
        string ContentType,
        XFUploadStatus Status = XFUploadStatus.Ready,
        int Progress = 0,
        string? Message = null,
        IBrowserFile? File = null)
    {
        public static XFUploadItem FromFile(IBrowserFile file)
        {
            return new XFUploadItem(Guid.NewGuid().ToString("N"), file.Name, file.Size, file.ContentType, File: file);
        }
    }

    public sealed class XFUploadCompletedEventArgs
    {
        public XFUploadCompletedEventArgs(XFUploadItem item, bool success, string? error)
        {
            Item = item;
            Success = success;
            Error = error;
        }

        public XFUploadItem Item { get; }
        public bool Success { get; }
        public string? Error { get; }
    }
}
