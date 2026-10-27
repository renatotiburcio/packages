using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Emoji;

namespace XForge.DesignSystem.Tests.Components;

public class TestEmojiClass { }

public class XFFluentEmojiComponentTest : BunitContext
{
    public XFFluentEmojiComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_GenericComponent_RendersSpan()
    {
        var cut = Render<XFFluentEmoji<TestEmojiClass>>();
        Assert.Contains("<span", cut.Markup);
    }

    [Fact]
    public void Render_NoMatch_RendersMissingContent()
    {
        var cut = Render<XFFluentEmoji<TestEmojiClass>>();
        Assert.Contains("xf-emoji-missing", cut.Markup);
    }

    [Fact]
    public void Render_HasEmojiClass()
    {
        var cut = Render<XFFluentEmoji<TestEmojiClass>>();
        Assert.Contains("xf-emoji", cut.Markup);
    }
}
