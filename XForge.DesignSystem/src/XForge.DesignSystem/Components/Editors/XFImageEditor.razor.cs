using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Editors
{
    public partial class XFImageEditor : XForgeComponentBase
    {
        [Parameter] public string Source { get; set; } = "";
        [Parameter] public int Width { get; set; } = 600;
        [Parameter] public int Height { get; set; } = 400;
        [Parameter] public bool AllowAnnotations { get; set; } = true;
        [Parameter] public EventCallback<string> OnSave { get; set; }

        private void Rotate(double deg) { }
        private void Crop() { }
    }
}
