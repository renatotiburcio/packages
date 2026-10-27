using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.ColorInput;

namespace XForge.DesignSystem.Tests.Components;

public class XFColorPickerComponentTest : BunitContext
{
    [Fact]
    public void Render_WithValue_ShowsHexAndRgb()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Label, "Accent");
            parameters.Add(p => p.Value, "#38bdf8");
        });

        Assert.Contains("xf-color-picker", cut.Markup);
        Assert.Contains("Accent", cut.Markup);
        Assert.Contains("value=\"#38BDF8\"", cut.Markup);
        Assert.DoesNotContain("Theme color", cut.Markup);

        cut.Find("button[aria-label='Open color palette']").Click();

        Assert.Contains("RGB 56, 189, 248", cut.Markup);
        Assert.Contains("Theme color", cut.Markup);
    }

    [Fact]
    public async Task TextChange_NormalizesHexValue()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("input[type=text]").Change("22c55e");
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.Equal("#22C55E", changed);
    }

    [Fact]
    public void TextInput_InvalidValue_MarksAriaInvalid()
    {
        var cut = Render<XFColorPicker>();

        cut.Find("input[type=text]").Change("nope");

        Assert.Equal("true", cut.Find("input[type=text]").GetAttribute("aria-invalid"));
    }

    [Fact]
    public async Task PaletteButton_SelectsColor()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Palette, new[] { "#0ea5e9", "#f97316" });
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        await cut.Find("button[aria-label='Open color palette']").ClickAsync();

        // Switch to Palette tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Palette")).Click();

        Assert.Contains("role=\"group\"", cut.Markup);
        await cut.Find("button[aria-label='Select #F97316']").ClickAsync();
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.Equal("#F97316", changed);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredCopy()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#38bdf8");
            parameters.Add(p => p.Palette, new[] { "#0ea5e9" });
            parameters.Add(p => p.ToggleLabel, "Open colors");
            parameters.Add(p => p.ThemeColorTitle, "Theme color");
            parameters.Add(p => p.ThemeScaleLabel, "Theme color scale");
            parameters.Add(p => p.CustomColorsTitle, "Saved colors");
            parameters.Add(p => p.AddColorText, "Add");
            parameters.Add(p => p.HexLabel, "Hex value");
            parameters.Add(p => p.AlphaLabel, "Opacity");
            parameters.Add(p => p.CancelText, "Cancel");
            parameters.Add(p => p.ApplyText, "Apply");
            parameters.Add(p => p.SwatchLabelFormat, "Choose {0}");
        });

        cut.Find("button[aria-label='Open colors']").Click();

        // Header and footer are always visible
        Assert.Contains("Theme color", cut.Markup);
        Assert.Contains("Cancel", cut.Markup);
        Assert.Contains("Apply", cut.Markup);

        // Switch to Palette tab for palette content
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Palette")).Click();
        Assert.Contains("aria-label=\"Theme color scale\"", cut.Markup);
        Assert.Contains("Saved colors", cut.Markup);
        Assert.Contains("Add", cut.Markup);
        Assert.Contains("aria-label=\"Choose #0EA5E9\"", cut.Markup);

        // Switch to Details tab for hex/RGB content
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Details")).Click();
        Assert.Contains("Hex value", cut.Markup);
    }

    [Fact]
    public async Task ThemeSwatch_SelectsColor()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.ThemePalette, new[] { new[] { "#111111", "#222222" } });
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        await cut.Find("button[aria-label='Open color palette']").ClickAsync();

        // Switch to Palette tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Palette")).Click();

        await cut.Find("button[aria-label='Select #222222']").ClickAsync();
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.Equal("#222222", changed);
    }

    [Fact]
    public void Render_WithAlpha_ShowsRgbaAndEightDigitHex()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#38bdf880");
            parameters.Add(p => p.ShowAlpha, true);
        });

        Assert.Contains("value=\"#38BDF880\"", cut.Markup);

        cut.Find("button[aria-label='Open color palette']").Click();

        Assert.Contains("RGBA 56, 189, 248, 50%", cut.Markup);
        Assert.Equal("#38BDF8", cut.Find("input[type=color]").GetAttribute("value"));
    }

    [Fact]
    public void Render_ColorModels_ShowsHslAndHsvReadouts()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Switch to Details tab for color models
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Details")).Click();

        Assert.Contains("HSL", cut.Markup);
        Assert.Contains("0deg 100% 50%", cut.Markup);
        Assert.Contains("HSV", cut.Markup);
        Assert.Contains("0deg 100% 100%", cut.Markup);
    }

    [Fact]
    public void AlphaChange_UpdatesEightDigitHex()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#22C55EFF");
            parameters.Add(p => p.ShowAlpha, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("button[aria-label='Open color palette']").Click();
        cut.Find("input[type=range]").Change("25");
        cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).Click();

        Assert.Equal("#22C55E40", changed);
    }

    [Fact]
    public async Task Cancel_DiscardsDraftSelection()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#111111");
            parameters.Add(p => p.ThemePalette, new[] { new[] { "#111111", "#222222" } });
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        await cut.Find("button[aria-label='Open color palette']").ClickAsync();

        // Switch to Palette tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Palette")).Click();

        await cut.Find("button[aria-label='Select #222222']").ClickAsync();
        await cut.FindAll("button").First(button => button.TextContent.Contains("Cancel", StringComparison.Ordinal)).ClickAsync();

        Assert.Null(changed);
        Assert.Contains("value=\"#111111\"", cut.Markup);
    }

    // --- S-G051: Interactive HSV Picker tests ---

    [Fact]
    public void Render_WithShowHsvPicker_ShowsSvAreaAndHueSlider()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Picker tab is default (tab 0)
        Assert.Contains("Saturation and brightness picker", cut.Markup);
        Assert.Contains("Hue slider", cut.Markup);
        Assert.Contains("role=\"slider\"", cut.Markup);
    }

    [Fact]
    public void Render_WithShowHsvPickerFalse_HidesSvArea()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, false);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        Assert.DoesNotContain("Saturation and brightness picker", cut.Markup);
        Assert.DoesNotContain("Hue slider", cut.Markup);
    }

    [Fact]
    public void Render_WithShowHsvPicker_ShowsRgbInputs()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
            parameters.Add(p => p.ShowRgb, true);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Switch to Details tab for RGB inputs
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Details")).Click();

        Assert.Contains("aria-label=\"R\"", cut.Markup);
        Assert.Contains("aria-label=\"G\"", cut.Markup);
        Assert.Contains("aria-label=\"B\"", cut.Markup);
    }

    [Fact]
    public async Task HsvPicker_OnSvDrag_UpdatesDraftColor()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("button[aria-label='Open color palette']").Click();
        await cut.InvokeAsync(async () => await cut.Instance.OnSvDrag(0.5, 0.5));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.NotNull(changed);
        Assert.NotEqual("#FF0000", changed);
    }

    [Fact]
    public async Task HsvPicker_OnHueDrag_UpdatesDraftColor()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("button[aria-label='Open color palette']").Click();
        await cut.InvokeAsync(async () => await cut.Instance.OnHueDrag(120));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.NotNull(changed);
        Assert.NotEqual("#FF0000", changed);
    }

    [Fact]
    public async Task HsvPicker_RgbInputs_UpdateDraftColor()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
            parameters.Add(p => p.ShowRgb, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Switch to Details tab for RGB inputs
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Details")).Click();

        cut.Find("input[aria-label='R']").Change("0");
        cut.Find("input[aria-label='G']").Change("255");
        cut.Find("input[aria-label='B']").Change("0");
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.Equal("#00FF00", changed);
    }

    [Fact]
    public async Task HsvPicker_SvDragThenApply_PreservesAlpha()
    {
        string? changed = null;
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff000080");
            parameters.Add(p => p.ShowAlpha, true);
            parameters.Add(p => p.ShowHsvPicker, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value));
        });

        cut.Find("button[aria-label='Open color palette']").Click();
        await cut.InvokeAsync(async () => await cut.Instance.OnSvDrag(0.5, 0.5));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Apply", StringComparison.Ordinal)).ClickAsync();

        Assert.NotNull(changed);
        Assert.EndsWith("80", changed, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Render_TabBar_ShowsThreeTabs()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        var tabs = cut.FindAll("button[role='tab']");
        Assert.Equal(3, tabs.Count);
        Assert.Contains("Picker", tabs[0].TextContent);
        Assert.Contains("Palette", tabs[1].TextContent);
        Assert.Contains("Details", tabs[2].TextContent);
    }

    [Fact]
    public void Render_TabBar_HidesDetailsTab_WhenNoRgbOrModels()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowRgb, false);
            parameters.Add(p => p.ShowColorModels, false);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        var tabs = cut.FindAll("button[role='tab']");
        Assert.Equal(2, tabs.Count);
    }

    [Fact]
    public void TabNavigation_SwitchesContent()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Default: Picker tab — HSV visible
        Assert.Contains("Saturation and brightness picker", cut.Markup);

        // Switch to Palette tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Palette")).Click();
        Assert.DoesNotContain("Saturation and brightness picker", cut.Markup);
        Assert.Contains("Theme color scale", cut.Markup);

        // Switch to Details tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Details")).Click();
        Assert.DoesNotContain("Saturation and brightness picker", cut.Markup);
        Assert.Contains("HSL", cut.Markup);

        // Back to Picker tab
        cut.FindAll("button[role='tab']").First(b => b.TextContent.Contains("Picker")).Click();
        Assert.Contains("Saturation and brightness picker", cut.Markup);
    }

    [Fact]
    public void TabBar_DefaultTab_IsPicker_WhenHsvEnabled()
    {
        var cut = Render<XFColorPicker>(parameters =>
        {
            parameters.Add(p => p.Value, "#ff0000");
            parameters.Add(p => p.ShowHsvPicker, true);
        });

        cut.Find("button[aria-label='Open color palette']").Click();

        // Picker tab is the default active tab (index 0)
        Assert.Contains("role=\"tab\"", cut.Markup);
        Assert.Contains("Saturation and brightness picker", cut.Markup);
        Assert.Contains("Hue slider", cut.Markup);
    }

    [Theory]
    [InlineData(0, 1, 1, 255, 0, 0)]
    [InlineData(120, 1, 1, 0, 255, 0)]
    [InlineData(240, 1, 1, 0, 0, 255)]
    [InlineData(0, 0, 1, 255, 255, 255)]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(60, 1, 1, 255, 255, 0)]
    [InlineData(180, 1, 1, 0, 255, 255)]
    public void HsvToRgb_Conversion_Correct(double h, double s, double v, int expectedR, int expectedG, int expectedB)
    {
        var (r, g, b) = XFColorPicker.HsvToRgb(h, s, v);
        Assert.Equal(expectedR, r);
        Assert.Equal(expectedG, g);
        Assert.Equal(expectedB, b);
    }

    [Theory]
    [InlineData(255, 0, 0, 0, 1, 1)]
    [InlineData(0, 255, 0, 120, 1, 1)]
    [InlineData(0, 0, 255, 240, 1, 1)]
    [InlineData(255, 255, 255, 0, 0, 1)]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(255, 255, 0, 60, 1, 1)]
    [InlineData(0, 255, 255, 180, 1, 1)]
    public void RgbToHsv_Conversion_Correct(int r, int g, int b, double expectedH, double expectedS, double expectedV)
    {
        var (h, s, v) = XFColorPicker.RgbToHsv(r, g, b);
        Assert.Equal(expectedH, h, 0);
        Assert.Equal(expectedS, s, 0);
        Assert.Equal(expectedV, v, 0);
    }
}
