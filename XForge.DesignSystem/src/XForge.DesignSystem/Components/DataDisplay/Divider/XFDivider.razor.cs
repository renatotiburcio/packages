using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Divider
{
    public partial class XFDivider : XForgeComponentBase
    {
        protected override string BaseClass => "xf-divider";

        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;
        [Parameter] public XFDividerAlign Align { get; set; } = XFDividerAlign.Center;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Neutral;
        [Parameter] public XFDividerSpacing Spacing { get; set; } = XFDividerSpacing.Md;
        [Parameter] public XFDividerThickness Thickness { get; set; } = XFDividerThickness.Thin;
        [Parameter] public bool Dashed { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string? Label { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Orientation, Align, Tone, Spacing, Thickness, Dashed, Visible, Label, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private bool HasLabel => !string.IsNullOrWhiteSpace(Label) || ChildContent is not null;

        private string AriaOrientation => Orientation == XFOrientation.Vertical ? "vertical" : "horizontal";

        protected override string GetComponentClass()
        {
            if (Orientation == XFOrientation.Vertical)
            {
                return GetClass($"{BaseClass} inline-block self-stretch {VerticalSpacingClass} {LineThicknessClass} {LineToneClass} {LineStyleClass}");
            }

            if (HasLabel)
            {
                return GetClass($"{BaseClass} flex w-full items-center {HorizontalSpacingClass}");
            }

            return GetClass($"{BaseClass} w-full {HorizontalSpacingClass} {LineThicknessClass} {LineToneClass} {LineStyleClass}");
        }

        private string HorizontalSpacingClass => Spacing switch
        {
            XFDividerSpacing.None => "my-0",
            XFDividerSpacing.Sm => "my-2",
            XFDividerSpacing.Lg => "my-6",
            XFDividerSpacing.Xl => "my-8",
            _ => "my-4"
        };

        private string VerticalSpacingClass => Spacing switch
        {
            XFDividerSpacing.None => "mx-0",
            XFDividerSpacing.Sm => "mx-2",
            XFDividerSpacing.Lg => "mx-6",
            XFDividerSpacing.Xl => "mx-8",
            _ => "mx-4"
        };

        private string LineThicknessClass => Orientation == XFOrientation.Vertical
            ? $"{VerticalThicknessClass} min-h-6"
            : HorizontalThicknessClass;

        private string HorizontalThicknessClass => Thickness switch
        {
            XFDividerThickness.Md => "h-0.5",
            XFDividerThickness.Lg => "h-1",
            _ => "h-px"
        };

        private string VerticalThicknessClass => Thickness switch
        {
            XFDividerThickness.Md => "w-0.5",
            XFDividerThickness.Lg => "w-1",
            _ => "w-px"
        };

        private string LineStyleClass => Dashed
            ? Orientation == XFOrientation.Vertical ? $"{VerticalBorderThicknessClass} border-dashed bg-transparent" : $"{HorizontalBorderThicknessClass} border-dashed bg-transparent"
            : "";

        private string LineToneClass => Dashed ? BorderToneClass : BackgroundToneClass;

        private string BeforeLineClass => $"{LineBasisBefore} {LabelLineClass}";

        private string AfterLineClass => $"{LineBasisAfter} {LabelLineClass}";

        private string LabelLineClass => $"{HorizontalThicknessClass} {LineToneClass} {LineStyleClass}";

        private string HorizontalBorderThicknessClass => Thickness switch
        {
            XFDividerThickness.Md => "border-t-2",
            XFDividerThickness.Lg => "border-t-4",
            _ => "border-t"
        };

        private string VerticalBorderThicknessClass => Thickness switch
        {
            XFDividerThickness.Md => "border-l-2",
            XFDividerThickness.Lg => "border-l-4",
            _ => "border-l"
        };

        private string LabelClass => "shrink-0 px-3 text-xs font-semibold uppercase tracking-wide text-text-muted";

        private string LineBasisBefore => Align switch
        {
            XFDividerAlign.Start => "w-8",
            XFDividerAlign.End => "flex-1",
            _ => "flex-1"
        };

        private string LineBasisAfter => Align switch
        {
            XFDividerAlign.Start => "flex-1",
            XFDividerAlign.End => "w-8",
            _ => "flex-1"
        };

        private string BackgroundToneClass => Tone switch
        {
            XFSemanticColor.Primary => "bg-[var(--color-primary)]",
            XFSemanticColor.Secondary => "bg-[var(--color-secondary)]",
            XFSemanticColor.Success => "bg-[var(--color-success)]",
            XFSemanticColor.Warning => "bg-[var(--color-warning)]",
            XFSemanticColor.Danger => "bg-[var(--color-danger)]",
            _ => "bg-border"
        };

        private string BorderToneClass => Tone switch
        {
            XFSemanticColor.Primary => "border-[var(--color-primary)]",
            XFSemanticColor.Secondary => "border-[var(--color-secondary)]",
            XFSemanticColor.Success => "border-[var(--color-success)]",
            XFSemanticColor.Warning => "border-[var(--color-warning)]",
            XFSemanticColor.Danger => "border-[var(--color-danger)]",
            _ => "border-border"
        };
    }
}
