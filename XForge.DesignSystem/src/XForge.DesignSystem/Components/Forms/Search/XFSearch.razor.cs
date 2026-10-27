using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Search
{
    public partial class XFSearch : XFInputComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private CancellationTokenSource? debounceCts;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; } = "Pesquisar";

        protected override void OnInitialized()
        {
            Placeholder = L("search.placeholder", Placeholder);
            ClearLabel = L("search.clear", ClearLabel);
            base.OnInitialized();
        }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public EventCallback<string?> OnSearch { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }
        [Parameter] public string? AutoComplete { get; set; } = "off";
        [Parameter] public int? MaxLength { get; set; }
        [Parameter] public int? MinLength { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public bool ClearOnEscape { get; set; } = true;
        [Parameter] public int DebounceMilliseconds { get; set; } = 300;
        [Parameter] public string ClearLabel { get; set; } = "Limpar pesquisa";

        private string? CurrentValue { get; set; }
        private string? LastParameterValue { get; set; }
        private string HelperId => $"{Id}-helper";
        private string? DescribedBy => !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass => Join("xf-search flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "text-sm font-medium leading-5 text-text-primary";
        private string FieldClass => Join(
            "group flex h-10 items-center gap-2 rounded-lg border border-surface-200 bg-bg-surface px-3.5 shadow-[0_1px_2px_rgb(15_23_42/.035)] ring-1 ring-surface-200/80 transition-all duration-200 ease-out focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
            Disabled ? "cursor-not-allowed opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)]");

        private string InputClass =>
            "min-w-0 flex-1 border-0 bg-transparent p-0 text-sm text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted [&::-webkit-search-cancel-button]:appearance-none [&::-webkit-search-decoration]:appearance-none";

        private string ClearButtonClass =>
            "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";

        protected override void OnParametersSet()
        {
            if (Value != LastParameterValue)
            {
                CurrentValue = Value;
                LastParameterValue = Value;
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            CurrentValue = args.Value?.ToString();

            if (Immediate)
            {
                await CommitValueAsync(CurrentValue, invokeChange: false);
                await InvokeSearchDebouncedAsync(CurrentValue);
            }
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            CurrentValue = args.Value?.ToString();
            await CommitValueAsync(CurrentValue, invokeChange: true);

            if (!Immediate)
            {
                await InvokeSearchDebouncedAsync(CurrentValue);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await InvokeSearchAsync(CurrentValue);
            }
            else if (ClearOnEscape && args.Key == "Escape" && !string.IsNullOrEmpty(CurrentValue))
            {
                await ClearAsync();
            }
        }

        private async Task ClearAsync()
        {
            debounceCts?.Cancel();
            CurrentValue = string.Empty;
            await CommitValueAsync(CurrentValue, invokeChange: true);
            await InvokeSearchAsync(CurrentValue);
        }

        private async Task CommitValueAsync(string? value, bool invokeChange)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }

            if (invokeChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(value);
            }
        }

        private async Task InvokeSearchDebouncedAsync(string? value)
        {
            if (DebounceMilliseconds <= 0)
            {
                await InvokeSearchAsync(value);
                return;
            }

            debounceCts?.Cancel();
            debounceCts?.Dispose();
            debounceCts = new CancellationTokenSource();
            var token = debounceCts.Token;

            try
            {
                await Task.Delay(DebounceMilliseconds, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (!token.IsCancellationRequested)
            {
                await InvokeSearchAsync(value);
            }
        }

        private Task InvokeSearchAsync(string? value)
        {
            return OnSearch.HasDelegate ? OnSearch.InvokeAsync(value) : Task.CompletedTask;
        }

        public override async ValueTask DisposeAsync()
        {
            if (debounceCts is not null)
            {
                debounceCts.Cancel();
                debounceCts.Dispose();
                debounceCts = null;
            }

            await base.DisposeAsync();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
