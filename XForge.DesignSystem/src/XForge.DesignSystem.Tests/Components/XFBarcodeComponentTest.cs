using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Barcode;

namespace XForge.DesignSystem.Tests.Components;

public class XFBarcodeComponentTest : BunitContext
{
    [Fact]
    public void Render_Code128SvgWithBarsAndText()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Value, "ABC123");
        });

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("<rect", cut.Markup);
        Assert.Contains(">ABC123</text>", cut.Markup);
        Assert.Contains("Codigo de barras ABC123", cut.Markup);
        Assert.Contains("fill=\"#ffffff\"", cut.Markup);
        Assert.Contains("fill=\"#111827\"", cut.Markup);
    }

    [Fact]
    public void Render_HidesTextWhenShowTextFalse()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Value, "ABC123");
            parameters.Add(p => p.ShowText, false);
        });

        Assert.DoesNotContain("<text", cut.Markup);
    }

    [Fact]
    public void Render_EmptyValueShowsFallback()
    {
        var cut = Render<XFBarcode>();

        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um valor", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredCopy()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Value, "ABC123");
            parameters.Add(p => p.AriaLabelFormat, "Barcode value {0}");
        });

        Assert.Contains("aria-label=\"Barcode value ABC123\"", cut.Markup);
    }

    [Fact]
    public void Render_InvalidCharacterShowsFallback()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Value, "ação");
        });

        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um valor", cut.Markup);
    }

    [Theory]
    [InlineData(XFBarcodeFormat.Ean13, "7891234567895")]
    [InlineData(XFBarcodeFormat.Ean8, "96385074")]
    [InlineData(XFBarcodeFormat.UpcA, "036000291452")]
    [InlineData(XFBarcodeFormat.Code39, "INV-8842")]
    [InlineData(XFBarcodeFormat.Gs1_128, "01123456789012311726010110AB12")]
    [InlineData(XFBarcodeFormat.Itf14, "10012345678902")]
    [InlineData(XFBarcodeFormat.Interleaved2Of5, "1234567890")]
    public void Render_SupportedFormatsGenerateSvg(XFBarcodeFormat format, string value)
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Format, format);
            parameters.Add(p => p.Value, value);
        });

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("<rect", cut.Markup);
        Assert.Contains(value, cut.Markup);
    }

    [Fact]
    public void Render_Ean13WithTwelveDigitsAddsChecksum()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Format, XFBarcodeFormat.Ean13);
            parameters.Add(p => p.Value, "789123456789");
        });

        Assert.Contains(">7891234567895</text>", cut.Markup);
    }

    [Fact]
    public void Render_Unsupported2DFormatShowsFallback()
    {
        var cut = Render<XFBarcode>(parameters =>
        {
            parameters.Add(p => p.Format, XFBarcodeFormat.QrCode);
            parameters.Add(p => p.Value, "pix payload");
        });

        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um valor", cut.Markup);
    }
}
