using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Navigation.Breadcrumb
{
    public partial class XFBreadcrumbItem : XForgeComponentBase
    {
        protected override string BaseClass => "xf-breadcrumb-item";

        [Parameter] public string? Text { get; set; }
        [Parameter] public string? Href { get; set; }
        [Parameter] public bool Current { get; set; }
        [Parameter] public RenderFragment? Icon { get; set; }
        [Parameter] public RenderFragment? Separator { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Text, Href, Current, Icon, Separator, ChildContent, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} inline-flex min-w-0 items-center gap-1");
        }

        private string LinkClass => Current
            ? "inline-flex h-8 min-w-0 max-w-48 items-center gap-1.5 truncate rounded-md px-2.5 text-sm font-medium text-text-primary"
            : "inline-flex h-8 min-w-0 max-w-48 items-center gap-1.5 truncate rounded-md px-2.5 text-sm font-medium text-text-secondary outline-none transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_40%,transparent)] focus-visible:ring-offset-2";

        private RenderFragment RenderContent() => builder =>
        {
            if (Icon is not null)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "inline-flex shrink-0 items-center justify-center");
                builder.AddContent(2, Icon);
                builder.CloseElement();
            }

            if (ChildContent is not null)
            {
                builder.AddContent(3, ChildContent);
            }
            else
            {
                builder.OpenElement(4, "span");
                builder.AddAttribute(5, "class", "truncate");
                builder.AddContent(6, Text);
                builder.CloseElement();
            }
        };
    }
}
