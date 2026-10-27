using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor
{
    public partial class XFRichTextEditor : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        /// <summary>HTML content of the editor.</summary>
        [Parameter] public string? Value { get; set; }

        /// <summary>Fires when the editor content changes.</summary>
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }

        /// <summary>Value expression for EditForm integration.</summary>
        [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }

        /// <summary>Fires when the editor content changes (alias).</summary>
        [Parameter] public EventCallback<string?> Change { get; set; }

        /// <summary>Makes the editor read-only.</summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>Placeholder text for empty editor.</summary>
        [Parameter] public string? Placeholder { get; set; }

        /// <summary>Editor theme: Snow (toolbar) or Bubble (tooltip).</summary>
        [Parameter] public XFRichTextEditorTheme Theme { get; set; } = XFRichTextEditorTheme.Snow;

        /// <summary>Custom toolbar RenderFragment. When null, a default toolbar is used.</summary>
        [Parameter] public RenderFragment? Toolbar { get; set; }

        /// <summary>Accessible label for the editor.</summary>
        [Parameter] public string? AriaLabel { get; set; }

        /// <summary>Label displayed above the editor.</summary>
        [Parameter] public string? Label { get; set; }

        /// <summary>Helper text displayed below the editor.</summary>
        [Parameter] public string? HelperText { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private IJSObjectReference? jsModule;
        private DotNetObjectReference<XFRichTextEditor>? dotNetReference;
        private bool initialized;
        private string? lastInitializedHtml;
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string EditorContainerId => $"{Id}-editor";
        private string ToolbarContainerId => $"{Id}-toolbar";
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass =>
            Join("xf-richtext-editor flex flex-col gap-1.5", Class);

        private string EditorWrapperClass =>
            Join(
                "rounded-lg border transition-all duration-200",
                Disabled ? "cursor-not-allowed opacity-60" : null,
                HasValidationError
                    ? "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]"
                    : "border-surface-200 focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]");

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        protected override void OnParametersSet()
        {
            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Disabled) return;

            var shouldInit = firstRender || (!initialized && jsModule is null);
            if (!shouldInit) return;

            try
            {
                jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/XForge.DesignSystem/js/xf-rich-text-editor.js");

                dotNetReference ??= DotNetObjectReference.Create(this);

                await jsModule.InvokeVoidAsync("initialize", EditorContainerId, dotNetReference, new
                {
                    theme = Theme.ToString().ToLowerInvariant(),
                    readOnly = ReadOnly,
                    placeholder = Placeholder ?? string.Empty,
                    toolbarId = Toolbar is not null ? ToolbarContainerId : null,
                    initialHtml = Value ?? string.Empty
                });

                initialized = true;
                lastInitializedHtml = Value;
            }
            catch (JSDisconnectedException) { }
        }

        /// <summary>Sets the editor content as HTML.</summary>
        public async Task SetHtmlAsync(string? html)
        {
            if (jsModule is not null)
            {
                try
                {
                    await jsModule.InvokeVoidAsync("setHtml", EditorContainerId, html ?? string.Empty);
                }
                catch (JSDisconnectedException) { }
            }
        }

        /// <summary>Gets the editor content as HTML.</summary>
        public async Task<string> GetHtmlAsync()
        {
            if (jsModule is not null)
            {
                try
                {
                    return await jsModule.InvokeAsync<string>("getHtml", EditorContainerId);
                }
                catch (JSDisconnectedException) { }
            }

            return Value ?? string.Empty;
        }

        /// <summary>Gets the editor content as plain text.</summary>
        public async Task<string> GetTextAsync()
        {
            if (jsModule is not null)
            {
                try
                {
                    return await jsModule.InvokeAsync<string>("getText", EditorContainerId);
                }
                catch (JSDisconnectedException) { }
            }

            return string.Empty;
        }

        /// <summary>Clears the editor content.</summary>
        public async Task ClearAsync()
        {
            if (jsModule is not null)
            {
                try
                {
                    await jsModule.InvokeVoidAsync("clear", EditorContainerId);
                }
                catch (JSDisconnectedException) { }
            }
        }

        /// <summary>Focuses the editor.</summary>
        public async Task FocusAsync()
        {
            if (jsModule is not null)
            {
                try
                {
                    await jsModule.InvokeVoidAsync("focus", EditorContainerId);
                }
                catch (JSDisconnectedException) { }
            }
        }

        [JSInvokable]
        public async Task OnContentChangedFromJs(string html, string text)
        {
            Value = html;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(html);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(html);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (jsModule is not null)
            {
                try
                {
                    await jsModule.InvokeVoidAsync("release", EditorContainerId);
                    await jsModule.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
            }

            dotNetReference?.Dispose();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
