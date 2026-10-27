using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.OneTimeInput
{
    public partial class XFOneTimeInput : XFInputComponentBase
    {
        private readonly List<string> slotValues = new();
        private readonly List<SlotGroup> slotGroups = new();
        private string? lastParameterValue;

        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }

        /// <summary>Fires when all digits are filled. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<string> OnComplete { get; set; }


        [Parameter] public int Digits { get; set; } = 6;
        [Parameter] public string? Group { get; set; } = "3,3";
        [Parameter] public string Separator { get; set; } = "-";
        [Parameter] public string InputMode { get; set; } = "numeric";
        [Parameter] public string Pattern { get; set; } = "[0-9]*";
        [Parameter] public string AutoComplete { get; set; } = "one-time-code";
        [Parameter] public bool Mask { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public bool AutoFocusNext { get; set; } = true;
        [Parameter] public string SlotAriaLabelFormat { get; set; } = "Caractere {0} de {1}";

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private IReadOnlyList<SlotGroup> SlotGroups => slotGroups;
        private int NormalizedDigits => Math.Max(1, Digits);
        private string LabelId => $"{Id}-label";
        private string HelperId => $"{Id}-helper";
        private string? DescribedBy => !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;
        private string CurrentCombinedValue => string.Concat(slotValues);

        private string RootClass => Join("xf-one-time-input flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string GroupContainerClass => "flex flex-wrap items-center gap-2";
        private string SlotGroupClass => "flex items-center gap-1.5";
        private string SeparatorClass => "px-0.5 text-sm font-semibold text-text-muted";
        private string SlotClass => Join(
            "h-10 w-10 rounded-lg border border-surface-200 bg-bg-surface text-center text-base font-semibold text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.035)] outline-none ring-1 ring-surface-200/80 transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:text-text-muted",
            Disabled ? "opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)]");

        protected override void OnParametersSet()
        {
            EnsureSlots();
            BuildGroups();

            if (Value != lastParameterValue)
            {
                ApplyValue(Value);
                lastParameterValue = Value;
            }
        }

        private async Task HandleSlotInputAsync(int slotIndex, ChangeEventArgs args)
        {
            if (Disabled || ReadOnly || slotIndex < 0 || slotIndex >= slotValues.Count)
            {
                return;
            }

            var characters = ExtractCharacters(args.Value?.ToString());

            if (characters.Length == 0)
            {
                slotValues[slotIndex] = string.Empty;
            }
            else
            {
                var nextSlot = slotIndex;
                foreach (var character in characters)
                {
                    if (nextSlot >= slotValues.Count)
                    {
                        break;
                    }

                    slotValues[nextSlot] = character.ToString();
                    nextSlot++;
                }

                if (AutoFocusNext)
                {
                    var focusIndex = Math.Min(nextSlot, slotValues.Count - 1);
                    await FocusSlotAsync(focusIndex);
                }
            }

            await CommitAsync(invokeChange: false);
        }

        private async Task HandleSlotKeyDownAsync(int slotIndex, KeyboardEventArgs args)
        {
            if (Disabled || ReadOnly || slotIndex < 0 || slotIndex >= slotValues.Count)
            {
                return;
            }

            if (string.Equals(args.Key, "Backspace", StringComparison.OrdinalIgnoreCase))
            {
                slotValues[slotIndex] = string.Empty;
                await CommitAsync(invokeChange: true);

                if (AutoFocusNext && slotIndex > 0)
                {
                    await FocusSlotAsync(slotIndex - 1);
                }
            }
        }

        private async Task FocusSlotAsync(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slotValues.Count)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfOneTimeInput.focusSlot", GetSlotId(slotIndex));
            }
            catch (InvalidOperationException)
            {
            }
            catch (JSDisconnectedException)
            {
            }
        }

        private async Task CommitAsync(bool invokeChange)
        {
            var value = CurrentCombinedValue;
            Value = value;
            lastParameterValue = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }

            if (invokeChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(value);
            }

            if (value.Length == NormalizedDigits && OnComplete.HasDelegate)
            {
                await OnComplete.InvokeAsync(value);
            }
        }

        private void EnsureSlots()
        {
            while (slotValues.Count < NormalizedDigits)
            {
                slotValues.Add(string.Empty);
            }

            while (slotValues.Count > NormalizedDigits)
            {
                slotValues.RemoveAt(slotValues.Count - 1);
            }
        }

        private void ApplyValue(string? value)
        {
            var normalized = NormalizeValue(value);

            for (var index = 0; index < slotValues.Count; index++)
            {
                slotValues[index] = index < normalized.Length ? normalized[index].ToString() : string.Empty;
            }
        }

        private void BuildGroups()
        {
            slotGroups.Clear();

            var startIndex = 0;
            foreach (var length in ParseGroups())
            {
                slotGroups.Add(new SlotGroup(startIndex, length));
                startIndex += length;
            }
        }

        private IEnumerable<int> ParseGroups()
        {
            var remaining = NormalizedDigits;
            var parsedGroups = new List<int>();

            if (!string.IsNullOrWhiteSpace(Group))
            {
                foreach (var part in Group.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (int.TryParse(part, out var groupLength) && groupLength > 0)
                    {
                        parsedGroups.Add(groupLength);
                    }
                }
            }

            if (parsedGroups.Count == 0)
            {
                parsedGroups.Add(NormalizedDigits);
            }

            foreach (var parsedGroup in parsedGroups)
            {
                if (remaining <= 0)
                {
                    yield break;
                }

                var currentLength = Math.Min(parsedGroup, remaining);
                yield return currentLength;
                remaining -= currentLength;
            }

            if (remaining > 0)
            {
                yield return remaining;
            }
        }

        private string GetSlotId(int index) => $"{Id}-slot-{index}";

        private string GetSlotValue(int index)
        {
            if (index < 0 || index >= slotValues.Count)
            {
                return string.Empty;
            }

            return Mask && !string.IsNullOrEmpty(slotValues[index]) ? "•" : slotValues[index];
        }

        private string GetSlotAriaLabel(int index) =>
            string.Format(System.Globalization.CultureInfo.CurrentCulture, SlotAriaLabelFormat, index + 1, NormalizedDigits);

        private string NormalizeValue(string? value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Length <= NormalizedDigits ? value : value[..NormalizedDigits];
        }

        private static char[] ExtractCharacters(string? value)
        {
            return string.IsNullOrEmpty(value)
                ? Array.Empty<char>()
                : value.Where(character => !char.IsWhiteSpace(character) && character != '-').ToArray();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private readonly record struct SlotGroup(int StartIndex, int Length);
    }
}
