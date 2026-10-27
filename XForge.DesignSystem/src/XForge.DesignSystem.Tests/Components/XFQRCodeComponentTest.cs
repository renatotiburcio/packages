using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.QRCode;

namespace XForge.DesignSystem.Tests.Components;

public class XFQRCodeComponentTest : BunitContext
{
    [Fact]
    public void Render_ShortValueGeneratesSvgModules()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "https://xforge.local/pedido/184");
        });

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("shape-rendering=\"crispEdges\"", cut.Markup);
        Assert.Contains("fill=\"#ffffff\"", cut.Markup);
        Assert.Contains("fill=\"#111827\"", cut.Markup);
        Assert.Contains("Codigo QR https://xforge.local/pedido/184", cut.Markup);
    }

    [Fact]
    public void Render_ShowTextDisplaysValue()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "pix:xforge-184");
            parameters.Add(p => p.ShowText, true);
        });

        Assert.Contains("pix:xforge-184", cut.Markup);
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Render_EmptyValueShowsFallback()
    {
        var cut = Render<XFQRCode>();

        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um texto curto para gerar o codigo QR.", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredCopy()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "https://xforge.local/app");
            parameters.Add(p => p.AriaLabelFormat, "QR payload {0}");
            parameters.Add(p => p.ShowDownload, true);
            parameters.Add(p => p.DownloadText, "Download SVG");
        });

        Assert.Contains("aria-label=\"QR payload https://xforge.local/app\"", cut.Markup);
        Assert.Contains("Download SVG", cut.Markup);
    }

    [Fact]
    public void Render_LongPayloadShowsFallback()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, new string('A', 120));
        });

        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um texto curto para gerar o codigo QR.", cut.Markup);
    }

    [Fact]
    public void Render_CustomColorsAndSizing()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "LOTE:AB12");
            parameters.Add(p => p.ModuleSize, 4);
            parameters.Add(p => p.QuietZone, 2);
            parameters.Add(p => p.ForegroundColor, "#000000");
            parameters.Add(p => p.BackgroundColor, "#f8fafc");
        });

        Assert.Contains("fill=\"#000000\"", cut.Markup);
        Assert.Contains("fill=\"#f8fafc\"", cut.Markup);
        Assert.Contains("viewBox=\"0 0 100 100\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCenterContent_RendersOverlay()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "https://xforge.local/app");
            parameters.Add(p => p.CenterContent, builder => builder.AddContent(0, "XF"));
        });

        Assert.Contains("XF", cut.Markup);
        Assert.Contains("ring-4 ring-white", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithDownload_RendersSvgDataUri()
    {
        var cut = Render<XFQRCode>(parameters =>
        {
            parameters.Add(p => p.Value, "https://xforge.local/app");
            parameters.Add(p => p.ShowDownload, true);
            parameters.Add(p => p.DownloadFileName, "app.svg");
        });

        var link = cut.Find("a[download='app.svg']");

        Assert.Equal("Baixar SVG", link.TextContent);
        Assert.StartsWith("data:image/svg+xml;base64,", link.GetAttribute("href"));
    }
}
