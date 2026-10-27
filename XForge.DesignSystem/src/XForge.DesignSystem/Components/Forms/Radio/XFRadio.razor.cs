using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Radio
{
    public partial class XFRadio<TValue> : XForgeComponentBase, IDisposable
    {
        protected override void OnInitialized()
        {
            Group?.Register(this);
            var g = Group;
            if (g != null)
                g.SelectionChanged += OnGroupSelectionChanged;
        }

        private async void OnGroupSelectionChanged()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            var g = Group;
            if (g != null)
                g.SelectionChanged -= OnGroupSelectionChanged;
            Group?.Unregister(this);
        }
    }
}
