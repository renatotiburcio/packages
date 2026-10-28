using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Switch
{
    public partial class XFSwitch : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? CheckedText { get; set; } = "Sim";
        [Parameter] public string? UncheckedText { get; set; } = "Nao";

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(CheckedText)) CheckedText = L("switch.on", CheckedText);
            if (string.IsNullOrWhiteSpace(UncheckedText)) UncheckedText = L("switch.off", UncheckedText);
            base.OnInitialized();
        }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Value { get; set; }
        [Parameter] public EventCallback<bool> ValueChanged { get; set; }
        [Parameter] public Expression<Func<bool>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<bool> Change { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool ShowStateText { get; set; }
        [Parameter] public bool ShowThumbIcon { get; set; } = true;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public XFSwitchLabelPosition LabelPosition { get; set; } = XFSwitchLabelPosition.End;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }
    }
}
