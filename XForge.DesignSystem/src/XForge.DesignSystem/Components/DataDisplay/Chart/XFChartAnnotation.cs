using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public sealed class XFChartAnnotation
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public XFChartAnnotationAxis Axis { get; set; } = XFChartAnnotationAxis.Y;
        public XFSemanticColor? Tone { get; set; }
    }
}
