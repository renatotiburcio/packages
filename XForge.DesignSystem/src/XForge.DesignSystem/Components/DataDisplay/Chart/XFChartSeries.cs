using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public sealed class XFChartSeries<TItem>
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<TItem>? Data { get; set; }
        public Func<TItem, string>? LabelSelector { get; set; }
        public Func<TItem, double>? ValueSelector { get; set; }
        public string? LabelProperty { get; set; }
        public string? ValueProperty { get; set; }
        public XFSemanticColor? Tone { get; set; }
    }
}
