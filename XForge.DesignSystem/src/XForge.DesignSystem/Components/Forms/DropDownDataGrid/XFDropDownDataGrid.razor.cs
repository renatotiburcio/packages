using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.DropDownDataGrid
{
    public partial class XFDropDownDataGrid<TItem, TValue> : XFInputComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public IEnumerable<TItem>? Items { get; set; }
        [Parameter] public Func<TItem, string>? TextSelector { get; set; }
        [Parameter] public Func<TItem, TValue?>? ValueSelector { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
        [Parameter] public IReadOnlyList<XFDropDownDataGridColumn<TItem>>? Columns { get; set; }
        [Parameter] public TValue? Value { get; set; }
        [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<TValue?> Change { get; set; }
        [Parameter] public bool ShowSearch { get; set; } = true;
        [Parameter] public int MaxDropdownHeight { get; set; } = 320;
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public bool Loading { get; set; }
        [Parameter] public string ClearLabel { get; set; } = default!;
        [Parameter] public string ToggleLabel { get; set; } = default!;
        [Parameter] public string LoadingText { get; set; } = default!;
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public string ListboxLabel { get; set; } = default!;
        [Parameter] public string SearchPlaceholder { get; set; } = default!;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private FieldIdentifier? FieldIdentifierValue { get; set; }
        private bool IsOpen { get; set; }
        private int HighlightedIndex { get; set; }
        private string? SearchText { get; set; }
        private string ListboxId => $"{Id}-listbox";
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private bool HasSelection => Value is not null;
        private string AriaInvalid => HasValidationError ? "true" : "false";

        private string DisplayText
        {
            get
            {
                if (IsOpen && ShowSearch)
                {
                    return SearchText ?? string.Empty;
                }

                var selected = (Items ?? Array.Empty<TItem>()).FirstOrDefault(IsSelected);
                return selected is null ? string.Empty : GetItemText(selected);
            }
        }

        private IReadOnlyList<TItem> FilteredItems
        {
            get
            {
                var source = Items?.ToArray() ?? Array.Empty<TItem>();
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    return source;
                }

                return source
                    .Where(item => MatchesSearch(GetItemText(item), SearchText))
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
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass => Join("xf-dropdown-datagrid flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string ShellWrapClass => "relative";
        private string FieldShellClass => Join("group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out h-10 py-2 pl-3.5 pr-1.5 border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]", Disabled ? "cursor-not-allowed opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", StateClass);
        private string InputClass => "min-w-0 flex-1 border-0 bg-transparent p-0 text-sm text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted";
        private string AdornmentClass => "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";
        private string IconButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";
        private string ToggleButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-text-secondary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";
        private string ChevronClass => Join("h-4 w-4 transition-transform duration-150", IsOpen ? "rotate-180" : null);
        private string DropdownClass => "absolute left-0 top-full z-50 mt-2 w-full overflow-hidden rounded-lg border border-surface-200 bg-bg-surface shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80";
        private string StateRowClass => "px-3 py-6 text-center text-sm text-text-muted";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string StateClass => HasValidationError
            ? "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]"
            : "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("dropdownDatagrid.clear", ClearLabel);
            if (string.IsNullOrWhiteSpace(ToggleLabel)) ToggleLabel = L("dropdownDatagrid.toggle", ToggleLabel);
            if (string.IsNullOrWhiteSpace(LoadingText)) LoadingText = L("dropdownDatagrid.loading", LoadingText);
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("dropdownDatagrid.empty", EmptyText);
            if (string.IsNullOrWhiteSpace(ListboxLabel)) ListboxLabel = L("dropdownDatagrid.listbox", ListboxLabel);
            if (string.IsNullOrWhiteSpace(SearchPlaceholder)) SearchPlaceholder = L("dropdownDatagrid.search", SearchPlaceholder);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private Task HandleInputAsync(ChangeEventArgs args)
        {
            SearchText = args.Value?.ToString();
            IsOpen = true;
            HighlightedIndex = 0;
            return Task.CompletedTask;
        }

        private Task HandleFocusAsync()
        {
            if (!ReadOnly && !Disabled)
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
                if (IsOpen)
                {
                    SearchText = null;
                }
            }
            return Task.CompletedTask;
        }

        private Task HandleSearchInputAsync(ChangeEventArgs args)
        {
            SearchText = args.Value?.ToString();
            HighlightedIndex = 0;
            return Task.CompletedTask;
        }

        private Task HandleSearchKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Escape")
            {
                IsOpen = false;
            }
            return Task.CompletedTask;
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Escape")
            {
                IsOpen = false;
            }
            else if (args.Key == "ArrowDown")
            {
                IsOpen = true;
                HighlightedIndex = Math.Min(HighlightedIndex + 1, Math.Max(FilteredItems.Count - 1, 0));
            }
            else if (args.Key == "ArrowUp")
            {
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
            var value = ValueSelector is not null ? ValueSelector(item) : default;
            Value = value;
            SearchText = null;
            IsOpen = false;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private async Task ClearAsync()
        {
            Value = default;
            SearchText = null;
            IsOpen = false;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private string GetItemText(TItem item) => TextSelector?.Invoke(item) ?? item?.ToString() ?? string.Empty;

        private bool IsSelected(TItem item)
        {
            if (ValueSelector is null || Value is null)
            {
                return false;
            }

            var itemValue = ValueSelector(item);
            return EqualityComparer<TValue?>.Default.Equals(itemValue, Value);
        }

        private bool MatchesSearch(string text, string search)
        {
            return text.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private string GetRowClass(int index, bool selected)
        {
            return Join(
                "cursor-pointer transition-colors duration-100",
                selected ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]" : null,
                !selected && index == HighlightedIndex ? "bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]" : null,
                !selected && index != HighlightedIndex ? "hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]" : null);
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
