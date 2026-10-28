using Bunit;
using Bunit.TestDoubles;
using Xunit;
using XForge.DesignSystem.Components.Forms.SignaturePad;

namespace XForge.DesignSystem.Tests.Components;

public class XFSignaturePadComponentTest : XForgeTestContext
{
    public XFSignaturePadComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_CanvasWithLabelAndToolbar()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Label, "Assinatura");
        });

        Assert.Contains("Assinatura", cut.Markup);
        Assert.Contains("<canvas", cut.Markup);
        Assert.Contains("Limpar", cut.Markup);
        Assert.Contains("Desfazer", cut.Markup);
    }

    [Fact]
    public async Task SetValueFromJs_UpdatesValueAndChanged()
    {
        string? value = null;
        string? changed = null;
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, next => value = next);
            parameters.Add(p => p.Change, next => changed = next);
        });

        await cut.Instance.SetValueFromJsAsync("data:image/png;base64,abc");

        Assert.Equal("data:image/png;base64,abc", value);
        Assert.Equal("data:image/png;base64,abc", changed);
    }

    [Fact]
    public async Task Clear_ClearsValue()
    {
        string? value = "data:image/png;base64,abc";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.ClearAsync();

        Assert.Null(value);
    }

    [Fact]
    public void RequiredWithoutValueShowsMessage()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Required, true);
        });

        Assert.Contains("Assinatura obrigatoria.", cut.Markup);
    }

    [Fact]
    public void Render_NoLabel_NoLabelElement()
    {
        var cut = Render<XFSignaturePad>();

        Assert.DoesNotContain("<label", cut.Markup);
        Assert.Contains("<canvas", cut.Markup);
    }

    [Fact]
    public void Render_HiddenToolbar_NoToolbarButtons()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.ShowToolbar, false);
        });

        Assert.DoesNotContain("Limpar", cut.Markup);
        Assert.DoesNotContain("Desfazer", cut.Markup);
        Assert.Contains("<canvas", cut.Markup);
    }

    [Fact]
    public void Render_ReadOnly_CanvasHasNotAllowedCursor()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.ReadOnly, true);
        });

        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-70", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_CanvasHasNotAllowedCursor()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-70", cut.Markup);
    }

    [Fact]
    public void Render_Enabled_CanvasHasCrosshairCursor()
    {
        var cut = Render<XFSignaturePad>();

        Assert.Contains("cursor-crosshair", cut.Markup);
    }

    [Fact]
    public async Task Clear_WhenReadOnly_DoesNotClear()
    {
        string? value = "data:image/png;base64,abc";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ReadOnly, true);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.ClearAsync();

        Assert.Equal("data:image/png;base64,abc", value);
    }

    [Fact]
    public async Task Clear_WhenDisabled_DoesNotClear()
    {
        string? value = "data:image/png;base64,abc";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.ClearAsync();

        Assert.Equal("data:image/png;base64,abc", value);
    }

    [Fact]
    public async Task Undo_WhenReadOnly_DoesNotChangeValue()
    {
        string? value = "data:image/png;base64,original";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ReadOnly, true);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.UndoAsync();

        Assert.Equal("data:image/png;base64,original", value);
    }

    [Fact]
    public async Task Undo_WhenDisabled_DoesNotChangeValue()
    {
        string? value = "data:image/png;base64,original";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.UndoAsync();

        Assert.Equal("data:image/png;base64,original", value);
    }

    [Fact]
    public void Render_WithCustomAriaLabel()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Sign here");
        });

        Assert.Contains("aria-label=\"Sign here\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomWidthAndHeight()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Width, 400);
            parameters.Add(p => p.Height, 150);
        });

        Assert.Contains("width=\"400\"", cut.Markup);
        Assert.Contains("height=\"150\"", cut.Markup);
    }

    [Fact]
    public void Render_WithoutRequired_DoesNotShowRequiredMessage()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Required, false);
        });

        Assert.DoesNotContain("Assinatura obrigatoria.", cut.Markup);
    }

    [Fact]
    public void Render_RequiredWithValue_HidesRequiredMessage()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.Value, "data:image/png;base64,abc");
        });

        Assert.DoesNotContain("Assinatura obrigatoria.", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLabels()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.ClearLabel, "Apagar");
            parameters.Add(p => p.UndoLabel, "Voltar");
            parameters.Add(p => p.RequiredText, "Obrigatorio!");
        });

        Assert.Contains("Apagar", cut.Markup);
        Assert.Contains("Voltar", cut.Markup);
    }

    [Fact]
    public async Task SetValueFromJs_WithNullClearsValue()
    {
        string? value = "data:image/png;base64,old";
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, next => value = next);
        });

        await cut.Instance.SetValueFromJsAsync(null);

        Assert.Null(value);
    }

    [Fact]
    public void Render_ReadOnly_DisablesToolbarButtons()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.ReadOnly, true);
        });

        var buttons = cut.FindAll("button");
        Assert.All(buttons, b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void Render_Disabled_DisablesToolbarButtons()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        var buttons = cut.FindAll("button");
        Assert.All(buttons, b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void Render_WithHelperText_ShowsHelperText()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Desenhe sua assinatura.");
        });

        Assert.Contains("Desenhe sua assinatura.", cut.Markup);
    }

    [Fact]
    public void Render_CanvasHasDefaultDimensions()
    {
        var cut = Render<XFSignaturePad>();

        Assert.Contains("width=\"720\"", cut.Markup);
        Assert.Contains("height=\"220\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomPenColor()
    {
        var cut = Render<XFSignaturePad>(parameters =>
        {
            parameters.Add(p => p.PenColor, "#ff0000");
        });

        Assert.NotNull(cut);
        Assert.Contains("<canvas", cut.Markup);
    }
}
