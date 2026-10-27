using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Editors
{
    public partial class XFBlockEditor : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<Block> Blocks { get; set; } = new List<Block>();
        [Parameter] public IReadOnlyList<string> SlashCommands { get; set; } = new List<string> { "h1", "h2", "p", "code", "quote", "image", "video", "divider", "bullet", "numbered", "todo" };
        [Parameter] public bool ShowSlashMenu { get; set; }
        [Parameter] public bool EnableDrag { get; set; } = true;
        [Parameter] public bool EnableEmbed { get; set; } = true;
        [Parameter] public EventCallback<Block> OnBlockChanged { get; set; }
        [Parameter] public EventCallback<string> OnSlashCommand { get; set; }
    }
}
