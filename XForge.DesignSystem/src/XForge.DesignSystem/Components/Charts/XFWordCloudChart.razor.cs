using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Charts
{
    public partial class XFWordCloudChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<WordCloudItem> Words { get; set; } = new List<WordCloudItem>();
        [Parameter] public int MaxFontSize { get; set; } = 48;
        [Parameter] public int Width { get; set; } = 600;
        [Parameter] public int Height { get; set; } = 400;
        [Parameter] public EventCallback<WordCloudItem> OnWordClick { get; set; }
    }
}
