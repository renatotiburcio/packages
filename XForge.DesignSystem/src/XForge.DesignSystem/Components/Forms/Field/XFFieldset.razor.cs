using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Field
{
    public partial class XFFieldset : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? LegendContent { get; set; }
        [Parameter] public string? Legend { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public bool Bordered { get; set; } = true;

        private string RootClass => Join(
            "xf-fieldset min-w-0",
            Bordered ? "rounded-lg border border-border-light bg-bg-surface p-4" : "p-0",
            Disabled ? "opacity-60" : "",
            Class);

        private string LegendClass => "text-sm font-semibold text-text-primary";
        private string DescriptionClass => "text-sm leading-6 text-text-secondary";
        private string ContentClass => "mt-4 grid gap-4";

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
