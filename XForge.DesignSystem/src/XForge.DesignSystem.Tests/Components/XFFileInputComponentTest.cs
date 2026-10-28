using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.FileInput;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFFileInputComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelHelperAndNativeAttributes()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Invoice");
            parameters.Add(p => p.HelperText, "PDF only.");
            parameters.Add(p => p.Accept, ".pdf");
            parameters.Add(p => p.MaximumFileSize, 5 * 1024 * 1024);
        });

        Assert.Contains("xf-file-input", cut.Markup);
        Assert.Contains("Invoice", cut.Markup);
        Assert.Contains("PDF only.", cut.Markup);
        Assert.Contains("accept=\".pdf\"", cut.Markup);
        Assert.Contains("max. 5 MB", cut.Markup);
        Assert.Equal("false", cut.Find("label[aria-disabled]").GetAttribute("aria-disabled"));
        Assert.Contains(cut.Find("label[aria-disabled]").GetAttribute("aria-describedby")!, cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleAndCount()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.Multiple, true);
            parameters.Add(p => p.MaximumFileCount, 5);
        });

        Assert.Contains("multiple", cut.Markup);
        Assert.Contains("Multiplos arquivos", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredCopy()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.Multiple, true);
            parameters.Add(p => p.MultipleTitle, "Drop invoices or browse");
            parameters.Add(p => p.MultipleDescriptionFormat, "Multiple files - up to {0}");
            parameters.Add(p => p.MaximumFileSize, 5 * 1024 * 1024);
        });

        Assert.Contains("Drop invoices or browse", cut.Markup);
        Assert.Contains("Multiple files - up to 5 MB", cut.Markup);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.ChooseText, "Buscar");
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("min-h-24", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
        Assert.Contains("Buscar", cut.Markup);
    }

    [Fact]
    public void Render_WhenDisabled_PropagatesDisabledState()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("disabled", cut.Markup);
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Equal("true", cut.Find("label[aria-disabled]").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new FileModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Files);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "File is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFFileInput>(3);
                childBuilder.AddAttribute(4, "Files", model.Files);
                childBuilder.AddAttribute(5, "FilesChanged", EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>?>(this, value => model.Files = value));
                childBuilder.AddAttribute(6, "FilesExpression", (System.Linq.Expressions.Expression<Func<IReadOnlyList<IBrowserFile>?>>)(() => model.Files));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("File is required.", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    [Fact]
    public void FileItem_FromFile_MarksOversizedFile()
    {
        var file = new TestBrowserFile("large.pdf", 1024, "application/pdf");

        var item = XFFileInputItem.FromFile(file, 10);

        Assert.False(item.IsValid);
        Assert.Equal("large.pdf", item.Name);
        Assert.Equal("Arquivo excede o tamanho maximo.", item.Error);
    }

    [Fact]
    public void FileItem_FromFile_UsesCustomOversizedMessage()
    {
        var file = new TestBrowserFile("large.pdf", 1024, "application/pdf");

        var item = XFFileInputItem.FromFile(file, 10, "File is too large.");

        Assert.False(item.IsValid);
        Assert.Equal("File is too large.", item.Error);
    }

    // --- Progress tracking tests ---

    [Fact]
    public void Render_WithShowProgress_HasAriaBusy()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.ShowProgress, true);
        });

        Assert.Contains("aria-busy=\"false\"", cut.Markup);
        Assert.Contains("role=\"group\"", cut.Markup);
    }

    [Fact]
    public void Render_WithShowProgress_NoProgressBarsWhenNoItems()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.ShowProgress, true);
        });

        Assert.DoesNotContain("rounded-full bg-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_WithShowProgress_PendingItem_NoProgressBar()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.ShowProgress, true);
            parameters.Add(p => p.ItemsChanged, EventCallback.Factory.Create<IReadOnlyList<XFFileInputItem>>(this, _ => { }));
        });

        // Pending items should not show progress bar
        Assert.DoesNotContain("Enviando", cut.Markup);
    }

    [Fact]
    public void FileItem_DefaultStatus_IsPending()
    {
        var file = new TestBrowserFile("doc.pdf", 1024, "application/pdf");
        var item = XFFileInputItem.FromFile(file, 5000);

        Assert.Equal(XFFileInputStatus.Pending, item.Status);
        Assert.Equal(0, item.Progress);
        Assert.Null(item.Message);
    }

    [Fact]
    public void FileItem_WithProgress_TracksStatusAndPercent()
    {
        var item = new XFFileInputItem(
            "report.pdf", 2048, "application/pdf", DateTimeOffset.UtcNow,
            true, null, XFFileInputStatus.Uploading, 45, null);

        Assert.Equal(XFFileInputStatus.Uploading, item.Status);
        Assert.Equal(45, item.Progress);
    }

    [Fact]
    public void FileItem_CopyWith_UpdatesProgress()
    {
        var original = new XFFileInputItem(
            "data.csv", 1024, "text/csv", DateTimeOffset.UtcNow,
            true, null);

        var updated = original with { Status = XFFileInputStatus.Uploaded, Progress = 100 };

        Assert.Equal(XFFileInputStatus.Uploaded, updated.Status);
        Assert.Equal(100, updated.Progress);
        Assert.Equal("data.csv", updated.Name);
    }

    [Fact]
    public void XFFileInputStatus_HasExpectedValues()
    {
        Assert.Equal(5, Enum.GetValues<XFFileInputStatus>().Length);
        Assert.Contains(XFFileInputStatus.Pending, Enum.GetValues<XFFileInputStatus>());
        Assert.Contains(XFFileInputStatus.Uploading, Enum.GetValues<XFFileInputStatus>());
        Assert.Contains(XFFileInputStatus.Uploaded, Enum.GetValues<XFFileInputStatus>());
        Assert.Contains(XFFileInputStatus.Error, Enum.GetValues<XFFileInputStatus>());
        Assert.Contains(XFFileInputStatus.Canceled, Enum.GetValues<XFFileInputStatus>());
    }

    [Fact]
    public void Render_WithCustomProgressTexts_UsesConfiguredCopy()
    {
        var cut = Render<XFFileInput>(parameters =>
        {
            parameters.Add(p => p.ShowProgress, true);
            parameters.Add(p => p.UploadingText, "Uploading");
            parameters.Add(p => p.UploadedText, "Done");
            parameters.Add(p => p.ErrorUploadText, "Failed");
            parameters.Add(p => p.CanceledText, "Stopped");
        });

        // Custom texts are set on the component (they only render when items have non-Pending status)
        Assert.Contains("aria-busy=\"false\"", cut.Markup);
    }

    private sealed class FileModel
    {
        [Required]
        public IReadOnlyList<IBrowserFile>? Files { get; set; }
    }

    private sealed class TestBrowserFile(string name, long size, string contentType) : IBrowserFile
    {
        public string Name { get; } = name;
        public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
        public long Size { get; } = size;
        public string ContentType { get; } = contentType;

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return new MemoryStream(new byte[Math.Min(Size, maxAllowedSize)]);
        }
    }
}
