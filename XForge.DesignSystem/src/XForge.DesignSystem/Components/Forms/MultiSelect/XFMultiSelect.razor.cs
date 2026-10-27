using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.MultiSelect
{
    public partial class XFMultiSelect<TItem, TValue> : XForgeComponentBase
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
        [Parameter] public Func<TItem, TValue>? ValueSelector { get; set; }
        [Parameter] public Func<TItem, bool>? DisabledItemSelector { get; set; }
        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
        [Parameter] public Func<TItem, string>? GroupSelector { get; set; }
        [Parameter] public RenderFragment<string>? GroupTemplate { get; set; }
        [Parameter] public IReadOnlyList<TValue>? Values { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }
        [Parameter] public Expression<Func<IReadOnlyList<TValue>?>>? ValuesExpression { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<TValue>> Change { get; set; }
        [Parameter] public string? Search { get; set; }
        [Parameter] public EventCallback<string?> SearchChanged { get; set; }
        [Parameter] public EventCallback<string?> SearchRequested { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFMultiSelectFilter Filter { get; set; } = XFMultiSelectFilter.Contains;
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool OpenOnFocus { get; set; } = true;
        [Parameter] public bool ShowBulkActions { get; set; } = true;
        [Parameter] public int MaxItems { get; set; } = 10;
        [Parameter] public int MaxVisibleChips { get; set; } = 3;
        [Parameter] public string ClearLabel { get; set; } = "Limpar";
        [Parameter] public string SelectAllLabel { get; set; } = "Selecionar todos";
        [Parameter] public string ToggleLabel { get; set; } = "Abrir opcoes";
        [Parameter] public string LoadingText { get; set; } = "Carregando...";
        [Parameter] public string EmptyText { get; set; } = "Nenhuma opcao.";
        [Parameter] public string ListboxLabel { get; set; } = "Opcoes";
        [Parameter] public string RemoveChipLabelFormat { get; set; } = "Remover {0}";

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private FieldIdentifier? FieldIdentifierValue { get; set; }
        private bool IsOpen { get; set; }
        private int HighlightedIndex { get; set; }
        private string? SearchText { get; set; }
        private string ListboxId => $"{Id}-listbox";
        private string? ActiveOptionId => IsOpen && FilteredItems.Count > 0 ? GetOptionId(Math.Clamp(HighlightedIndex, 0, FilteredItems.Count - 1)) : null;
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string? InputPlaceholder => SelectedItems.Count == 0 ? Placeholder : null;
        private string AriaInvalid => HasValidationError ? "true" : "false";

        private IReadOnlyList<TItem> SelectedItems =>
            (Items ?? Array.Empty<TItem>())
                .Where(IsSelected)
                .ToArray();

        private IReadOnlyList<TItem> FilteredItems
        {
            get
            {
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
            && ValuesExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass => Join("xf-multiselect flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string ShellWrapClass => "relative";
        private string FieldShellClass => Join("group relative flex min-h-10 items-center gap-2 rounded-lg border transition-all duration-200 ease-out", SizeClass, VariantClass, ToneClass, StateClass);
        private string ChipListClass => "flex min-w-0 shrink flex-wrap items-center gap-1";
        private string ChipClass => "inline-flex max-w-44 items-center gap-1 rounded-md bg-[color-mix(in_oklab,var(--color-primary)_9%,transparent)] px-2 py-0.5 text-xs font-medium text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_15%,transparent)]";
        private string ChipRemoveClass => "inline-flex h-4 w-4 shrink-0 items-center justify-center rounded text-primary transition-colors hover:bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]";
        private string InputClass => Join("min-w-24 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted", SizeTextClass);
        private string AdornmentClass => "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";
        private string IconButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";
        private string ToggleButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-text-secondary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";
        private string ChevronClass => Join("h-4 w-4 transition-transform duration-150", IsOpen ? "rotate-180" : null);
        private string ListboxClass => "absolute left-0 top-full z-50 mt-2 max-h-72 w-full overflow-auto rounded-lg border border-surface-200 bg-bg-surface p-1 shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80";
        private string BulkButtonClass => "rounded-md px-2 py-1 text-xs font-medium text-primary transition-colors hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]";
        private string StateRowClass => "px-3 py-2 text-sm text-text-secondary";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "py-1.5 pl-3 pr-1.5",
            XFSize.Lg => "py-2.5 pl-4 pr-2",
            _ => "py-2 pl-3.5 pr-1.5"
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
            ClearLabel = L("multiselect.clear", ClearLabel);
            SelectAllLabel = L("multiselect.select_all", SelectAllLabel);
            ToggleLabel = L("multiselect.toggle", ToggleLabel);
            LoadingText = L("multiselect.loading", LoadingText);
            EmptyText = L("multiselect.no_options", EmptyText);
            ListboxLabel = L("multiselect.listbox", ListboxLabel);
            RemoveChipLabelFormat = L("multiselect.remove_chip", RemoveChipLabelFormat);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (ValuesExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValuesExpression);
            }

            if (Search != SearchText)
            {
                SearchText = Search;
            }
        }

        private Task OpenAsync()
        {
            if (!ReadOnly && !Disabled)
            {
                IsOpen = true;
            }

            return Task.CompletedTask;
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

            if (SearchRequested.HasDelegate)
            {
                await SearchRequested.InvokeAsync(SearchText);
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
                await ToggleItemAsync(FilteredItems[Math.Clamp(HighlightedIndex, 0, FilteredItems.Count - 1)]);
            }
            else if (args.Key == "Tab")
            {
                IsOpen = false;
            }
        }

        private async Task ToggleItemAsync(TItem item)
        {
            if (IsDisabled(item))
            {
                return;
            }

            var values = Values?.ToList() ?? new List<TValue>();
            var value = GetItemValue(item);
            var existingIndex = values.FindIndex(current => EqualityComparer<TValue>.Default.Equals(current, value));
            if (existingIndex >= 0)
            {
                values.RemoveAt(existingIndex);
            }
            else
            {
                values.Add(value);
            }

            SearchText = string.Empty;
            Search = string.Empty;
            await CommitValuesAsync(values);
        }

        private async Task RemoveItemAsync(TItem item)
        {
            var value = GetItemValue(item);
            var values = (Values ?? Array.Empty<TValue>())
                .Where(current => !EqualityComparer<TValue>.Default.Equals(current, value))
                .ToArray();

            await CommitValuesAsync(values);
        }

        private async Task SelectAllAsync()
        {
            var values = FilteredItems
                .Where(item => !IsDisabled(item))
                .Select(GetItemValue)
                .Distinct()
                .ToArray();

            await CommitValuesAsync(values);
        }

        private async Task ClearAsync()
        {
            SearchText = string.Empty;
            Search = string.Empty;
            await CommitValuesAsync(Array.Empty<TValue>());
        }

        private async Task CommitValuesAsync(IReadOnlyList<TValue> values)
        {
            Values = values;

            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(values);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(values);
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

        private string GetItemText(TItem item) => TextSelector?.Invoke(item) ?? item?.ToString() ?? string.Empty;

        private string FormatRemoveChipLabel(TItem item)
        {
            return string.Format(CultureInfo.CurrentCulture, RemoveChipLabelFormat, GetItemText(item));
        }

        private TValue GetItemValue(TItem item) => ValueSelector is not null ? ValueSelector(item) : (TValue)(object)item!;

        private bool IsSelected(TItem item)
        {
            var value = GetItemValue(item);
            return (Values ?? Array.Empty<TValue>()).Any(current => EqualityComparer<TValue>.Default.Equals(current, value));
        }

        private bool IsDisabled(TItem item) => DisabledItemSelector?.Invoke(item) == true;

        private bool MatchesSearch(string text, string search)
        {
            return Filter == XFMultiSelectFilter.StartsWith
                ? text.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)
                : text.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private string CheckClass(bool selected)
        {
            return Join(
                "inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-md border transition-colors",
                selected
                    ? "border-[var(--color-primary)] bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary"
                    : "border-surface-300 bg-bg-surface text-transparent");
        }

        private string GetOptionClass(int index, bool selected)
        {
            var disabled = IsDisabled(FilteredItems[index]);
            return Join(
                "flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-sm outline-none transition-colors duration-150",
                disabled ? "cursor-not-allowed text-text-muted opacity-55" : null,
                selected ? "bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary" : null,
                !selected && index == HighlightedIndex ? "bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] text-primary" : null,
                !selected && index != HighlightedIndex && !disabled ? "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary" : null);
        }

        private string GetOptionId(int index) => $"{ListboxId}-option-{index}";

        private bool HasGroups => GroupSelector is not null;

        private string GetItemGroup(TItem item) => GroupSelector?.Invoke(item) ?? string.Empty;

        private string GroupHeaderClass => "px-3 pt-2 pb-1 text-[11px] font-semibold uppercase tracking-wide text-text-muted";

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
