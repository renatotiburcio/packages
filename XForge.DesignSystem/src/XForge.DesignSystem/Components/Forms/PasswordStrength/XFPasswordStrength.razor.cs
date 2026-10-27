using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.PasswordStrength
{
    public partial class XFPasswordStrength : XFInputComponentBase
    {
        private static readonly string[] DefaultBlockedPasswords =
        [
            "123456",
            "12345678",
            "123456789",
            "abc123",
            "admin",
            "letmein",
            "password",
            "password1",
            "qwerty",
            "qwerty123",
            "welcome",
            "111111"
        ];

        private readonly List<PasswordRuleState> ruleStates = new();
        private bool showPassword;
        private string? currentValue;
        private string? lastParameterValue;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; } = "Digite uma senha";
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public EventCallback<XFPasswordStrengthChangedEventArgs> StrengthChanged { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }
        [Parameter] public string AutoComplete { get; set; } = "new-password";
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; } = true;
        [Parameter] public bool ShowStrength { get; set; } = true;
        [Parameter] public bool ShowRules { get; set; } = true;
        [Parameter] public bool ShowPasswordToggle { get; set; } = true;
        [Parameter] public int MinimumLength { get; set; } = 12;
        [Parameter] public bool RequireUppercase { get; set; } = true;
        [Parameter] public bool RequireLowercase { get; set; } = true;
        [Parameter] public bool RequireNumber { get; set; } = true;
        [Parameter] public bool RequireSpecialCharacter { get; set; } = true;
        [Parameter] public bool RequireNotCommonPassword { get; set; } = true;
        [Parameter] public IEnumerable<string>? BlockedPasswords { get; set; }
        [Parameter] public string ShowPasswordLabel { get; set; } = "Mostrar senha";
        [Parameter] public string HidePasswordLabel { get; set; } = "Ocultar senha";
        [Parameter] public string StrengthCaptionText { get; set; } = "Forca";
        [Parameter] public string StrongLabel { get; set; } = "Forte";
        [Parameter] public string GoodLabel { get; set; } = "Boa";
        [Parameter] public string FairLabel { get; set; } = "Media";
        [Parameter] public string WeakLabel { get; set; } = "Fraca";
        [Parameter] public string NoneLabel { get; set; } = "Nao informada";
        [Parameter] public string MinimumLengthRuleFormat { get; set; } = "Pelo menos {0} caracteres";
        [Parameter] public string UppercaseRuleText { get; set; } = "Uma letra maiuscula";
        [Parameter] public string LowercaseRuleText { get; set; } = "Uma letra minuscula";
        [Parameter] public string NumberRuleText { get; set; } = "Um numero";
        [Parameter] public string SpecialCharacterRuleText { get; set; } = "Um caractere especial";
        [Parameter] public string NotCommonPasswordRuleText { get; set; } = "Nao usar senha comum";

        private IReadOnlyList<PasswordRuleState> RuleStates => ruleStates;
        private string? CurrentValue => currentValue;
        private string InputType => showPassword ? "text" : "password";
        private string ToggleLabel => showPassword ? HidePasswordLabel : ShowPasswordLabel;
        private string HelperId => $"{Id}-helper";
        private string RulesId => $"{Id}-rules";
        private string? DescribedBy => string.Join(" ", new[] { !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null, ShowRules ? RulesId : null }.Where(value => value is not null));

        private int Score { get; set; }
        private int PassedRules { get; set; }
        private int TotalRules { get; set; }
        private bool IsValid { get; set; }
        private XFPasswordStrengthLevel Level { get; set; }

        private string RootClass => Join("xf-password-strength flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string FieldClass => Join(
            "group flex h-10 items-center gap-2 rounded-lg border border-surface-200 bg-bg-surface px-3.5 shadow-[0_1px_2px_rgb(15_23_42/.035)] ring-1 ring-surface-200/80 transition-all duration-200 ease-out focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
            Disabled ? "cursor-not-allowed opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)]");

        private string InputClass => "min-w-0 flex-1 border-0 bg-transparent p-0 text-sm text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted";
        private string ToggleButtonClass => "inline-flex h-7 w-7 shrink-0 items-center justify-center rounded-md bg-transparent text-text-muted outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] disabled:cursor-not-allowed disabled:opacity-45";
        private string StrengthCaption => StrengthCaptionText;
        private string StrengthLabel => Level switch
        {
            XFPasswordStrengthLevel.Strong => StrongLabel,
            XFPasswordStrengthLevel.Good => GoodLabel,
            XFPasswordStrengthLevel.Fair => FairLabel,
            XFPasswordStrengthLevel.Weak => WeakLabel,
            _ => NoneLabel
        };

        private string StrengthTextClass => Level switch
        {
            XFPasswordStrengthLevel.Strong => "text-sm font-semibold text-success",
            XFPasswordStrengthLevel.Good => "text-sm font-semibold text-primary",
            XFPasswordStrengthLevel.Fair => "text-sm font-semibold text-warning",
            XFPasswordStrengthLevel.Weak => "text-sm font-semibold text-danger",
            _ => "text-sm font-semibold text-text-muted"
        };

        private string ProgressClass => Level switch
        {
            XFPasswordStrengthLevel.Strong => "h-full rounded-full bg-success transition-all duration-200",
            XFPasswordStrengthLevel.Good => "h-full rounded-full bg-primary transition-all duration-200",
            XFPasswordStrengthLevel.Fair => "h-full rounded-full bg-warning transition-all duration-200",
            XFPasswordStrengthLevel.Weak => "h-full rounded-full bg-danger transition-all duration-200",
            _ => "h-full rounded-full bg-surface-300 transition-all duration-200"
        };

        protected override void OnParametersSet()
        {
            if (Value != lastParameterValue)
            {
                currentValue = Value;
                lastParameterValue = Value;
                Evaluate(currentValue);
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            currentValue = args.Value?.ToString();
            Evaluate(currentValue);

            if (Immediate)
            {
                await CommitAsync(invokeChange: false);
            }
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            currentValue = args.Value?.ToString();
            Evaluate(currentValue);
            await CommitAsync(invokeChange: true);
        }

        private async Task CommitAsync(bool invokeChange)
        {
            Value = currentValue;
            lastParameterValue = currentValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(currentValue);
            }

            if (invokeChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(currentValue);
            }

            if (StrengthChanged.HasDelegate)
            {
                await StrengthChanged.InvokeAsync(new XFPasswordStrengthChangedEventArgs(currentValue, Level, Score, PassedRules, TotalRules, IsValid));
            }
        }

        private void Evaluate(string? value)
        {
            var password = value ?? string.Empty;
            ruleStates.Clear();

            AddRule(string.Format(System.Globalization.CultureInfo.CurrentCulture, MinimumLengthRuleFormat, MinimumLength), MinimumLength <= 0 || password.Length >= MinimumLength, 3);
            AddRule(UppercaseRuleText, !RequireUppercase || password.Any(char.IsUpper), 1, RequireUppercase);
            AddRule(LowercaseRuleText, !RequireLowercase || password.Any(char.IsLower), 1, RequireLowercase);
            AddRule(NumberRuleText, !RequireNumber || password.Any(char.IsDigit), 1, RequireNumber);
            AddRule(SpecialCharacterRuleText, !RequireSpecialCharacter || password.Any(character => char.IsPunctuation(character) || char.IsSymbol(character)), 1, RequireSpecialCharacter);
            AddRule(NotCommonPasswordRuleText, !RequireNotCommonPassword || IsNotBlocked(password), 2, RequireNotCommonPassword);

            TotalRules = ruleStates.Count;
            PassedRules = ruleStates.Count(rule => rule.IsSatisfied);

            if (string.IsNullOrWhiteSpace(password))
            {
                Score = 0;
                Level = XFPasswordStrengthLevel.None;
                IsValid = false;
                return;
            }

            var totalWeight = ruleStates.Sum(rule => rule.Weight);
            var passedWeight = ruleStates.Where(rule => rule.IsSatisfied).Sum(rule => rule.Weight);
            Score = totalWeight > 0 ? (int)Math.Round((double)passedWeight / totalWeight * 100d, MidpointRounding.AwayFromZero) : CalculateLengthOnlyScore(password);
            Level = Score switch
            {
                >= 100 => XFPasswordStrengthLevel.Strong,
                >= 75 => XFPasswordStrengthLevel.Good,
                >= 50 => XFPasswordStrengthLevel.Fair,
                _ => XFPasswordStrengthLevel.Weak
            };
            IsValid = TotalRules == 0 || ruleStates.All(rule => rule.IsSatisfied);
        }

        private void AddRule(string text, bool satisfied, int weight, bool enabled = true)
        {
            if (enabled)
            {
                ruleStates.Add(new PasswordRuleState(text, satisfied, weight));
            }
        }

        private bool IsNotBlocked(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var blocked = new HashSet<string>(DefaultBlockedPasswords, StringComparer.OrdinalIgnoreCase);
            if (BlockedPasswords is not null)
            {
                foreach (var item in BlockedPasswords.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    blocked.Add(item.Trim());
                }
            }

            return !blocked.Contains(password.Trim());
        }

        private void ToggleVisibility()
        {
            showPassword = !showPassword;
        }

        private string GetRuleIconClass(PasswordRuleState rule) =>
            rule.IsSatisfied ? "inline-flex h-4 w-4 text-success" : "inline-flex h-4 w-4 text-text-muted";

        private string GetRuleTextClass(PasswordRuleState rule) =>
            rule.IsSatisfied ? "text-text-primary" : "text-text-secondary";

        private static int CalculateLengthOnlyScore(string password)
        {
            if (password.Length >= 15) return 100;
            if (password.Length >= 12) return 75;
            if (password.Length >= 8) return 50;
            return 25;
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private sealed record PasswordRuleState(string Text, bool IsSatisfied, int Weight);
    }
}
