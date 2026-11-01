using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Avatar;
using XForge.DesignSystem.Components.DataDisplay.PdfViewer;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Feedback.SplashScreen;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Components.Forms.FileInput;
using XForge.DesignSystem.Components.Forms.Search;
using XForge.DesignSystem.Components.Forms.SignaturePad;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Components.Navigation.Pagination;
using XForge.DesignSystem.Components.Overlays.Popover;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Validates that all Category 2 localized components use L() calls correctly.
/// With TestLocalizer, L(key, fallback) returns the key instead of the fallback.
/// </summary>
public class XFLocalizationCategory2Test : XForgeTestContext
{
    public XFLocalizationCategory2Test()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Switch_RendersLocalizedLabels()
    {
        // Test checked state
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, true);
            parameters.Add(p => p.ShowStateText, true);
        });

        Assert.Contains("Sim", cut.Markup);

        // Test unchecked state
        var cut2 = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, false);
            parameters.Add(p => p.ShowStateText, true);
        });

        Assert.Contains("Não", cut2.Markup);
    }

    [Fact]
    public void Search_RendersLocalizedPlaceholder()
    {
        var cut = Render<XFSearch>();

        Assert.Contains("Pesquisar", cut.Markup);
    }

    [Fact]
    public void TextInput_RendersLocalizedClearButton()
    {
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Value, "test");
            parameters.Add(p => p.Clearable, true);
        });

        Assert.Contains("Limpar valor", cut.Markup);
    }

    [Fact]
    public void Popover_RendersLocalizedLabel()
    {
        var cut = Render<XFPopover>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Content");
        });

        Assert.Contains("Abrir", cut.Markup);
    }

    [Fact]
    public void SplashScreen_RendersLocalizedLoading()
    {
        var cut = Render<XFSplashScreen>();

        Assert.Contains("Carregando", cut.Markup);
    }

    [Fact]
    public void Progress_RendersLocalizedLoading()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Indeterminate, true);
        });

        Assert.Contains("Carregando", cut.Markup);
    }

    [Fact]
    public void ProgressRing_RendersLocalizedLoading()
    {
        var cut = Render<XFProgressRing>(parameters =>
        {
            parameters.Add(p => p.Indeterminate, true);
        });

        Assert.Contains("Carregando", cut.Markup);
    }

    [Fact]
    public void SignaturePad_RendersLocalizedLabels()
    {
        var cut = Render<XFSignaturePad>();

        Assert.Contains("Campo de assinatura", cut.Markup);
        Assert.Contains("Limpar", cut.Markup);
        Assert.Contains("Desfazer", cut.Markup);
    }

    [Fact]
    public void FileInput_RendersLocalizedLabels()
    {
        var cut = Render<XFFileInput>();

        Assert.Contains("Escolher", cut.Markup);
    }

    [Fact]
    public void DatePicker_RendersLocalizedLabels()
    {
        var cut = Render<XFDatePicker>();

        Assert.Contains("Abrir calendário", cut.Markup);
    }

    [Fact]
    public void Pagination_RendersLocalizedLabels()
    {
        var cut = Render<XFPagination>(parameters =>
        {
            parameters.Add(p => p.TotalItems, 100);
            parameters.Add(p => p.PageSize, 10);
            parameters.Add(p => p.Page, 1);
        });

        Assert.Contains("Primeira página", cut.Markup);
        Assert.Contains("Página anterior", cut.Markup);
        Assert.Contains("Próxima página", cut.Markup);
        Assert.Contains("Última página", cut.Markup);
    }

    [Fact]
    public void Avatar_RendersLocalizedStatus()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Test User");
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("Disponível", cut.Markup);
    }

    [Fact]
    public void PdfViewer_RendersLocalizedLabels()
    {
        var cut = Render<XFPdfViewer>();

        Assert.Contains("Visualizador de PDF", cut.Markup);
        Assert.Contains("Informe uma URL ou base64 para visualizar o PDF.", cut.Markup);
    }
}
