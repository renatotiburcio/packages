using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.FileInput
{
    public partial class XFFileInput : XForgeComponentBase
    {
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Accept { get; set; }
        [Parameter] public string ChooseText { get; set; } = "Escolher";
        [Parameter] public IReadOnlyList<IBrowserFile>? Files { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> FilesChanged { get; set; }
        [Parameter] public Expression<Func<IReadOnlyList<IBrowserFile>?>>? FilesExpression { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> Change { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<XFFileInputItem>> ItemsChanged { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public bool Multiple { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public int MaximumFileCount { get; set; } = 10;
        [Parameter] public long MaximumFileSize { get; set; } = 10 * 1024 * 1024;
        [Parameter] public string SingleTitle { get; set; } = "Solte um arquivo aqui ou selecione";
        [Parameter] public string MultipleTitle { get; set; } = "Solte arquivos aqui ou selecione";
        [Parameter] public string SingleDescriptionFormat { get; set; } = "Arquivo unico - max. {0}";
        [Parameter] public string MultipleDescriptionFormat { get; set; } = "Multiplos arquivos - max. {0}";
        [Parameter] public string FileTooLargeMessage { get; set; } = "Arquivo excede o tamanho maximo.";
        [Parameter] public string MetadataSeparator { get; set; } = " - ";
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;

        [Inject] private IXFLocalizer? Localizer { get; set; }

        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        // Progress tracking parameters
        [Parameter] public bool ShowProgress { get; set; }
        [Parameter] public string UploadingText { get; set; } = "Enviando";
        [Parameter] public string UploadedText { get; set; } = "Enviado";
        [Parameter] public string ErrorUploadText { get; set; } = "Erro";
        [Parameter] public string CanceledText { get; set; } = "Cancelado";

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private IReadOnlyList<XFFileInputItem> Items { get; set; } = Array.Empty<XFFileInputItem>();
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string ResolvedTitle => Title ?? (Multiple ? MultipleTitle : SingleTitle);
        private string ResolvedDescription => Description ?? string.Format(System.Globalization.CultureInfo.CurrentCulture, Multiple ? MultipleDescriptionFormat : SingleDescriptionFormat, FormatFileSize(MaximumFileSize));
        private string? ResolvedHelperText => LocalError ?? HelperText;
        private string? LocalError => Items.FirstOrDefault(item => !item.IsValid)?.Error;
        private string? DescribedBy => HasValidationError ? ErrorId : !string.IsNullOrWhiteSpace(ResolvedHelperText) ? HelperId : null;
        private bool IsUploading => ShowProgress && Items.Any(i => i.Status == XFFileInputStatus.Uploading);

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && FilesExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string RootClass =>
            Join("xf-file-input flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string DropZoneClass =>
            Join(
                "group relative flex cursor-pointer items-center gap-3 rounded-lg border transition-all duration-200 ease-out",
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        private string NativeInputClass => "sr-only";

        private string IconWrapClass =>
            "inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_16%,transparent)] transition-all duration-150 group-hover:bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]";

        private string TitleClass => "block truncate text-sm font-semibold text-text-primary";

        private string DescriptionClass => "block truncate text-xs text-text-secondary";

        private string ButtonClass =>
            "inline-flex h-8 shrink-0 items-center rounded-md px-3 text-sm font-semibold text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] transition-all duration-150 group-hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]";

        private string ListClass => "mt-2 space-y-2";

        private string HelperClass => LocalError is not null
            ? "text-xs font-medium leading-5 text-danger"
            : "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-16 p-3",
            XFSize.Lg => "min-h-24 p-5",
            _ => "min-h-20 p-4"
        };

        private string VariantClass => Variant switch
        {
            XFFormVariant.Filled => "border-transparent bg-surface-100 ring-1 ring-surface-200/70 shadow-[inset_0_1px_0_rgb(255_255_255/.55)]",
            XFFormVariant.Soft => "border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)] bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
            XFFormVariant.Ghost => "border-transparent bg-transparent ring-1 ring-transparent hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]",
            _ => "border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]"
        };

        private string ToneClass
        {
            get
            {
                if (HasValidationError || LocalError is not null)
                {
                    return "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]";
                }

                return Tone switch
                {
                    XFSemanticColor.Primary => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
                    XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
                    XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
                    XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
                    XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
                    _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
                };
            }
        }

        private string StateClass => Disabled
            ? "cursor-not-allowed opacity-60"
            : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ChooseText = L("fileinput.choose", ChooseText);
            SingleTitle = L("fileinput.drop_single", SingleTitle);
            MultipleTitle = L("fileinput.drop_multiple", MultipleTitle);
            SingleDescriptionFormat = L("fileinput.max_single_format", SingleDescriptionFormat);
            MultipleDescriptionFormat = L("fileinput.max_multiple_format", MultipleDescriptionFormat);
            FileTooLargeMessage = L("fileinput.too_large", FileTooLargeMessage);
            UploadingText = L("fileinput.uploading", UploadingText);
            UploadedText = L("fileinput.uploaded", UploadedText);
            ErrorUploadText = L("fileinput.error", ErrorUploadText);
            CanceledText = L("fileinput.cancelled", CanceledText);
        }

        protected override void OnParametersSet()
        {
            if (FilesExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(FilesExpression);
            }
        }

        private async Task HandleChangeAsync(InputFileChangeEventArgs args)
        {
            var files = args.GetMultipleFiles(Multiple ? MaximumFileCount : 1);
            Files = files;
            Items = files.Select(file => XFFileInputItem.FromFile(file, MaximumFileSize, FileTooLargeMessage)).ToArray();

            if (FilesChanged.HasDelegate)
            {
                await FilesChanged.InvokeAsync(Files);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(Files);
            }

            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private string GetItemClass(XFFileInputItem item)
        {
            return Join(
                "flex items-center gap-3 rounded-lg border p-3",
                item.IsValid
                    ? "border-border-light bg-bg-surface"
                    : "border-[color-mix(in_oklab,var(--color-danger)_35%,transparent)] bg-[color-mix(in_oklab,var(--color-danger)_4%,transparent)]");
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            var kilobytes = bytes / 1024d;
            if (kilobytes < 1024)
            {
                return $"{kilobytes:0.#} KB";
            }

            var megabytes = kilobytes / 1024d;
            return $"{megabytes:0.#} MB";
        }

        private string FormatFileMetadata(XFFileInputItem item)
        {
            var size = FormatFileSize(item.Size);
            return string.IsNullOrWhiteSpace(item.ContentType)
                ? size
                : $"{size}{MetadataSeparator}{item.ContentType}";
        }

        /// <summary>Updates the status and progress of a file item by index. Call this from parent components to report upload progress.</summary>
        public void UpdateItemProgress(int index, XFFileInputStatus status, int progress, string? message = null)
        {
            if (index < 0 || index >= Items.Count)
            {
                return;
            }

            var item = Items[index];
            var updated = item with
            {
                Status = status,
                Progress = Math.Clamp(progress, 0, 100),
                Message = message
            };

            var list = Items.ToList();
            list[index] = updated;
            Items = list;

            StateHasChanged();
        }

        private int ClampedProgress(int progress) => Math.Clamp(progress, 0, 100);

        private string GetProgressClass(XFFileInputStatus status) => status switch
        {
            XFFileInputStatus.Uploading => "bg-primary",
            XFFileInputStatus.Uploaded => "bg-success",
            XFFileInputStatus.Error => "bg-danger",
            XFFileInputStatus.Canceled => "bg-surface-400",
            _ => "bg-surface-300"
        };

        private string GetProgressStyle(int progress) => $"width: {ClampedProgress(progress)}%";

        private string GetStatusText(XFFileInputItem item) => item.Status switch
        {
            XFFileInputStatus.Uploading when !string.IsNullOrWhiteSpace(item.Message) => item.Message,
            XFFileInputStatus.Uploading => $"{UploadingText} {ClampedProgress(item.Progress)}%",
            XFFileInputStatus.Uploaded => UploadedText,
            XFFileInputStatus.Error when !string.IsNullOrWhiteSpace(item.Message) => item.Message,
            XFFileInputStatus.Error => ErrorUploadText,
            XFFileInputStatus.Canceled => CanceledText,
            _ => string.Empty
        };

        private string GetStatusColorClass(XFFileInputStatus status) => status switch
        {
            XFFileInputStatus.Uploading => "text-primary",
            XFFileInputStatus.Uploaded => "text-success",
            XFFileInputStatus.Error => "text-danger",
            XFFileInputStatus.Canceled => "text-text-muted",
            _ => "text-text-secondary"
        };

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
