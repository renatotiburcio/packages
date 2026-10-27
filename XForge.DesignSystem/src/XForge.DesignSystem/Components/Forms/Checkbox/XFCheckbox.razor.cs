using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.Checkbox
{
    public partial class XFCheckbox : XForgeComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool? Value { get; set; }
        [Parameter] public EventCallback<bool?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<bool?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<bool?> Change { get; set; }
        [Parameter] public bool ThreeState { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public XFCheckboxLabelPosition LabelPosition { get; set; } = XFCheckboxLabelPosition.End;
        [Parameter] public string? CheckedStateText { get; set; }
        [Parameter] public string? UncheckedStateText { get; set; }
        [Parameter] public string? MixedStateText { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }
    }
}
