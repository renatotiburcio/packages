using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Tooling;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFAiDesignToolingComponentsTest : BunitContext
{
    #region XFThemeEditor

    [Fact]
    public void ThemeEditor_RendersDefault()
    {
        var cut = Render<XFThemeEditor>();

        Assert.Contains("xf-theme-editor", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Theme Editor", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_ShowsTabs()
    {
        var cut = Render<XFThemeEditor>();

        Assert.Contains("Cores", cut.Markup);
        Assert.Contains("Tipografia", cut.Markup);
        Assert.Contains("Espacamento", cut.Markup);
        Assert.Contains("Preview", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_ShowsColorInputs()
    {
        var cut = Render<XFThemeEditor>();

        Assert.Contains("Primary", cut.Markup);
        Assert.Contains("--xf-color-primary", cut.Markup);
        Assert.Contains("type=\"color\"", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_CustomTitle_Shown()
    {
        var cut = Render<XFThemeEditor>(p => p.Add(x => x.Title, "Meu Tema"));

        Assert.Contains("Meu Tema", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFThemeEditor>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Theme Editor", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_ExportCssButton_Shown()
    {
        var cut = Render<XFThemeEditor>(p => p.Add(x => x.ShowExportPanel, true));

        Assert.Contains("Exportar Tema", cut.Markup);
        Assert.Contains("CSS", cut.Markup);
        Assert.Contains("JSON", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_DarkModeToggle_Shown()
    {
        var cut = Render<XFThemeEditor>(p => p.Add(x => x.ShowDarkModeToggle, true));

        Assert.Contains("Modo", cut.Markup);
    }

    [Fact]
    public void ThemeEditor_GetColors_ReturnsDefault()
    {
        var cut = Render<XFThemeEditor>();
        var component = cut.Instance;

        var colors = component.GetColors();
        Assert.NotEmpty(colors);
        Assert.Contains(colors, c => c.Name == "Primary");
    }

    #endregion

    #region XFDesignTokenExplorer

    [Fact]
    public void DesignTokenExplorer_RendersDefault()
    {
        var cut = Render<XFDesignTokenExplorer>();

        Assert.Contains("xf-design-token-explorer", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Design Tokens", cut.Markup);
    }

    [Fact]
    public void DesignTokenExplorer_ShowsTokenCount()
    {
        var tokens = new List<XFDesignToken>
        {
            new() { Name = "Primary", Value = "#6366f1", Category = XFDesignTokenCategory.Color, CssVariable = "--xf-color-primary" },
            new() { Name = "Secondary", Value = "#8b5cf6", Category = XFDesignTokenCategory.Color, CssVariable = "--xf-color-secondary" }
        };
        var cut = Render<XFDesignTokenExplorer>(p => p.Add(x => x.Tokens, tokens));

        Assert.Contains("2 tokens", cut.Markup);
    }

    [Fact]
    public void DesignTokenExplorer_ShowsSearchInput()
    {
        var cut = Render<XFDesignTokenExplorer>(p => p.Add(x => x.ShowSearch, true));

        Assert.Contains("Buscar token", cut.Markup);
    }

    [Fact]
    public void DesignTokenExplorer_ShowsExportButtons()
    {
        var cut = Render<XFDesignTokenExplorer>(p => p.Add(x => x.ShowExport, true));

        Assert.Contains("CSS", cut.Markup);
        Assert.Contains("JSON", cut.Markup);
        Assert.Contains("SCSS", cut.Markup);
        Assert.Contains("TS", cut.Markup);
    }

    [Fact]
    public void DesignTokenExplorer_ColorTokens_ShowSwatch()
    {
        var tokens = new List<XFDesignToken>
        {
            new() { Name = "Primary", Value = "#6366f1", Category = XFDesignTokenCategory.Color, CssVariable = "--xf-color-primary" }
        };
        var cut = Render<XFDesignTokenExplorer>(p => p.Add(x => x.Tokens, tokens));

        Assert.Contains("background: #6366f1", cut.Markup);
    }

    [Fact]
    public void DesignTokenExplorer_HidesSearch_WhenShowSearchFalse()
    {
        var cut = Render<XFDesignTokenExplorer>(p => p.Add(x => x.ShowSearch, false));

        Assert.DoesNotContain("Buscar token", cut.Markup);
    }

    #endregion

    #region XFComponentGenerator

    [Fact]
    public void ComponentGenerator_RendersDefault()
    {
        var cut = Render<XFComponentGenerator>();

        Assert.Contains("xf-component-generator", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Component Generator", cut.Markup);
    }

    [Fact]
    public void ComponentGenerator_ShowsForm()
    {
        var cut = Render<XFComponentGenerator>();

        Assert.Contains("Nome do Componente", cut.Markup);
        Assert.Contains("Namespace", cut.Markup);
        Assert.Contains("Tipo", cut.Markup);
        Assert.Contains("Base Class", cut.Markup);
        Assert.Contains("Parametros", cut.Markup);
    }

    [Fact]
    public void ComponentGenerator_ShowsCheckboxes()
    {
        var cut = Render<XFComponentGenerator>();

        Assert.Contains("Incluir testes", cut.Markup);
        Assert.Contains("Incluir docs", cut.Markup);
        Assert.Contains("Tailwind-first", cut.Markup);
        Assert.Contains("Dark/Light", cut.Markup);
        Assert.Contains("IDisposable", cut.Markup);
        Assert.Contains("IAsyncDisposable", cut.Markup);
    }

    [Fact]
    public void ComponentGenerator_CustomTitle_Shown()
    {
        var cut = Render<XFComponentGenerator>(p => p.Add(x => x.Title, "Gerador"));

        Assert.Contains("Gerador", cut.Markup);
    }

    [Fact]
    public void ComponentGenerator_ShowsEmptyState()
    {
        var cut = Render<XFComponentGenerator>();

        Assert.Contains("Configure e clique", cut.Markup);
    }

    #endregion

    #region XFFigmaSync

    [Fact]
    public void FigmaSync_RendersDefault()
    {
        var cut = Render<XFFigmaSync>();

        Assert.Contains("xf-figma-sync", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Figma Sync", cut.Markup);
    }

    [Fact]
    public void FigmaSync_ShowsDisconnectedState()
    {
        var cut = Render<XFFigmaSync>();

        Assert.Contains("Desconectado", cut.Markup);
        Assert.Contains("Conectar", cut.Markup);
        Assert.Contains("Figma Access Token", cut.Markup);
        Assert.Contains("File Key", cut.Markup);
    }

    [Fact]
    public void FigmaSync_ShowsDirectionSelector()
    {
        var cut = Render<XFFigmaSync>();

        Assert.Contains("Importar do Figma", cut.Markup);
        Assert.Contains("Exportar para Figma", cut.Markup);
        Assert.Contains("Bidirecional", cut.Markup);
    }

    [Fact]
    public void FigmaSync_CustomTitle_Shown()
    {
        var cut = Render<XFFigmaSync>(p => p.Add(x => x.Title, "Sync Panel"));

        Assert.Contains("Sync Panel", cut.Markup);
    }

    [Fact]
    public void FigmaSync_GetStatus_ReturnsDisconnected()
    {
        var cut = Render<XFFigmaSync>();
        var component = cut.Instance;

        Assert.Equal(XFFigmaSyncStatus.Disconnected, component.GetStatus());
    }

    #endregion

    #region XFStorybook

    [Fact]
    public void Storybook_RendersDefault()
    {
        var cut = Render<XFStorybook>();

        Assert.Contains("xf-storybook", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("XForge Storybook", cut.Markup);
    }

    [Fact]
    public void Storybook_ShowsSearchInput()
    {
        var cut = Render<XFStorybook>(p => p.Add(x => x.ShowSearch, true));

        Assert.Contains("Buscar componente", cut.Markup);
    }

    [Fact]
    public void Storybook_ShowsComponents()
    {
        var components = new List<XFStorybookComponent>
        {
            new() { Name = "XFButton", Category = "Componentes", Description = "Botao", Status = XFStorybookComponentStatus.Stable, ParameterCount = 12, TestCount = 45 },
            new() { Name = "XFCard", Category = "Componentes", Description = "Card", Status = XFStorybookComponentStatus.Stable, ParameterCount = 8, TestCount = 20 }
        };
        var cut = Render<XFStorybook>(p => p.Add(x => x.Components, components));

        Assert.Contains("XFButton", cut.Markup);
        Assert.Contains("XFCard", cut.Markup);
        Assert.Contains("2 componentes", cut.Markup);
    }

    [Fact]
    public void Storybook_ShowsCategoryFilter()
    {
        var components = new List<XFStorybookComponent>
        {
            new() { Name = "XFButton", Category = "Componentes", Description = "Botao", Status = XFStorybookComponentStatus.Stable, ParameterCount = 12, TestCount = 45 }
        };
        var cut = Render<XFStorybook>(p =>
        {
            p.Add(x => x.ShowCategoryFilter, true);
            p.Add(x => x.Components, components);
        });

        Assert.Contains("Todas", cut.Markup);
    }

    [Fact]
    public void Storybook_ShowsStatusBadge_ForBeta()
    {
        var components = new List<XFStorybookComponent>
        {
            new() { Name = "XFTest", Category = "Test", Description = "Test", Status = XFStorybookComponentStatus.Beta, ParameterCount = 5, TestCount = 10 }
        };
        var cut = Render<XFStorybook>(p => p.Add(x => x.Components, components));

        Assert.Contains("Beta", cut.Markup);
    }

    [Fact]
    public void Storybook_ShowsEmptyState()
    {
        var cut = Render<XFStorybook>();

        Assert.Contains("Selecione um componente", cut.Markup);
    }

    #endregion
}
