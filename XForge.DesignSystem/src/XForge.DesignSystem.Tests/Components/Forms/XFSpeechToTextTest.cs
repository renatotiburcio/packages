using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.SpeechToText;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

/// <summary>
/// Tests for XFSpeechToText component.
/// </summary>
public sealed class XFSpeechToTextTest : BunitContext
{
    public XFSpeechToTextTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void SpeechToText_Renders_Default()
    {
        var cut = Render<XFSpeechToText>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithCustomLanguage()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Language, "en-US"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithContinuous()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Continuous, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithInterimResults()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.InterimResults, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithCustomStartLabel()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.StartLabel, "Start Recording"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithCustomStopLabel()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.StopLabel, "Stop Recording"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithFullWidth()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.FullWidth, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithOutlineVariant()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Variant, XFFormVariant.Outline));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithFilledVariant()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithSoftVariant()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithGhostVariant()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithSmSize()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithLgSize()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithDefaultTone()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Tone, XFSemanticColor.Default));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithPrimaryTone()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithSuccessTone()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithWarningTone()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithDangerTone()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithCustomClass()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_InitialState_IsNotRecording()
    {
        var cut = Render<XFSpeechToText>();
        Assert.False(cut.Instance.IsRecording);
    }

    [Fact]
    public void SpeechToText_Renders_WithStartContent()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.StartContent, (RenderFragment)(builder => { builder.AddContent(0, "start"); })));
        Assert.NotNull(cut);
    }

    [Fact]
    public void SpeechToText_Renders_WithRecordingContent()
    {
        var cut = Render<XFSpeechToText>(p => p.Add(x => x.RecordingContent, (RenderFragment)(builder => { builder.AddContent(0, "recording"); })));
        Assert.NotNull(cut);
    }
}
