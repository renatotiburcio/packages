using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.AI.Components;

/// <summary>
/// Voice biometric component for speaker identification and verification.
/// Records audio samples, displays waveform visualization, and identifies
/// speakers with confidence scoring.
/// </summary>
public partial class XFAiVoiceBiometric : XForgeComponentBase
{
    /// <summary>Current recording state (idle, recording, processing).</summary>
    [Parameter] public string State { get; set; } = "idle";

    /// <summary>Whether the microphone is currently recording.</summary>
    [Parameter] public bool IsRecording { get; set; }

    /// <summary>Identified speaker name, or null if unknown.</summary>
    [Parameter] public string? IdentifiedSpeaker { get; set; }

    /// <summary>Identification confidence score (0.0-1.0).</summary>
    [Parameter] public double Confidence { get; set; }

    /// <summary>Audio waveform samples for visualization.</summary>
    [Parameter] public IReadOnlyList<double> Samples { get; set; } = new List<double>();

    /// <summary>Fires when recording is toggled on/off.</summary>
    [Parameter] public EventCallback<bool> OnRecordToggle { get; set; }

    private void ToggleRecording() => OnRecordToggle.InvokeAsync(!IsRecording);
}
