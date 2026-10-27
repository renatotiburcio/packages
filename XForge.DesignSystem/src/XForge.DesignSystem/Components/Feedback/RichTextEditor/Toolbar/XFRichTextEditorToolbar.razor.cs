using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorToolbar : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override string BaseClass => "xf-toolbar flex items-center gap-1 px-2 py-1 border-b border-surface-200 bg-bg-surface";
    }
}
