namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// State of an onboarding wizard step.
/// </summary>
public enum XFAiOnboardingStepState
{
    /// <summary>Step not yet reached.</summary>
    Pending,
    /// <summary>Step is currently active.</summary>
    Active,
    /// <summary>Step completed by user.</summary>
    Completed,
    /// <summary>Step skipped by user.</summary>
    Skipped
}

/// <summary>
/// A single step in the onboarding wizard.
/// </summary>
public sealed record XFAiOnboardingStep
{
    /// <summary>Unique step identifier.</summary>
    public required string Id { get; init; }

    /// <summary>Step title displayed in header.</summary>
    public required string Title { get; init; }

    /// <summary>Step description displayed below title.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Icon name from icon catalog.</summary>
    public string? IconName { get; init; }

    /// <summary>Current state of this step.</summary>
    public XFAiOnboardingStepState State { get; init; } = XFAiOnboardingStepState.Pending;

    /// <summary>Whether this step can be skipped.</summary>
    public bool CanSkip { get; init; } = true;

    /// <summary>Whether this step requires AI interaction.</summary>
    public bool IsAiDriven { get; init; }

    /// <summary>AI prompt for this step (if IsAiDriven).</summary>
    public string? AiPrompt { get; init; }

    /// <summary>Custom content key for rendering.</summary>
    public string? ContentKey { get; init; }
}

/// <summary>
/// Event arguments when onboarding step changes.
/// </summary>
public sealed record XFAiOnboardingStepChangedEventArgs
{
    /// <summary>Previous step index.</summary>
    public int PreviousStep { get; init; }

    /// <summary>Current step index.</summary>
    public int CurrentStep { get; init; }

    /// <summary>Whether the previous step was skipped.</summary>
    public bool WasSkipped { get; init; }

    /// <summary>Timestamp of the change.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Event arguments when onboarding completes.
/// </summary>
public sealed record XFAiOnboardingCompletedEventArgs
{
    /// <summary>Total steps completed (excluding skipped).</summary>
    public int StepsCompleted { get; init; }

    /// <summary>Total steps skipped.</summary>
    public int StepsSkipped { get; init; }

    /// <summary>Total duration of onboarding.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>Timestamp of completion.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
