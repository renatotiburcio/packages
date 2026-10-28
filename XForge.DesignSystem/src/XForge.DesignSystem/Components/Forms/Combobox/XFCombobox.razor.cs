using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Combobox
{
    public partial class XFCombobox<TItem, TValue> : XFInputComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private CancellationTokenSource? _searchCts;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public IEnumerable<TItem>? Items { get; set; }
        [Parameter] public Func<TItem, string>? TextSelector { get; set; }
        [Parameter] public Func<TItem, TValue?>? ValueSelector { get; set; }
        [Parameter] public Func<TItem, bool>? DisabledItemSelector { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
        [Parameter] public TValue? Value { get; set; }
        [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<TValue?> Change { get; set; }
        [Parameter] public string? Search { get; set; }
        [Parameter] public EventCallback<string?> SearchChanged { get; set; }
        [Parameter] public EventCallback<string?> SearchRequested { get; set; }
        [Parameter] public Func<string?, CancellationToken, Task<IEnumerable<TItem>>>? SearchAsync { get; set; }
        [Parameter] public int DebounceMs { get; set; } = 300;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFComboboxFilter Filter { get; set; } = XFComboboxFilter.Contains;
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool OpenOnFocus { get; set; } = true;
        [Parameter] public int MaxItems { get; set; } = 10;
        [Parameter] public string ClearLabel { get; set; } = default!;
        [Parameter] public string ToggleLabel { get; set; } = default!;
        [Parameter] public string LoadingText { get; set; } = default!;
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public string ListboxLabel { get; set; } = default!;
        [Parameter] public string? ErrorText { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private FieldIdentifier? FieldIdentifierValue { get; set; }
        private bool IsOpen { get; set; }
        private int HighlightedIndex { get; set; }
        private string? SearchText { get; set; }
        private IReadOnlyList<TItem>? _asyncItems;
        private string ListboxId => $"{Id}-listbox";
        private string? ActiveOptionId => IsOpen && FilteredItems.Count > 0 ? GetOptionId(Math.Clamp(HighlightedIndex, 0, FilteredItems.Count - 1)) : null;
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private bool HasSelection => Value is not null;
        private string AriaInvalid => HasValidationError ? "true" : "false";

        private IReadOnlyList<TItem> FilteredItems
        {
            get
            {
                if (SearchAsync is not null)
                {
                    return _asyncItems ?? Array.Empty<TItem>();
                }

                var source = Items?.ToArray() ?? Array.Empty<TItem>();
                var search = SearchText ?? string.Empty;
                if (string.IsNullOrWhiteSpace(search))
                {
                    return source.Take(Math.Max(1, MaxItems)).ToArray();
                }

                return source
                    .Where(item => MatchesSearch(GetItemText(item), search))
                    .Take(Math.Max(1, MaxItems))
                    .ToArray();
            }
        }

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
            : !string.IsNullOrWhiteSpace(ErrorText) ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass => Join("xf-combobox flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string ShellWrapClass => "relative";
        private string FieldShellClass => Join("group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out", SizeClass, VariantClass, ToneClass, StateClass);
        private string InputClass => Join("min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted", SizeTextClass);
        private string AdornmentClass => "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";
        private string IconButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";
        private string ToggleButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-text-secondary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";
        private string ChevronClass => Join("h-4 w-4 transition-transform duration-150", IsOpen ? "rotate-180" : null);
        private string ListboxClass => "absolute left-0 top-full z-50 mt-2 max-h-72 w-full overflow-auto rounded-lg border border-surface-200 bg-bg-surface p-1 shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80";
        private string StateRowClass => "px-3 py-2 text-sm text-text-secondary";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-9 py-1.5 pl-3 pr-1.5",
            XFSize.Lg => "h-12 py-2.5 pl-4 pr-2",
            _ => "h-10 py-2 pl-3.5 pr-1.5"
        };

        private string SizeTextClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
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
                if (HasValidationError)
                {
                    return "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]";
                }

                return Tone switch
                {
                    XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
                    XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
                    XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
                    XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
                    _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
                };
            }
        }

        private string StateClass => Disabled ? "cursor-not-allowed opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("combobox.clear", ClearLabel);
            if (string.IsNullOrWhiteSpace(ToggleLabel)) ToggleLabel = L("combobox.toggle", ToggleLabel);
            if (string.IsNullOrWhiteSpace(LoadingText)) LoadingText = L("combobox.loading", LoadingText);
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("combobox.no_options", EmptyText);
            if (string.IsNullOrWhiteSpace(ListboxLabel)) ListboxLabel = L("combobox.listbox", ListboxLabel);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }

            if (Search != SearchText)
            {
                SearchText = Search ?? GetSelectedText();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            await base.DisposeAsync();
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            SearchText = args.Value?.ToString();
            Search = SearchText;
            HighlightedIndex = 0;
            IsOpen = !ReadOnly && !Disabled;

            if (SearchChanged.HasDelegate)
            {
                await SearchChanged.InvokeAsync(SearchText);
            }

            if (SearchAsync is not null)
            {
                await DebouncedSearchAsync();
            }
            else if (SearchRequested.HasDelegate)
            {
                await SearchRequested.InvokeAsync(SearchText);
            }
        }

        private async Task DebouncedSearchAsync()
        {
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var cts = _searchCts;

            try
            {
                if (DebounceMs > 0)
                {
                    await Task.Delay(DebounceMs, cts.Token);
                }

                if (!cts.Token.IsCancellationRequested && SearchAsync is not null)
                {
                    Loading = true;
                    ErrorText = null;
                    StateHasChanged();

                    var results = await SearchAsync.Invoke(SearchText, cts.Token);
                    _asyncItems = results?.ToArray() ?? Array.Empty<TItem>();
                    Loading = false;
                    StateHasChanged();
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when debounce cancels — do nothing
            }
            catch (Exception ex)
            {
                if (!cts.Token.IsCancellationRequested)
                {
                    Loading = false;
                    ErrorText = ex.Message;
                    _asyncItems = Array.Empty<TItem>();
                    StateHasChanged();
                }
            }
        }

        private Task HandleFocusAsync()
        {
            if (OpenOnFocus && !ReadOnly && !Disabled)
            {
                IsOpen = true;
            }

            return Task.CompletedTask;
        }

        private Task ToggleAsync()
        {
            if (!ReadOnly && !Disabled)
            {
                IsOpen = !IsOpen;
                HighlightedIndex = Math.Clamp(HighlightedIndex, 0, Math.Max(FilteredItems.Count - 1, 0));
            }

            return Task.CompletedTask;
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Escape")
            {
                IsOpen = false;
                SearchText = GetSelectedText();
            }
            else if (args.Key == "ArrowDown")
            {
                IsOpen = true;
                HighlightedIndex = Math.Min(HighlightedIndex + 1, Math.Max(FilteredItems.Count - 1, 0));
            }
            else if (args.Key == "ArrowUp")
            {
                IsOpen = true;
                HighlightedIndex = Math.Max(HighlightedIndex - 1, 0);
            }
            else if (args.Key == "Enter" && IsOpen && FilteredItems.Count > 0)
            {
                await SelectItemAsync(FilteredItems[Math.Clamp(HighlightedIndex, 0, FilteredItems.Count - 1)]);
            }
            else if (args.Key == "Tab")
            {
                IsOpen = false;
            }
        }

        private async Task SelectItemAsync(TItem item)
        {
            if (IsDisabled(item))
            {
                return;
            }

            var value = ValueSelector is not null ? ValueSelector(item) : default;
            Value = value;
            SearchText = GetItemText(item);
            Search = SearchText;
            IsOpen = false;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (SearchChanged.HasDelegate)
            {
                await SearchChanged.InvokeAsync(SearchText);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private async Task ClearAsync()
        {
            Value = default;
            SearchText = string.Empty;
            Search = string.Empty;
            _asyncItems = null;
            IsOpen = false;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (SearchChanged.HasDelegate)
            {
                await SearchChanged.InvokeAsync(SearchText);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private string? GetSelectedText()
        {
            var selected = (Items ?? Array.Empty<TItem>()).FirstOrDefault(IsSelected);
            return selected is null ? null : GetItemText(selected);
        }

        private string GetItemText(TItem item) => TextSelector?.Invoke(item) ?? item?.ToString() ?? string.Empty;

        private bool IsSelected(TItem item)
        {
            var value = ValueSelector is not null ? ValueSelector(item) : default;
            return EqualityComparer<TValue?>.Default.Equals(value, Value);
        }

        private bool IsDisabled(TItem item) => DisabledItemSelector?.Invoke(item) == true;

        private bool MatchesSearch(string text, string search)
        {
            return Filter == XFComboboxFilter.StartsWith
                ? text.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)
                : text.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private string GetOptionClass(int index, bool selected)
        {
            var disabled = IsDisabled(FilteredItems[index]);
            return Join(
                "flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-sm outline-none transition-colors duration-150",
                disabled ? "cursor-not-allowed text-text-muted opacity-55" : null,
                selected ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary" : null,
                !selected && index == HighlightedIndex ? "bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] text-primary" : null,
                !selected && index != HighlightedIndex && !disabled ? "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary" : null);
        }

        private string GetOptionId(int index) => $"{ListboxId}-option-{index}";

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
