using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Pdf
{
    public partial class XFPdfViewerAdvanced : XForgeComponentBase
    {
        [Parameter] public int Pages { get; set; }
        [Parameter] public string SearchQuery { get; set; } = "";
        [Parameter] public bool EnableAnnotation { get; set; } = true;
        [Parameter] public bool EnableSignature { get; set; } = true;
        [Parameter] public bool EnableFormFill { get; set; } = true;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<string> OnSearch { get; set; }
        [Parameter] public EventCallback OnAnnotate { get; set; }
        [Parameter] public EventCallback OnSign { get; set; }
        [Parameter] public EventCallback OnFormFill { get; set; }
    }
}
