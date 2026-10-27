using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// AI-powered onboarding wizard component. Guides users through configurable
/// steps with AI-driven interactions, progress tracking, skip support,
/// and completion callbacks.
/// </summary>
public partial class XFAiOnboardingWizard : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Wizard steps.</summary>
    [Parameter] public IReadOnlyList<XFAiOnboardingStep> Steps { get; set; } = Array.Empty<XFAiOnboardingStep>();

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string Title { get; set; } = "Configuração Inicial";

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the progress indicator.</summary>
    [Parameter] public bool ShowProgress { get; set; } = true;

    /// <summary>Back button label.</summary>
    [Parameter] public string BackLabel { get; set; } = "Voltar";

    /// <summary>Next button label.</summary>
    [Parameter] public string NextLabel { get; set; } = "Próximo";

    /// <summary>Skip button label.</summary>
    [Parameter] public string SkipLabel { get; set; } = "Pular";

    /// <summary>Finish button label.</summary>
    [Parameter] public string FinishLabel { get; set; } = "Concluir";

    /// <summary>ARIA label for the wizard.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Assistente de configuração";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Template for rendering the current step content.</summary>
    [Parameter] public RenderFragment<XFAiOnboardingStep>? StepTemplate { get; set; }

    /// <summary>Custom content rendered in each step (when StepTemplate is null).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Fired when the current step changes.</summary>
    [Parameter] public EventCallback<XFAiOnboardingStepChangedEventArgs> OnStepChanged { get; set; }

    /// <summary>Fired when the wizard completes.</summary>
    [Parameter] public EventCallback<XFAiOnboardingCompletedEventArgs> OnCompleted { get; set; }

    /// <summary>Fired when a step is skipped.</summary>
    [Parameter] public EventCallback<XFAiOnboardingStep> OnStepSkipped { get; set; }

    // -- Internal State --------------------------------------------------

    private int _currentIndex;
    private readonly List<XFAiOnboardingStep> _steps = new();
    private readonly List<XFAiOnboardingStepState> _stepStates = new();
    private DateTimeOffset _startTime;

    /// <summary>Current step (computed).</summary>
    public XFAiOnboardingStep? CurrentStep => _steps.Count > 0 && _currentIndex < _steps.Count
        ? _steps[_currentIndex]
        : null;

    /// <summary>Current step index (public read).</summary>
    public int CurrentIndex => _currentIndex;

    private string AriaBusyValue => "false";

    // -- Lifecycle -------------------------------------------------------

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_steps, Steps) && !_steps.SequenceEqual(Steps))
        {
            _steps.Clear();
            _steps.AddRange(Steps);
            _stepStates.Clear();
            _stepStates.AddRange(Steps.Select(s => s.State));
            _currentIndex = 0;
            _startTime = DateTimeOffset.UtcNow;
        }
    }

    // -- Public Methods --------------------------------------------------

    /// <summary>
    /// Navigates to a specific step by index.
    /// </summary>
    public async Task GoToStepAsync(int index)
    {
        if (index < 0 || index >= _steps.Count || index == _currentIndex)
            return;

        var previous = _currentIndex;
        _currentIndex = index;

        await OnStepChanged.InvokeAsync(new XFAiOnboardingStepChangedEventArgs
        {
            PreviousStep = previous,
            CurrentStep = index,
            WasSkipped = false
        });

        StateHasChanged();
    }

    /// <summary>
    /// Advances to the next step.
    /// </summary>
    public async Task NextStepAsync()
    {
        if (_currentIndex < _steps.Count - 1)
        {
            MarkCurrentCompleted();
            await GoToStepAsync(_currentIndex + 1);
        }
    }

    /// <summary>
    /// Returns to the previous step.
    /// </summary>
    public async Task PreviousStepAsync()
    {
        if (_currentIndex > 0)
        {
            await GoToStepAsync(_currentIndex - 1);
        }
    }

    /// <summary>
    /// Skips the current step.
    /// </summary>
    public async Task SkipStepAsync()
    {
        if (_currentIndex >= _steps.Count || !_steps[_currentIndex].CanSkip)
            return;

        MarkCurrentSkipped();
        await OnStepSkipped.InvokeAsync(_steps[_currentIndex]);

        if (_currentIndex < _steps.Count - 1)
        {
            await GoToStepAsync(_currentIndex + 1);
        }
    }

    /// <summary>
    /// Completes the onboarding wizard.
    /// </summary>
    public async Task CompleteAsync()
    {
        MarkCurrentCompleted();

        await OnCompleted.InvokeAsync(new XFAiOnboardingCompletedEventArgs
        {
            StepsCompleted = _stepStates.Count(s => s == XFAiOnboardingStepState.Completed),
            StepsSkipped = _stepStates.Count(s => s == XFAiOnboardingStepState.Skipped),
            Duration = DateTimeOffset.UtcNow - _startTime
        });

        StateHasChanged();
    }

    // -- Internal --------------------------------------------------------

    private void MarkCurrentCompleted()
    {
        if (_currentIndex < _stepStates.Count)
            _stepStates[_currentIndex] = XFAiOnboardingStepState.Completed;
    }

    private void MarkCurrentSkipped()
    {
        if (_currentIndex < _stepStates.Count)
            _stepStates[_currentIndex] = XFAiOnboardingStepState.Skipped;
    }

    // -- CSS Helpers -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-onboarding-wizard overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private static string GetStepDotClass(int index, bool isActive, bool isCompleted)
    {
        var baseClass = "inline-flex h-7 w-7 items-center justify-center rounded-full text-xs font-medium transition-colors focus-visible:ring-2 focus-visible:ring-border-focus";

        if (isCompleted)
            return $"{baseClass} bg-primary text-primary-contrast";
        if (isActive)
            return $"{baseClass} bg-primary/10 text-primary ring-2 ring-primary";

        return $"{baseClass} bg-surface-100 text-text-muted";
    }

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
