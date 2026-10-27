using Bunit;
using XForge.DesignSystem.Components.DataDisplay.Barcode;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFBarcodeExtendedTest : BunitContext
{
    public XFBarcodeExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // Code39 with invalid characters (lines 149-151) — char not in Code39Patterns
    [Fact]
    public void Extended_Code39InvalidChars_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Code39);
            p.Add(x => x.Value, "AB@CD"); // @ is not a valid Code39 character
        });
        Assert.DoesNotContain("<svg", cut.Markup);
        Assert.Contains("Informe um valor", cut.Markup);
    }

    // Code39 with asterisk (lines 149-151) — returns null
    [Fact]
    public void Extended_Code39Asterisk_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Code39);
            p.Add(x => x.Value, "AB*CD"); // asterisk is start/stop char, not allowed in data
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Code39 null value (lines 143-145) — returns null
    [Fact]
    public void Extended_Code39NullValue_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Code39);
            p.Add(x => x.Value, (string?)null);
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Ean13 wrong length (lines 162-163) — returns null
    [Fact]
    public void Extended_Ean13WrongLength_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Ean13);
            p.Add(x => x.Value, "12345"); // too short
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Ean13 invalid checksum (lines 168-169) — returns null
    [Fact]
    public void Extended_Ean13InvalidChecksum_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Ean13);
            p.Add(x => x.Value, "7891234567890"); // wrong checksum digit
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Ean8 wrong length (lines 194-195) — returns null
    [Fact]
    public void Extended_Ean8WrongLength_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Ean8);
            p.Add(x => x.Value, "123"); // too short
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // UpcA wrong length (lines 224-225) — returns null
    [Fact]
    public void Extended_UpcAWrongLength_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.UpcA);
            p.Add(x => x.Value, "123"); // too short
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Code128 with invalid character (lines 123-124) — returns null
    [Fact]
    public void Extended_Code128InvalidChar_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Code128);
            p.Add(x => x.Value, "ABC\u0001DEF"); // control char < 32
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Code128 null value (lines 110-111) — returns null
    [Fact]
    public void Extended_Code128NullValue_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Code128);
            p.Add(x => x.Value, (string?)null);
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Itf14 wrong length
    [Fact]
    public void Extended_Itf14WrongLength_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Itf14);
            p.Add(x => x.Value, "123"); // too short
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Interleaved2Of5 with non-digit chars — NormalizeDigits strips them, may become empty
    [Fact]
    public void Extended_Interleaved2Of5InvalidChars_RendersFallback()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Interleaved2Of5);
            p.Add(x => x.Value, "ABC"); // no digits at all
        });
        Assert.DoesNotContain("<svg", cut.Markup);
    }

    // Custom ForegroundColor
    [Fact]
    public void Extended_CustomForegroundColor_RendersColor()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.ForegroundColor, "#FF0000");
        });
        Assert.Contains("#FF0000", cut.Markup);
    }

    // Custom BackgroundColor
    [Fact]
    public void Extended_CustomBackgroundColor_RendersColor()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.BackgroundColor, "#F0F0F0");
        });
        Assert.Contains("#F0F0F0", cut.Markup);
    }

    // Custom Height
    [Fact]
    public void Extended_CustomHeight_RendersHeight()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.Height, 150);
        });
        Assert.Contains("<svg", cut.Markup);
    }

    // Custom ModuleWidth
    [Fact]
    public void Extended_CustomModuleWidth_RendersBars()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.ModuleWidth, 3);
        });
        Assert.Contains("<svg", cut.Markup);
    }

    // Custom QuietZone
    [Fact]
    public void Extended_CustomQuietZone_RendersSvg()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.QuietZone, 20);
        });
        Assert.Contains("<svg", cut.Markup);
    }

    // Custom AriaLabel
    [Fact]
    public void Extended_CustomAriaLabel_RendersLabel()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, "ABC123");
            p.Add(x => x.AriaLabel, "Custom barcode");
        });
        Assert.Contains("Custom barcode", cut.Markup);
    }

    // EmptyText custom
    [Fact]
    public void Extended_CustomEmptyText_RendersText()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Value, (string?)null);
            p.Add(x => x.EmptyText, "No barcode");
        });
        Assert.Contains("No barcode", cut.Markup);
    }

    // Gs1_128 format
    [Fact]
    public void Extended_Gs1128_RendersSvg()
    {
        var cut = Render<XFBarcode>(p =>
        {
            p.Add(x => x.Format, XFBarcodeFormat.Gs1_128);
            p.Add(x => x.Value, "01123456789012311726010110AB12");
        });
        Assert.Contains("<svg", cut.Markup);
    }
}
