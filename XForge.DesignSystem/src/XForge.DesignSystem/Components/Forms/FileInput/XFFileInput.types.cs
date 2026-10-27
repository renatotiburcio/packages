using Microsoft.AspNetCore.Components.Forms;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.FileInput
{
public enum XFFileInputStatus
    {
        Pending,
        Uploading,
        Uploaded,
        Error,
        Canceled
    }

    public sealed record XFFileInputItem(
        string Name,
        long Size,
        string ContentType,
        DateTimeOffset LastModified,
        bool IsValid,
        string? Error,
        XFFileInputStatus Status = XFFileInputStatus.Pending,
        int Progress = 0,
        string? Message = null)
    {
        public static XFFileInputItem FromFile(IBrowserFile file, long maximumFileSize, string fileTooLargeMessage = "Arquivo excede o tamanho maximo.")
        {
            var isValid = file.Size <= maximumFileSize;
            return new XFFileInputItem(
                file.Name,
                file.Size,
                file.ContentType,
                file.LastModified,
                isValid,
                isValid ? null : fileTooLargeMessage);
        }
    }
}
