using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Upload
{
    public partial class XFUpload : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private readonly Dictionary<string, CancellationTokenSource> _activeUploads = new();

        [Inject] private HttpClient Http { get; set; } = default!;

        [Parameter] public string? Label { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Accept { get; set; }
        [Parameter] public IReadOnlyList<XFUploadItem> Items { get; set; } = Array.Empty<XFUploadItem>();
        [Parameter] public EventCallback<IReadOnlyList<XFUploadItem>> ItemsChanged { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> FilesSelected { get; set; }
        [Parameter] public EventCallback<XFUploadItem> UploadRequested { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<XFUploadItem>> UploadAllRequested { get; set; }
        [Parameter] public EventCallback<XFUploadItem> CancelRequested { get; set; }
        [Parameter] public EventCallback<XFUploadItem> RemoveRequested { get; set; }
        [Parameter] public EventCallback<XFUploadCompletedEventArgs> UploadCompleted { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public RenderFragment? Icon { get; set; }
        [Parameter] public bool Multiple { get; set; }
        [Parameter] public bool AutoUpload { get; set; }
        [Parameter] public bool ShowUploadAll { get; set; } = true;
        [Parameter] public int MaximumFileCount { get; set; } = 10;
        [Parameter] public long MaximumFileSize { get; set; } = 10 * 1024 * 1024;
        [Parameter] public string? UploadUrl { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Label, Title, Description, HelperText, Accept, Items is null ? -1 : Items.Count, ItemsChanged, FilesSelected, UploadRequested, UploadAllRequested, CancelRequested, RemoveRequested, UploadCompleted, EmptyContent, Icon, Multiple, AutoUpload, ShowUploadAll, MaximumFileCount, MaximumFileSize, UploadUrl, HttpMethod, ParameterName, MaxChunkSize, UploadText, RetryText, CancelText, RemoveText, ReadyText, UploadingText, UploadedText, ErrorText, CanceledText, SingleTitle, MultipleTitle, SingleDescriptionFormat, MultipleDescriptionFormat, FileTooLargeMessage, UploadingStatusFormat, MetadataSeparator, FullWidth, Variant, Size, Tone, ShowImagePreview, ImagePreviewSize);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
        [Parameter] public string HttpMethod { get; set; } = default!;
        [Parameter] public string ParameterName { get; set; } = default!;
        [Parameter] public int MaxChunkSize { get; set; } = 0;
        [Parameter] public string UploadText { get; set; } = default!;
        [Parameter] public string RetryText { get; set; } = default!;
        [Parameter] public string CancelText { get; set; } = default!;
        [Parameter] public string RemoveText { get; set; } = default!;
        [Parameter] public string ReadyText { get; set; } = default!;
        [Parameter] public string UploadingText { get; set; } = default!;
        [Parameter] public string UploadedText { get; set; } = default!;
        [Parameter] public string ErrorText { get; set; } = default!;
        [Parameter] public string CanceledText { get; set; } = default!;
        [Parameter] public string SingleTitle { get; set; } = default!;
        [Parameter] public string MultipleTitle { get; set; } = default!;
        [Parameter] public string SingleDescriptionFormat { get; set; } = default!;
        [Parameter] public string MultipleDescriptionFormat { get; set; } = default!;
        [Parameter] public string FileTooLargeMessage { get; set; } = default!;
        [Parameter] public string UploadingStatusFormat { get; set; } = default!;
        [Parameter] public string MetadataSeparator { get; set; } = default!;
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public IReadOnlyDictionary<string, string>? CustomHeaders { get; set; }
        [Parameter] public bool ShowImagePreview { get; set; }
        [Parameter] public int ImagePreviewSize { get; set; } = 48;

        private string ResolvedTitle => Title ?? (Multiple ? MultipleTitle : SingleTitle);
        private string ResolvedDescription => Description ?? string.Format(System.Globalization.CultureInfo.CurrentCulture, Multiple ? MultipleDescriptionFormat : SingleDescriptionFormat, FormatFileSize(MaximumFileSize));
        private bool ActionDisabled => Disabled || !Items.Any(item => item.Status is XFUploadStatus.Ready or XFUploadStatus.Error or XFUploadStatus.Canceled);
        private string HelperId => $"{Id}-helper";
        private string? DescribedBy => !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;
        private bool HasUploadUrl => !string.IsNullOrWhiteSpace(UploadUrl);

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(UploadText)) UploadText = L("upload.send", UploadText);
            if (string.IsNullOrWhiteSpace(RetryText)) RetryText = L("upload.retry", RetryText);
            if (string.IsNullOrWhiteSpace(CancelText)) CancelText = L("upload.cancel", CancelText);
            if (string.IsNullOrWhiteSpace(RemoveText)) RemoveText = L("upload.remove", RemoveText);
            if (string.IsNullOrWhiteSpace(ReadyText)) ReadyText = L("upload.ready", ReadyText);
            if (string.IsNullOrWhiteSpace(UploadingText)) UploadingText = L("upload.uploading", UploadingText);
            if (string.IsNullOrWhiteSpace(UploadedText)) UploadedText = L("upload.uploaded", UploadedText);
            if (string.IsNullOrWhiteSpace(ErrorText)) ErrorText = L("upload.error", ErrorText);
            if (string.IsNullOrWhiteSpace(CanceledText)) CanceledText = L("upload.canceled", CanceledText);
            if (string.IsNullOrWhiteSpace(SingleTitle)) SingleTitle = L("upload.single_title", SingleTitle);
            if (string.IsNullOrWhiteSpace(MultipleTitle)) MultipleTitle = L("upload.multiple_title", MultipleTitle);
            if (string.IsNullOrWhiteSpace(SingleDescriptionFormat)) SingleDescriptionFormat = L("upload.single_desc", SingleDescriptionFormat);
            if (string.IsNullOrWhiteSpace(MultipleDescriptionFormat)) MultipleDescriptionFormat = L("upload.multiple_desc", MultipleDescriptionFormat);
            if (string.IsNullOrWhiteSpace(FileTooLargeMessage)) FileTooLargeMessage = L("upload.file_too_large", FileTooLargeMessage);
            base.OnInitialized();
        }

        private string RootClass => Join("xf-upload flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "text-sm font-medium leading-5 text-text-primary";
        private string DropZoneClass => Join("group flex items-center gap-3 rounded-lg border transition-all duration-200 ease-out", SizeClass, VariantClass, ToneClass, StateClass);
        private string IconWrapClass => "inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_16%,transparent)]";
        private string TitleClass => "block truncate text-sm font-semibold text-text-primary";
        private string DescriptionClass => "block truncate text-xs text-text-secondary";
        private string ActionButtonClass => "inline-flex h-8 shrink-0 items-center rounded-md px-3 text-sm font-semibold text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] disabled:pointer-events-none disabled:text-text-muted";
        private string IconButtonClass => "inline-flex h-7 w-7 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)]";
        private string ListClass => "mt-2 space-y-2";
        private string ItemClass => "flex items-start gap-3 rounded-lg border border-border-light bg-bg-surface p-3";
        private string HelperClass => "text-xs leading-5 text-text-secondary";

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

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
            XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
            XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
            XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
            _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
        };

        private string StateClass => Disabled ? "cursor-not-allowed opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

        public async ValueTask DisposeAsync()
        {
            foreach (var cts in _activeUploads.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _activeUploads.Clear();
            GC.SuppressFinalize(this);
        }

        private async Task HandleChangeAsync(InputFileChangeEventArgs args)
        {
            var files = args.GetMultipleFiles(Multiple ? MaximumFileCount : 1);
            var nextItems = files.Select(file =>
                file.Size <= MaximumFileSize
                    ? XFUploadItem.FromFile(file)
                    : XFUploadItem.FromFile(file) with { Status = XFUploadStatus.Error, Message = FileTooLargeMessage }).ToArray();

            Items = Items.Concat(nextItems).ToArray();
            await NotifyItemsChangedAsync();

            if (FilesSelected.HasDelegate)
            {
                await FilesSelected.InvokeAsync(files);
            }

            if (AutoUpload)
            {
                await UploadAllAsync();
            }
        }

        private async Task UploadAllAsync()
        {
            var uploadable = Items.Where(item => item.Status is XFUploadStatus.Ready or XFUploadStatus.Error or XFUploadStatus.Canceled).ToArray();

            if (HasUploadUrl)
            {
                foreach (var item in uploadable)
                {
                    _ = UploadItemToUrlAsync(item);
                }
            }
            else if (UploadAllRequested.HasDelegate)
            {
                await UploadAllRequested.InvokeAsync(uploadable);
            }
        }

        private async Task UploadItemAsync(XFUploadItem item)
        {
            if (HasUploadUrl)
            {
                await UploadItemToUrlAsync(item);
            }
            else if (UploadRequested.HasDelegate)
            {
                await UploadRequested.InvokeAsync(item);
            }
        }

        private async Task UploadItemToUrlAsync(XFUploadItem item)
        {
            if (item.File is null || string.IsNullOrWhiteSpace(UploadUrl))
            {
                return;
            }

            var cts = new CancellationTokenSource();
            _activeUploads[item.Id] = cts;

            UpdateItem(item with { Status = XFUploadStatus.Uploading, Progress = 0, Message = null });

            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(item.File.OpenReadStream(MaximumFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
                content.Add(fileContent, ParameterName, item.Name);

                var request = new HttpRequestMessage(new HttpMethod(HttpMethod), UploadUrl) { Content = content };
                var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    UpdateItem(item with { Status = XFUploadStatus.Uploaded, Progress = 100, Message = null });
                    await RaiseUploadCompletedAsync(item, true, null);
                }
                else
                {
                    var error = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    UpdateItem(item with { Status = XFUploadStatus.Error, Progress = 0, Message = error });
                    await RaiseUploadCompletedAsync(item, false, error);
                }
            }
            catch (OperationCanceledException)
            {
                UpdateItem(item with { Status = XFUploadStatus.Canceled, Progress = 0, Message = CanceledText });
            }
            catch (Exception ex)
            {
                UpdateItem(item with { Status = XFUploadStatus.Error, Progress = 0, Message = ex.Message });
                await RaiseUploadCompletedAsync(item, false, ex.Message);
            }
            finally
            {
                _activeUploads.Remove(item.Id);
                cts.Dispose();
            }
        }

        private void UpdateItem(XFUploadItem updated)
        {
            Items = Items.Select(existing => existing.Id == updated.Id ? updated : existing).ToArray();
            _ = NotifyItemsChangedAsync();
        }

        private async Task RaiseUploadCompletedAsync(XFUploadItem item, bool success, string? error)
        {
            if (UploadCompleted.HasDelegate)
            {
                await UploadCompleted.InvokeAsync(new XFUploadCompletedEventArgs(item, success, error));
            }
        }

        private async Task CancelItemAsync(XFUploadItem item)
        {
            if (_activeUploads.TryGetValue(item.Id, out var cts))
            {
                await cts.CancelAsync();
            }
            else if (CancelRequested.HasDelegate)
            {
                await CancelRequested.InvokeAsync(item);
            }
        }

        private async Task RemoveItemAsync(XFUploadItem item)
        {
            if (_activeUploads.TryGetValue(item.Id, out var cts))
            {
                await cts.CancelAsync();
                _activeUploads.Remove(item.Id);
            }

            Items = Items.Where(existing => existing.Id != item.Id).ToArray();
            await NotifyItemsChangedAsync();
            if (RemoveRequested.HasDelegate)
            {
                await RemoveRequested.InvokeAsync(item);
            }
        }

        private Task NotifyItemsChangedAsync() => ItemsChanged.HasDelegate ? ItemsChanged.InvokeAsync(Items) : Task.CompletedTask;

        private string GetStatusClass(XFUploadStatus status) => status switch
        {
            XFUploadStatus.Uploaded => "rounded-md bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] px-2 py-1 text-xs font-medium text-success",
            XFUploadStatus.Uploading => "rounded-md bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] px-2 py-1 text-xs font-medium text-primary",
            XFUploadStatus.Error => "rounded-md bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] px-2 py-1 text-xs font-medium text-danger",
            XFUploadStatus.Canceled => "rounded-md bg-surface-100 px-2 py-1 text-xs font-medium text-text-secondary",
            _ => "rounded-md bg-surface-100 px-2 py-1 text-xs font-medium text-text-secondary"
        };

        private string GetProgressClass(XFUploadStatus status) => status switch
        {
            XFUploadStatus.Uploaded => "h-full rounded-full bg-success transition-all duration-200",
            XFUploadStatus.Error => "h-full rounded-full bg-danger transition-all duration-200",
            XFUploadStatus.Canceled => "h-full rounded-full bg-surface-400 transition-all duration-200",
            _ => "h-full rounded-full bg-primary transition-all duration-200"
        };

        private string GetProgressStyle(int progress) => $"width: {ClampProgress(progress)}%";
        private static int ClampProgress(int progress) => Math.Clamp(progress, 0, 100);

        private string GetStatusText(XFUploadItem item) => item.Status switch
        {
            XFUploadStatus.Uploading => string.Format(System.Globalization.CultureInfo.CurrentCulture, UploadingStatusFormat, UploadingText, ClampProgress(item.Progress)),
            XFUploadStatus.Uploaded => UploadedText,
            XFUploadStatus.Error => ErrorText,
            XFUploadStatus.Canceled => CanceledText,
            _ => ReadyText
        };

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            var kb = bytes / 1024d;
            if (kb < 1024) return $"{kb:0.#} KB";
            return $"{kb / 1024d:0.#} MB";
        }

        private string FormatFileMetadata(XFUploadItem item)
        {
            var size = FormatFileSize(item.Size);
            return string.IsNullOrWhiteSpace(item.ContentType)
                ? size
                : $"{size}{MetadataSeparator}{item.ContentType}";
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
