using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public sealed class XFStepItemModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Label { get; set; } = string.Empty;

        public string? Description { get; set; }

        public XFStepStatus Status { get; set; } = XFStepStatus.Pending;

        public bool Disabled { get; set; }

        public RenderFragment? Icon { get; set; }
    }
}
