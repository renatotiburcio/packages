using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.PdfViewer;

namespace XForge.DesignSystem.Tests.Components;

public class XFPdfViewerComponentTest : BunitContext
{
    // --- Existing tests (preserved) ---

    [Fact]
    public void Render_WithUrlBuildsIframeSource()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.Page, 2);
            parameters.Add(p => p.Zoom, 125);
        });

        var iframe = cut.Find("iframe");
        Assert.Equal("/docs/file.pdf#page=2&zoom=125", iframe.GetAttribute("src"));
    }

    [Fact]
    public void Render_WithBase64BuildsDataSource()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Base64, "JVBERi0x");
        });

        Assert.Contains("data:application/pdf;base64,JVBERi0x#page=1&amp;zoom=100", cut.Markup);
    }

    [Fact]
    public async Task Toolbar_NavigatesPagesAndZoom()
    {
        var page = 1;
        var zoom = 100;
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.Page, page);
            parameters.Add(p => p.PageCount, 2);
            parameters.Add(p => p.PageChanged, value => page = value);
            parameters.Add(p => p.ZoomChanged, value => zoom = value);
        });

        await cut.Find("[aria-label=Proxima]").ClickAsync(new MouseEventArgs());
        await cut.Find("[aria-label=\"Aumentar zoom\"]").ClickAsync(new MouseEventArgs());

        Assert.Equal(2, page);
        Assert.Equal(110, zoom);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredPageLabels()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.Page, 2);
            parameters.Add(p => p.PageCount, 5);
            parameters.Add(p => p.PreviousLabel, "Previous");
            parameters.Add(p => p.NextLabel, "Next");
            parameters.Add(p => p.ZoomInLabel, "Zoom in");
            parameters.Add(p => p.ZoomOutLabel, "Zoom out");
            parameters.Add(p => p.PageCountLabelFormat, "Page {0} of {1}");
        });

        Assert.Contains("aria-label=\"Previous\"", cut.Markup);
        Assert.Contains("aria-label=\"Next\"", cut.Markup);
        Assert.Contains("aria-label=\"Zoom in\"", cut.Markup);
        Assert.Contains("aria-label=\"Zoom out\"", cut.Markup);
        Assert.Contains("Page 2 of 5", cut.Markup);
    }

    [Fact]
    public void Render_WithoutSourceShowsEmptyState()
    {
        var cut = Render<XFPdfViewer>();

        Assert.DoesNotContain("<iframe", cut.Markup);
        Assert.Contains("Informe uma URL", cut.Markup);
    }

    // --- S-C016: New tests for Canvas mode, toolbar buttons, loading state ---

    [Fact]
    public void Render_CanvasMode_ShowsCanvasElement()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
        });

        Assert.Contains("<canvas", cut.Markup);
        Assert.DoesNotContain("<iframe", cut.Markup);
    }

    [Fact]
    public void Render_IframeMode_ShowsIframeElement()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Iframe);
        });

        Assert.Contains("<iframe", cut.Markup);
        Assert.DoesNotContain("<canvas", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_DefaultRenderModeIsIframe()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
        });

        // Default should be Iframe
        Assert.Contains("<iframe", cut.Markup);
        Assert.DoesNotContain("<canvas", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_ShowsDownloadButton()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.ShowDownload, true);
        });

        Assert.Contains("aria-label=\"Baixar\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_ShowsPrintButton()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.ShowPrint, true);
        });

        Assert.Contains("aria-label=\"Imprimir\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_ShowsFullscreenButton()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.ShowFullscreen, true);
        });

        Assert.Contains("aria-label=\"Tela cheia\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_HidesDownloadButton_WhenDisabled()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.ShowDownload, false);
        });

        Assert.DoesNotContain("aria-label=\"Baixar\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_HidesPrintButton_WhenDisabled()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.ShowPrint, false);
        });

        Assert.DoesNotContain("aria-label=\"Imprimir\"", cut.Markup);
    }

    [Fact]
    public void Render_IframeMode_HidesCanvasButtons()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Iframe);
        });

        Assert.DoesNotContain("aria-label=\"Baixar\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Imprimir\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Tela cheia\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_LoadingTextParameter_IsStored()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.LoadingText, "Carregando...");
        });

        // LoadingText is a parameter — verify it's accessible on the instance
        Assert.Equal("Carregando...", cut.Instance.LoadingText);
    }

    [Fact]
    public void Render_CanvasMode_CustomLoadingTextParameter_IsStored()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.LoadingText, "Please wait...");
        });

        Assert.Equal("Please wait...", cut.Instance.LoadingText);
    }

    [Fact]
    public void Render_CanvasMode_CustomDownloadLabel()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.DownloadLabel, "Download PDF");
        });

        Assert.Contains("aria-label=\"Download PDF\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_CustomPrintLabel()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.PrintLabel, "Print Document");
        });

        Assert.Contains("aria-label=\"Print Document\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_CustomFullscreenLabel()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.FullscreenLabel, "Go Fullscreen");
        });

        Assert.Contains("aria-label=\"Go Fullscreen\"", cut.Markup);
    }

    [Fact]
    public void Render_CanvasMode_HasAriaLabel()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.AriaLabel, "My PDF Viewer");
        });

        Assert.Contains("aria-label=\"My PDF Viewer\"", cut.Markup);
    }

    [Fact]
    public void NotifyPdfInitialized_UpdatesPageCount()
    {
        var loadedArgs = default(XFPdfDocumentLoadedEventArgs);
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.OnDocumentLoaded, args => loadedArgs = args);
        });

        var instance = cut.Instance;
        instance.NotifyPdfInitialized(new XFPdfNotification
        {
            PageNumber = 1,
            TotalPages = 10,
            Scale = 100,
            Title = "Test Document",
        });

        Assert.NotNull(loadedArgs);
        Assert.Equal(10, loadedArgs.PageCount);
        Assert.Equal("Test Document", loadedArgs.Title);
    }

    [Fact]
    public void NotifyPdfChanged_UpdatesPage()
    {
        var page = 1;
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.PageChanged, value => page = value);
        });

        var instance = cut.Instance;
        instance.NotifyPdfChanged(new XFPdfNotification
        {
            PageNumber = 3,
            TotalPages = 10,
            Scale = 100,
        });

        Assert.Equal(3, page);
    }

    [Fact]
    public void NotifyPdfInitialized_WithNullTitle_SetsNullTitle()
    {
        var loadedArgs = default(XFPdfDocumentLoadedEventArgs);
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.RenderMode, XFPdfRenderMode.Canvas);
            parameters.Add(p => p.OnDocumentLoaded, args => loadedArgs = args);
        });

        var instance = cut.Instance;
        instance.NotifyPdfInitialized(new XFPdfNotification
        {
            PageNumber = 1,
            TotalPages = 5,
            Scale = 100,
            Title = "",
        });

        Assert.NotNull(loadedArgs);
        Assert.Equal(5, loadedArgs.PageCount);
        Assert.Null(loadedArgs.Title);
    }

    [Fact]
    public void XFPdfRenderMode_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFPdfRenderMode.Iframe);
        Assert.Equal(1, (int)XFPdfRenderMode.Canvas);
    }

    [Fact]
    public void XFPdfDocumentLoadedEventArgs_HasCorrectProperties()
    {
        var args = new XFPdfDocumentLoadedEventArgs
        {
            PageCount = 42,
            Title = "Test PDF",
        };

        Assert.Equal(42, args.PageCount);
        Assert.Equal("Test PDF", args.Title);
    }

    [Fact]
    public void Render_WithCustomId_RendersId()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Id, "my-pdf-viewer");
            parameters.Add(p => p.Url, "/docs/file.pdf");
        });

        Assert.Contains("id=\"my-pdf-viewer\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomClass_IncludesClass()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Class, "my-custom-class");
            parameters.Add(p => p.Url, "/docs/file.pdf");
        });

        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_RendersAttributes()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "pdf-viewer" },
            });
        });

        Assert.Contains("data-testid=\"pdf-viewer\"", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_DisablesButtons()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.Disabled, true);
        });

        var prevButton = cut.Find("[aria-label=Anterior]");
        Assert.True(prevButton.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_HideToolbar_NoToolbar()
    {
        var cut = Render<XFPdfViewer>(parameters =>
        {
            parameters.Add(p => p.Url, "/docs/file.pdf");
            parameters.Add(p => p.ShowToolbar, false);
        });

        Assert.DoesNotContain("Anterior", cut.Markup);
        Assert.DoesNotContain("Proxima", cut.Markup);
    }
}
