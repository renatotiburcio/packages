using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Smart;

public enum HxSmartMode { TextBox, Paste, Suggest }

public abstract class HxSmartComponentBase : XForgeComponentBase
{
    [Parameter] public HxSmartMode Mode { get; set; } = HxSmartMode.TextBox;
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public EventCallback<string> OnValueChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    protected string CurrentValue { get; set; } = string.Empty;
    protected Task SetValueAsync(string v) { CurrentValue = v; return OnValueChanged.InvokeAsync(v); }
    public abstract string ComponentName { get; }
}

public class HxSmartTextBox : HxSmartComponentBase
{
    public override string ComponentName => nameof(HxSmartTextBox);
    [Parameter] public int MaxLength { get; set; } = 500;
    [Parameter] public bool UseAiCompletion { get; set; } = true;
}

public class HxSmartPaste : HxSmartComponentBase
{
    public override string ComponentName => nameof(HxSmartPaste);
    [Parameter] public string? SchemaJson { get; set; }
    [Parameter] public bool AutoFillOnPaste { get; set; } = true;
}

public class HxSmartSuggest : HxSmartComponentBase
{
    public override string ComponentName => nameof(HxSmartSuggest);
    [Parameter] public System.Collections.Generic.IEnumerable<string>? Suggestions { get; set; }
    [Parameter] public int MaxSuggestions { get; set; } = 5;
}

public static class HxSmartComponentFactory
{
    public static HxSmartComponentBase Create(HxSmartMode mode) => mode switch
    {
        HxSmartMode.TextBox => new HxSmartTextBox(),
        HxSmartMode.Paste => new HxSmartPaste(),
        HxSmartMode.Suggest => new HxSmartSuggest(),
        _ => new HxSmartTextBox()
    };
}
