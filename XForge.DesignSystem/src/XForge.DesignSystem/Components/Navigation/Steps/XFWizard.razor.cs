using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

using XForge.DesignSystem.Components.Layout;
namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public partial class XFWizard : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private readonly List<XFWizardStep> steps = new();
        private int maxVisitedIndex;

        protected override string BaseClass => "xf-wizard";

        [Parameter] public string? ActiveId { get; set; }
        [Parameter] public EventCallback<string> ActiveIdChanged { get; set; }
        [Parameter] public EventCallback<XFWizardStepChangeEventArgs> StepChanging { get; set; }
        [Parameter] public EventCallback<XFWizardStepChangeEventArgs> StepChanged { get; set; }

        /// <summary>Fires when the wizard finishes (last step completed). Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<XFWizardStepChangeEventArgs> OnFinished { get; set; }

        [Parameter] public Func<XFWizardStepChangeEventArgs, Task<bool>>? CanChange { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment<XFWizard>? ActionsTemplate { get; set; }
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Surface;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFWizardSequence Sequence { get; set; } = XFWizardSequence.Linear;
        [Parameter] public string AriaLabel { get; set; } = "Assistente de etapas";
        [Parameter] public string PreviousText { get; set; } = "Voltar";
        [Parameter] public string NextText { get; set; } = "Continuar";
        [Parameter] public string FinishText { get; set; } = "Finalizar";
        [Parameter] public bool HideActions { get; set; }

        public int ActiveIndex => ActiveStep is null ? -1 : steps.IndexOf(ActiveStep);

        public bool IsLastStep => ActiveIndex >= 0 && ActiveIndex == LastEnabledIndex;

        public bool CanGoPrevious => ActiveIndex > 0;

        public bool CanGoNext => ActiveIndex >= 0 && ActiveIndex < steps.Count;

        protected override void OnInitialized()
        {
            AriaLabel = L("wizard.title", AriaLabel);
            PreviousText = L("wizard.back", PreviousText);
            NextText = L("wizard.next", NextText);
            FinishText = L("wizard.finish", FinishText);
            base.OnInitialized();
        }

        internal IReadOnlyList<XFWizardStep> RegisteredSteps => steps;

        private XFWizardStep? ActiveStep => steps.FirstOrDefault(step => step.ResolvedStepId == ResolvedActiveId) ?? steps.FirstOrDefault(step => !step.Disabled);

        private string? ResolvedActiveId => !string.IsNullOrWhiteSpace(ActiveId)
            ? ActiveId
            : steps.FirstOrDefault(step => !step.Disabled)?.ResolvedStepId;

        private IReadOnlyList<XFStepItemModel> StepItems => steps.Select((step, index) => new XFStepItemModel
        {
            Id = step.ResolvedStepId,
            Label = step.Label,
            Description = step.Description,
            Disabled = step.Disabled,
            Icon = step.Icon,
            Status = GetStepStatus(step, index)
        }).ToArray();

        protected override string GetComponentClass()
        {
            var variantClass = Variant switch
            {
                XFContainerVariant.Plain => "",
                XFContainerVariant.Soft => "rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-4",
                _ => "rounded-lg border border-border-light bg-bg-surface p-4 shadow-[0_1px_2px_rgb(15_23_42/.04)]"
            };

            var layout = Orientation == XFOrientation.Vertical ? "grid gap-5 lg:grid-cols-[18rem_1fr]" : "grid gap-5";
            return GetClass($"{BaseClass} {layout} {variantClass}");
        }

        internal void Register(XFWizardStep step)
        {
            if (!steps.Contains(step))
            {
                steps.Add(step);
                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal void Unregister(XFWizardStep step)
        {
            if (steps.Remove(step))
            {
                _ = InvokeAsync(StateHasChanged);
            }
        }

        public Task NextAsync()
        {
            if (ActiveIndex < 0)
            {
                return Task.CompletedTask;
            }

            var targetIndex = steps.FindIndex(ActiveIndex + 1, step => !step.Disabled);
            return targetIndex < 0 ? Task.CompletedTask : GoToIndexAsync(targetIndex);
        }

        public Task PreviousAsync()
        {
            if (ActiveIndex <= 0)
            {
                return Task.CompletedTask;
            }

            return GoToIndexAsync(ActiveIndex - 1);
        }

        public async Task NextOrFinishAsync()
        {
            if (IsLastStep)
            {
                await OnFinished.InvokeAsync(CreateArgs(ActiveIndex, ActiveIndex));
                return;
            }

            await NextAsync();
        }

        public Task GoToStepAsync(string stepId)
        {
            var index = steps.FindIndex(step => step.ResolvedStepId == stepId);
            return index < 0 ? Task.CompletedTask : GoToIndexAsync(index);
        }

        public async Task GoToIndexAsync(int targetIndex)
        {
            if (Disabled || targetIndex < 0 || targetIndex >= steps.Count || ActiveIndex == targetIndex)
            {
                return;
            }

            var target = steps[targetIndex];
            if (target.Disabled || !IsStepReachable(targetIndex))
            {
                return;
            }

            var args = CreateArgs(ActiveIndex, targetIndex);
            await StepChanging.InvokeAsync(args);

            if (args.Cancel)
            {
                return;
            }

            if (CanChange is not null && !await CanChange(args))
            {
                return;
            }

            ActiveId = target.ResolvedStepId;
            maxVisitedIndex = Math.Max(maxVisitedIndex, targetIndex);

            await ActiveIdChanged.InvokeAsync(target.ResolvedStepId);
            await StepChanged.InvokeAsync(args);
        }

        private XFWizardStepChangeEventArgs CreateArgs(int currentIndex, int targetIndex)
        {
            var current = currentIndex >= 0 && currentIndex < steps.Count ? steps[currentIndex] : steps.First();
            var target = targetIndex >= 0 && targetIndex < steps.Count ? steps[targetIndex] : current;
            return new XFWizardStepChangeEventArgs(current.ResolvedStepId, target.ResolvedStepId, currentIndex, targetIndex);
        }

        private bool IsStepReachable(int targetIndex)
        {
            return Sequence switch
            {
                XFWizardSequence.Free => true,
                XFWizardSequence.Visited => targetIndex <= maxVisitedIndex + 1,
                _ => targetIndex <= ActiveIndex + 1
            };
        }

        private int LastEnabledIndex => steps.FindLastIndex(step => !step.Disabled);

        private XFStepStatus GetStepStatus(XFWizardStep step, int index)
        {
            if (step.Disabled)
            {
                return XFStepStatus.Disabled;
            }

            if (step.Error)
            {
                return XFStepStatus.Error;
            }

            if (step.ResolvedStepId == ActiveStep?.ResolvedStepId)
            {
                return XFStepStatus.Current;
            }

            if (step.Complete || index < ActiveIndex)
            {
                return XFStepStatus.Complete;
            }

            return XFStepStatus.Pending;
        }

        private string StepperClass => Orientation == XFOrientation.Vertical ? "min-w-0" : "min-w-0";

        private string ContentShellClass => "min-w-0 space-y-5";

        private string PanelClass => "rounded-lg border border-border-light bg-bg-surface p-4 text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]";

        private string ActionsClass => "flex flex-wrap items-center justify-end gap-2";
    }
}