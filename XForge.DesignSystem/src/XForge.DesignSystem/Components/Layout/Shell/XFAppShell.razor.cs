using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFAppShell : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public RenderFragment? Rail { get; set; }
        [Parameter] public RenderFragment? Sidebar { get; set; }
        [Parameter] public RenderFragment? AppBar { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }

        [Parameter] public string Theme { get; set; } = "light";
        [Parameter] public string? Brand { get; set; }
        [Parameter] public string MainId { get; set; } = "main-content";
        [Parameter] public string? MainAriaLabel { get; set; } = "Conteudo principal";
        [Parameter] public string? NavigationAriaLabel { get; set; } = "Navegacao do aplicativo";
        [Parameter] public string MainClass { get; set; } = "flex-1 overflow-y-auto overflow-x-hidden p-6 relative";
        [Parameter] public string AsideClass { get; set; } = "flex h-full shrink-0 z-50";

        /// <summary>
        /// When true, the shell fills the entire viewport (h-dvh w-screen).
        /// When false (default), the shell fills the parent container (h-full w-full).
        /// Use FullViewport="false" when nesting inside a container parent that controls sizing.
        /// </summary>
        [Parameter] public bool FullViewport { get; set; } = false;

        protected override void OnInitialized()
        {
            MainAriaLabel = L("appshell.main_content", MainAriaLabel);
            NavigationAriaLabel = L("appshell.navigation", NavigationAriaLabel);
            base.OnInitialized();
        }

        private string GetShellClass()
        {
            var sizing = FullViewport ? "h-dvh w-screen" : "h-full w-full";
            return GetClass($"flex {sizing} overflow-hidden bg-bg text-text-primary font-sans transition-theme");
        }
    }
}
