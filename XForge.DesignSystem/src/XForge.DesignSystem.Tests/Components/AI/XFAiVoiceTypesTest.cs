using Xunit;
using XForge.DesignSystem.Components.AI.Audio;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI Audio types at 0% coverage: XFAiVoiceState, XFAiTranscriptionResult.
/// </summary>
public sealed class XFAiVoiceTypesTest
{
    #region XFAiVoiceState

    [Fact]
    public void XFAiVoiceState_HasAllValues()
    {
        Assert.Equal(5, Enum.GetValues<XFAiVoiceState>().Length);
        Assert.True(Enum.IsDefined<XFAiVoiceState>(XFAiVoiceState.Idle));
        Assert.True(Enum.IsDefined<XFAiVoiceState>(XFAiVoiceState.Listening));
        Assert.True(Enum.IsDefined<XFAiVoiceState>(XFAiVoiceState.Processing));
        Assert.True(Enum.IsDefined<XFAiVoiceState>(XFAiVoiceState.Speaking));
        Assert.True(Enum.IsDefined<XFAiVoiceState>(XFAiVoiceState.Error));
    }

    #endregion

    #region XFAiTranscriptionResult

    [Fact]
    public void XFAiTranscriptionResult_RequiredText_Constructs()
    {
        var result = new XFAiTranscriptionResult { Text = "Hello world" };
        Assert.Equal("Hello world", result.Text);
        Assert.Null(result.Language);
        Assert.Null(result.DurationSeconds);
        Assert.Null(result.Confidence);
        Assert.False(result.IsPartial);
        Assert.True(Math.Abs((DateTime.UtcNow - result.Timestamp).TotalSeconds) < 5);
    }

    [Fact]
    public void XFAiTranscriptionResult_WithAllValues_AssignsCorrectly()
    {
        var ts = DateTime.UtcNow;
        var result = new XFAiTranscriptionResult
        {
            Text = "Teste de transcrição",
            Language = "pt-BR",
            DurationSeconds = 15.5f,
            Confidence = 0.95f,
            IsPartial = true,
            Timestamp = ts
        };
        Assert.Equal("Teste de transcrição", result.Text);
        Assert.Equal("pt-BR", result.Language);
        Assert.Equal(15.5f, result.DurationSeconds);
        Assert.Equal(0.95f, result.Confidence);
        Assert.True(result.IsPartial);
        Assert.Equal(ts, result.Timestamp);
    }

    [Fact]
    public void XFAiTranscriptionResult_RecordEquality_Works()
    {
        var ts = DateTime.UtcNow;
        var a = new XFAiTranscriptionResult { Text = "abc", Timestamp = ts };
        var b = new XFAiTranscriptionResult { Text = "abc", Timestamp = ts };
        Assert.Equal(a, b);
    }

    #endregion
}
