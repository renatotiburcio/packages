using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public sealed class XFChartTrendline
    {
        public string Name { get; set; } = string.Empty;
        public int StartIndex { get; set; }
        public double StartValue { get; set; }
        public int EndIndex { get; set; }
        public double EndValue { get; set; }
        public XFSemanticColor? Tone { get; set; }
    }
}
