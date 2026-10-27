using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Editors
{
    public partial class XFDocumentEditor : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string Header { get; set; } = "";
        [Parameter] public string Footer { get; set; } = "";
        [Parameter] public int Pages { get; set; } = 1;
        [Parameter] public string Orientation { get; set; } = "portrait";
        [Parameter] public bool TrackChanges { get; set; } = false;
        [Parameter] public IReadOnlyList<string> ToolbarCommands { get; set; } = new List<string> { "Bold", "Italic", "Underline", "Strike", "H1", "H2", "P", "Table", "Image", "List" };
        [Parameter] public EventCallback<string> OnCommand { get; set; }
    }
}
