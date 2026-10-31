using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Editors
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

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Blocks.Count, SlashCommands.Count, ShowSlashMenu, EnableDrag, EnableEmbed);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
