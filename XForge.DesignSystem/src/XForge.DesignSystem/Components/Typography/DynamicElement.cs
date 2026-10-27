using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Typography
{
    /// <summary>
    /// Helper component to render dynamic HTML tags from properties.
    /// This is safer and cleaner than building raw RenderTrees in the parent code-behind.
    /// </summary>
    public partial class DynamicElement : XForgeComponentBase
    {
        [Parameter]
        public string TagName { get; set; } = "div";

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, TagName);
            builder.AddMultipleAttributes(1, AdditionalAttributes);

            if (ChildContent != null)
            {
                builder.AddContent(2, ChildContent);
            }

            builder.CloseElement();
        }
    }
}
