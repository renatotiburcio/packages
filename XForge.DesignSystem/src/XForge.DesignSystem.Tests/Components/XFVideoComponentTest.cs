using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Video;

namespace XForge.DesignSystem.Tests.Components;

public class XFVideoComponentTest : BunitContext
{
    [Fact]
    public void Render_WithSrc_UsesVideoAttributes()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Type, "video/mp4");
            parameters.Add(p => p.Poster, "poster.png");
            parameters.Add(p => p.AriaLabel, "Demo video");
        });

        var video = cut.Find("video");
        Assert.Equal("poster.png", video.GetAttribute("poster"));
        Assert.Equal("Demo video", video.GetAttribute("aria-label"));
        Assert.Contains("src=\"demo.mp4\"", cut.Markup);
        Assert.Contains("type=\"video/mp4\"", cut.Markup);
    }

    [Fact]
    public void Render_WithSourcesAndTracks()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Sources, new[]
            {
                new XFVideoSource { Src = "demo.webm", Type = "video/webm" },
                new XFVideoSource { Src = "demo.mp4", Type = "video/mp4" },
            });
            parameters.Add(p => p.Tracks, new[]
            {
                new XFVideoTrack { Src = "demo.pt.vtt", SrcLang = "pt-BR", Label = "Portugues", Default = true },
            });
        });

        Assert.Equal(2, cut.FindAll("source").Count);
        Assert.Contains("demo.pt.vtt", cut.Markup);
        Assert.Contains("srclang=\"pt-BR\"", cut.Markup);
    }

    [Fact]
    public void Render_NoSourceUsesFallback()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Unavailable");
        });

        Assert.DoesNotContain("<video", cut.Markup);
        Assert.Contains("role=\"img\"", cut.Markup);
        Assert.Contains("Unavailable", cut.Markup);
    }

    [Fact]
    public void Render_DefaultAriaLabel_UsesPortugueseVideoLabel()
    {
        var fallbackCut = Render<XFVideo>();
        var videoCut = Render<XFVideo>(parameters =>
            parameters.Add(p => p.Src, "demo.mp4"));

        Assert.Contains("aria-label=\"Midia de video\"", fallbackCut.Markup);
        Assert.Equal("Midia de video", videoCut.Find("video").GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_FitCoverUsesObjectCover()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Fit, XFVideoFit.Cover);
        });

        Assert.Contains("object-cover", cut.Markup);
    }

    [Fact]
    public void Render_InvisibleProducesNoMarkup()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.Src, "demo.mp4");
        });

        Assert.Equal(string.Empty, cut.Markup);
    }

    [Fact]
    public void Render_FitFillUsesObjectFill()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Fit, XFVideoFit.Fill);
        });

        Assert.Contains("object-fill", cut.Markup);
    }

    [Fact]
    public void Render_FitContainDefaultUsesObjectContain()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
        });

        Assert.Contains("object-contain", cut.Markup);
    }

    [Fact]
    public void Render_ControlsAttribute_WhenTrue()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Controls, true);
        });

        var video = cut.Find("video");
        Assert.True(video.HasAttribute("controls"));
    }

    [Fact]
    public void Render_Autoplay_WhenTrue()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Autoplay, true);
        });

        var video = cut.Find("video");
        Assert.True(video.HasAttribute("autoplay"));
    }

    [Fact]
    public void Render_Muted_WhenTrue()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Muted, true);
        });

        var video = cut.Find("video");
        Assert.True(video.HasAttribute("muted"));
    }

    [Fact]
    public void Render_Loop_WhenTrue()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Loop, true);
        });

        var video = cut.Find("video");
        Assert.True(video.HasAttribute("loop"));
    }

    [Fact]
    public void Render_PlaysInline_DefaultTrue()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
        });

        var video = cut.Find("video");
        Assert.True(video.HasAttribute("playsinline"));
    }

    [Fact]
    public void Render_Preload_DefaultMetadata()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
        });

        var video = cut.Find("video");
        Assert.Equal("metadata", video.GetAttribute("preload"));
    }

    [Fact]
    public void Render_CustomPreload_AppliedToVideo()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Preload, "auto");
        });

        var video = cut.Find("video");
        Assert.Equal("auto", video.GetAttribute("preload"));
    }

    [Fact]
    public void Render_CustomFallbackText()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Unavailable");
            parameters.Add(p => p.FallbackText, "Video not supported");
        });

        Assert.Contains("Video not supported", cut.Markup);
    }

    [Fact]
    public void Render_FallbackContent_RendersCustomFragment()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.FallbackContent, builder => builder.AddContent(0, "<b>Custom fallback</b>"));
        });

        Assert.Contains("Custom fallback", cut.Markup);
    }

    [Fact]
    public void Render_DefaultFallbackText_InPortuguese()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "test");
        });

        Assert.Contains("Seu navegador nao suporta video.", cut.Markup);
    }

    [Fact]
    public void Render_TrackAttributes_AppliedCorrectly()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Sources, new[]
            {
                new XFVideoSource { Src = "demo.mp4", Type = "video/mp4" }
            });
            parameters.Add(p => p.Tracks, new[]
            {
                new XFVideoTrack
                {
                    Src = "subs.vtt",
                    Kind = "captions",
                    SrcLang = "en",
                    Label = "English",
                    Default = true
                }
            });
        });

        var track = cut.Find("track");
        Assert.Equal("subs.vtt", track.GetAttribute("src"));
        Assert.Equal("captions", track.GetAttribute("kind"));
        Assert.Equal("en", track.GetAttribute("srclang"));
        Assert.Equal("English", track.GetAttribute("label"));
        Assert.True(track.HasAttribute("default"));
    }

    [Fact]
    public void Render_MultipleTracks_AllRendered()
    {
        var cut = Render<XFVideo>(parameters =>
        {
            parameters.Add(p => p.Src, "demo.mp4");
            parameters.Add(p => p.Tracks, new[]
            {
                new XFVideoTrack { Src = "pt.vtt", SrcLang = "pt", Label = "PT" },
                new XFVideoTrack { Src = "en.vtt", SrcLang = "en", Label = "EN" }
            });
        });

        Assert.Equal(2, cut.FindAll("track").Count);
    }
}
