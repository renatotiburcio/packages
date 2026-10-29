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

        // snapshot dos parâmetros que dirigem o render — evita re-render redundante quando o
        // componente pai re-renderiza sem mudar nada deste componente (padrão ShouldRender)
        private string? _sName, _sLabel, _sAria, _sDescription, _sCheckedText, _sUncheckedText, _sMixedText;
        private bool _sInitialized, _sReadOnly, _sRequired, _sFullWidth, _sThreeState;
        private bool? _sValue;
        private XFSize _sSize;
        private XFSemanticColor _sTone;
        private XFCheckboxLabelPosition _sLabelPosition;

        protected override bool ShouldRender()
        {
            if (!_sInitialized)
            {
                CaptureSnapshot();
                return true;
            }

            var changed = _sName != Name || _sLabel != Label || _sAria != AriaLabel || _sDescription != Description ||
                          _sCheckedText != CheckedStateText || _sUncheckedText != UncheckedStateText ||
                          _sMixedText != MixedStateText || _sReadOnly != ReadOnly || _sRequired != Required ||
                          _sFullWidth != FullWidth || _sThreeState != ThreeState || _sValue != Value ||
                          _sSize != Size || _sTone != Tone || _sLabelPosition != LabelPosition;

            if (!changed)
            {
                return false;
            }

            CaptureSnapshot();
            return true;
        }

        private void CaptureSnapshot()
        {
            _sName = Name; _sLabel = Label; _sAria = AriaLabel; _sDescription = Description;
            _sCheckedText = CheckedStateText; _sUncheckedText = UncheckedStateText; _sMixedText = MixedStateText;
            _sReadOnly = ReadOnly; _sRequired = Required; _sFullWidth = FullWidth; _sThreeState = ThreeState;
            _sValue = Value; _sSize = Size; _sTone = Tone; _sLabelPosition = LabelPosition;
            _sInitialized = true;
        }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }
    }
}
