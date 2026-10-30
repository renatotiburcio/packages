using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public partial class XFWizardStep : XForgeComponentBase, IDisposable
    {
        [CascadingParameter] internal XFWizard? Parent { get; set; }

        [Parameter] public string? StepId { get; set; }
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public string? Description { get; set; }
        [Parameter] public RenderFragment? Icon { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Complete { get; set; }
        [Parameter] public bool Error { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', StepId, Label, Description, Icon, ChildContent, Complete, Error);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        internal string ResolvedStepId => string.IsNullOrWhiteSpace(StepId) ? Label : StepId;

        internal string HeaderId => $"{ResolvedStepId}-step";

        internal string PanelId => $"{ResolvedStepId}-panel";

        protected override void OnInitialized()
        {
            Parent?.Register(this);
        }

        protected override void OnParametersSet()
        {
            Parent?.Register(this);
        }

        public void Dispose()
        {
            Parent?.Unregister(this);
        }
    }
}
