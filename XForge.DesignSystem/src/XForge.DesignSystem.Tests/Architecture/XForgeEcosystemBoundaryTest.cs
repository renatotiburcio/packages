using System.Xml.Linq;
using Xunit;

namespace XForge.DesignSystem.Tests.Architecture;

/// <summary>
/// Guardas de arquitetura do ecossistema depois da reorganizacao (2026-10-30).
///
/// Estrutura vigente:
///   Packages/
///     XForge.DesignSystem/
///       src/XForge.DesignSystem/            (nucleo, RCL)
///       src/XForge.DesignSystem.Shared/     (base compartilhada)
///       src/XForge.DesignSystem.Tests/
///       docs/app/XForge.DesignSystem.Docs.Shared/   (shell do manual)
///       docs/app/XForge.DesignSystem.Docs/          (manual + demonstracao)
///
/// O satelite XForge.DesignSystem.AI e o app XForge.DesignSystem.Docs.AI foram
/// descartados; os testes abaixo falham se alguem reintroduzir essa dependencia.
/// </summary>
public sealed class XForgeEcosystemBoundaryTest
{
    [Fact]
    public void CoreDesignSystem_ShouldOnlyReferenceSharedFoundation()
    {
        var project = LoadCoreProject();
        var references = GetProjectReferenceNames(project);

        // O nucleo e uma RCL que depende apenas da fundacao compartilhada:
        // sem AI, sem Docs, sem projetos externos.
        Assert.Contains("XForge.DesignSystem.Shared.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.AI.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.Docs.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.Docs.Shared.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.Docs.AI.csproj", references);
    }

    [Fact]
    public void DocsSharedPackage_ShouldReferenceCoreShellAndSharedFoundation()
    {
        var project = LoadProject(GetDocsAppRoot(), "XForge.DesignSystem.Docs.Shared", "XForge.DesignSystem.Docs.Shared.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.Shared.csproj", references);
        Assert.Contains("XForge.DesignSystem.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.AI.csproj", references);
    }

    [Fact]
    public void DocsApp_ShouldUseSharedDesignSystemShellLayout()
    {
        var docsRoot = GetDocsAppRoot();
        var sharedLayout = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsMainLayout.razor");
        var sharedShell = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Components", "XForgeDocsShell.razor");
        var app = Path.Combine(docsRoot, "XForge.DesignSystem.Docs", "App.razor");

        Assert.True(File.Exists(sharedLayout), $"Expected shared docs layout at {sharedLayout}.");

        var layoutContent = File.ReadAllText(sharedLayout);
        Assert.Contains("<XFAppShell", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsRail", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsSidebar", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsTopBar", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFDialogProvider", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFDrawerProvider", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFToastProvider", layoutContent, StringComparison.Ordinal);

        var appContent = File.ReadAllText(app);
        Assert.Contains("DefaultLayout=\"@typeof(XForge.DesignSystem.Docs.Shared.Components.Layout.XForgeDocsMainLayout)\"", appContent, StringComparison.Ordinal);

        // O shell compartilhado compoe o chrome, nao duplica o AppShell.
        var sharedShellContent = File.ReadAllText(sharedShell);
        Assert.DoesNotContain("<XFAppShell", sharedShellContent, StringComparison.Ordinal);
        Assert.Contains("<XFTypography", sharedShellContent, StringComparison.Ordinal);
        Assert.Contains("<XFBadge", sharedShellContent, StringComparison.Ordinal);
    }

    [Fact]
    public void SharedDocsChrome_ShouldPreferDesignSystemComponentsForInteractiveElements()
    {
        var topBarFile = Path.Combine(GetDocsAppRoot(), "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsTopBar.razor");
        var topBarContent = File.ReadAllText(topBarFile);

        Assert.Contains("<XFAppBar", topBarContent, StringComparison.Ordinal);
        Assert.Contains("<XFTextInput", topBarContent, StringComparison.Ordinal);
        Assert.Contains("<XFButton", topBarContent, StringComparison.Ordinal);
        Assert.DoesNotContain("<button", topBarContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<input", topBarContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SharedDocsShell_ShouldUseCategoryRailAndActiveCategorySidebar()
    {
        var docsRoot = GetDocsAppRoot();
        var navigationFile = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Navigation", "XForgeDocsNavigation.cs");
        var sidebarFile = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsSidebar.razor");
        var coreProgram = Path.Combine(docsRoot, "XForge.DesignSystem.Docs", "Program.cs");

        var navigationContent = File.ReadAllText(navigationFile);
        Assert.Contains("Rail(\"components\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"display\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"forms\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"tokens\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("group.Select(item => item.Href)", navigationContent, StringComparison.Ordinal);

        var sidebarContent = File.ReadAllText(sidebarFile);
        Assert.Contains("CurrentSection", sidebarContent, StringComparison.Ordinal);
        Assert.Contains("CurrentMenuItems", sidebarContent, StringComparison.Ordinal);
        Assert.DoesNotContain("@foreach (var section in Sections)", sidebarContent, StringComparison.Ordinal);

        var programContent = File.ReadAllText(coreProgram);
        Assert.Contains("options.Brands[\"components\"]", programContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"display\"]", programContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"forms\"]", programContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"tokens\"]", programContent, StringComparison.Ordinal);
    }

    [Fact]
    public void BrandThemeCssRules_ShouldLiveInDesignSystemCss()
    {
        // Decisao de arquitetura: as regras de marca (:root[data-brand=...]) vivem no CSS
        // do nucleo, carregado via <link>; o app.css do manual só tem Tailwind.
        var coreCss = Path.Combine(GetCoreRoot(), "wwwroot", "css", "xf-designsystem.css");
        var coreContent = File.ReadAllText(coreCss);

        Assert.Contains(":root[data-brand=\"brand-components\"]", coreContent, StringComparison.Ordinal);
        Assert.Contains(":root[data-brand=\"brand-tokens\"]", coreContent, StringComparison.Ordinal);
        Assert.Contains("[data-theme=\"dark\"]", coreContent, StringComparison.Ordinal);

        var docsAppCss = Path.Combine(GetDocsAppRoot(), "XForge.DesignSystem.Docs", "wwwroot", "app.css");
        var docsCssContent = File.ReadAllText(docsAppCss);

        // O manual nao duplica as regras de marca.
        Assert.DoesNotContain("[data-brand=\"brand-components\"]", docsCssContent, StringComparison.Ordinal);
        Assert.DoesNotContain("[data-brand=\"brand-tokens\"]", docsCssContent, StringComparison.Ordinal);
    }

    [Fact]
    public void LayoutShouldNotInjectBrandCssAtRuntime()
    {
        var layoutCode = Path.Combine(GetDocsAppRoot(), "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsMainLayout.razor.cs");

        var layoutContent = File.ReadAllText(layoutCode);
        // Brand CSS e estatico no app.css - sem injecao em runtime.
        Assert.DoesNotContain("InjectBrandCssRulesAsync", layoutContent, StringComparison.Ordinal);
        Assert.DoesNotContain("BuildAllBrandThemeCssRules", layoutContent, StringComparison.Ordinal);
    }

    [Fact]
    public void RailComponent_ShouldUseCssVariableBridgeClasses()
    {
        var coreRoot = GetCoreRoot();
        var railCode = Path.Combine(coreRoot, "Components", "Layout", "Shell", "XFRail.razor.cs");
        var railItemCode = Path.Combine(coreRoot, "Components", "Layout", "Shell", "XFRailItem.razor.cs");

        var railContent = File.ReadAllText(railCode);
        // Rail usa bg-primary (classe ponte de variavel CSS), nao style inline.
        Assert.Contains("bg-primary", railContent, StringComparison.Ordinal);
        Assert.DoesNotContain("BuildBrandCssCustomProperties", railContent, StringComparison.Ordinal);

        var railItemContent = File.ReadAllText(railItemCode);
        Assert.Contains("text-primary-contrast", railItemContent, StringComparison.Ordinal);
        Assert.Contains("bg-primary-contrast", railItemContent, StringComparison.Ordinal);
    }

    [Fact]
    public void CoreDocsApp_ShouldNotReferenceCommercialAiPackage()
    {
        var project = LoadProject(GetDocsAppRoot(), "XForge.DesignSystem.Docs", "XForge.DesignSystem.Docs.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.csproj", references);
        Assert.Contains("XForge.DesignSystem.Docs.Shared.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.AI.csproj", references);

        // O satelite AI foi descartado: o projeto nao pode voltar a existir.
        var aiProject = Path.Combine(GetRepositoryRoot().FullName, "XForge.DesignSystem", "src", "XForge.DesignSystem.AI", "XForge.DesignSystem.AI.csproj");
        Assert.False(File.Exists(aiProject), $"O satelite AI foi descartado; nao recrie {aiProject}.");
    }

    [Fact]
    public void DocsCatalogRoutes_ShouldHaveMigratedPages()
    {
        var docsRoot = GetDocsAppRoot();
        var catalogFile = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Navigation", "XForgeDocsCatalog.cs");
        var catalogRoutes = ExtractCatalogRoutes(catalogFile);
        Assert.NotEmpty(catalogRoutes);

        var pagesDirectory = Path.Combine(docsRoot, "XForge.DesignSystem.Docs", "Components", "Pages");
        var pageRoutes = GetPageRoutes(Directory.GetFiles(pagesDirectory, "*.razor", SearchOption.AllDirectories));
        Assert.NotEmpty(pageRoutes);

        // Nenhum item de navegacao pode apontar para rota sem pagina.
        var broken = catalogRoutes.Where(route => !pageRoutes.Contains(route, StringComparer.OrdinalIgnoreCase)).ToArray();
        Assert.True(broken.Length == 0, $"Rotas do catalogo sem pagina: {string.Join(", ", broken)}");
    }

    [Fact]
    public void DocsPages_ShouldUseSharedStandardSections()
    {
        var docsRoot = GetDocsAppRoot();
        var pagesDirectory = Path.Combine(docsRoot, "XForge.DesignSystem.Docs", "Components", "Pages");
        var pages = Directory.GetFiles(pagesDirectory, "*Docs.razor", SearchOption.AllDirectories);
        Assert.NotEmpty(pages);

        foreach (var page in pages)
        {
            var content = File.ReadAllText(page);

            // Toda pagina compoe o shell compartilhado.
            Assert.Contains("<XForgeDocsShell", content, StringComparison.Ordinal);

            // Pagina que documenta API precisa faze-lo pelo componente padrao ou pelo
            // helper ApiTable — as duas formas suportadas. Paginas de sub-componente
            // (AccordionItem, RailItem, TableCell...) nao tem secao de API.
            if (content.Contains("@Section(\"API\"", StringComparison.Ordinal))
            {
                Assert.True(
                    content.Contains("<XForgeDocsPageStandard", StringComparison.Ordinal) ||
                    content.Contains("ApiTable", StringComparison.Ordinal),
                    $"Pagina {page} declara secao de API sem usar XForgeDocsPageStandard ou ApiTable.");
            }
        }

        var standardComponent = Path.Combine(docsRoot, "XForge.DesignSystem.Docs.Shared", "Components", "XForgeDocsPageStandard.razor");
        var standardContent = File.ReadAllText(standardComponent);
        Assert.Contains("<XFCode", standardContent, StringComparison.Ordinal);
        Assert.Contains("<XFTable", standardContent, StringComparison.Ordinal);
        Assert.Contains("Acessibilidade", standardContent, StringComparison.Ordinal);
        Assert.Contains("Teclado", standardContent, StringComparison.Ordinal);
        Assert.Contains("Performance", standardContent, StringComparison.Ordinal);
        Assert.Contains("Changelog", standardContent, StringComparison.Ordinal);
    }

    [Fact]
    public void DocsApp_ShouldDeclareTailwindPipeline()
    {
        var projectDirectory = Path.Combine(GetDocsAppRoot(), "XForge.DesignSystem.Docs");
        var projectFile = Path.Combine(projectDirectory, "XForge.DesignSystem.Docs.csproj");
        var packageFile = Path.Combine(projectDirectory, "package.json");
        var tailwindEntry = Path.Combine(projectDirectory, "wwwroot", "app.css");
        var generatedCss = Path.Combine(projectDirectory, "wwwroot", "css", "app.css");
        var indexHtml = Path.Combine(projectDirectory, "wwwroot", "index.html");

        Assert.True(File.Exists(packageFile), $"Expected Tailwind package.json at {packageFile}.");
        Assert.True(File.Exists(tailwindEntry), $"Expected Tailwind input at {tailwindEntry}.");
        Assert.True(File.Exists(generatedCss), $"Expected generated Tailwind CSS at {generatedCss}.");

        var projectContent = File.ReadAllText(projectFile);
        Assert.Contains("BuildTailwindCSS", projectContent, StringComparison.Ordinal);

        var packageContent = File.ReadAllText(packageFile);
        Assert.Contains("\"build:css\"", packageContent, StringComparison.Ordinal);
        Assert.Contains("tailwindcss -i ./wwwroot/app.css -o ./wwwroot/css/app.css --minify", packageContent, StringComparison.Ordinal);

        var tailwindContent = File.ReadAllText(tailwindEntry);
        Assert.Contains("@import \"tailwindcss\";", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("--color-bg:", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("--color-surface-100:", tailwindContent, StringComparison.Ordinal);

        // Os globs do @source precisam resolver para pastas reais: foi exatamente
        // isso que quebrou na migracao (caminhos relativos do layout antigo).
        var wwwroot = new DirectoryInfo(Path.Combine(projectDirectory, "wwwroot"));
        foreach (var source in File.ReadLines(tailwindEntry)
                 .Select(line => line.Trim())
                 .Where(line => line.StartsWith("@source \"", StringComparison.Ordinal))
                 .Select(ExtractQuotedValue))
        {
            var globDirectory = source[..source.IndexOf("**", StringComparison.Ordinal)];
            var resolved = Path.GetFullPath(Path.Combine(wwwroot.FullName, globDirectory));
            Assert.True(Directory.Exists(resolved), $"@source '{source}' nao resolve para {resolved}.");
        }

        var indexContent = File.ReadAllText(indexHtml);
        Assert.Contains("css/app.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-designsystem.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-select.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-scrollbar.css", indexContent, StringComparison.Ordinal);
    }

    [Fact]
    public void DocsApp_ShouldRegisterShellServicesAndLegacyLoadingExperience()
    {
        var projectDirectory = Path.Combine(GetDocsAppRoot(), "XForge.DesignSystem.Docs");
        var programFile = Path.Combine(projectDirectory, "Program.cs");
        var indexHtml = Path.Combine(projectDirectory, "wwwroot", "index.html");
        var favicon = Path.Combine(projectDirectory, "wwwroot", "favicon.png");

        var programContent = File.ReadAllText(programFile);
        Assert.Contains("AddScoped<XFDialogService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFDrawerService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFToastService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFNavigationGuardService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddSingleton<XFBreakpointService>", programContent, StringComparison.Ordinal);

        var indexContent = File.ReadAllText(indexHtml);
        Assert.Contains("Carregando XForge", indexContent, StringComparison.Ordinal);
        Assert.Contains("blazor-error-ui", indexContent, StringComparison.Ordinal);
        Assert.Contains("error-countdown", indexContent, StringComparison.Ordinal);
        Assert.Contains("favicon.png", indexContent, StringComparison.Ordinal);
        Assert.True(File.Exists(favicon), $"Expected favicon at {favicon}.");
    }

    private static string[] ExtractCatalogRoutes(string catalogFile)
    {
        return File.ReadLines(catalogFile)
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("new(\"/", StringComparison.Ordinal))
            .Select(ExtractQuotedValue)
            .Where(route => !string.IsNullOrWhiteSpace(route))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()!;
    }

    private static string ExtractQuotedValue(string line)
    {
        var firstQuote = line.IndexOf('"', StringComparison.Ordinal);

        if (firstQuote < 0)
        {
            return string.Empty;
        }

        var secondQuote = line.IndexOf('"', firstQuote + 1);

        return secondQuote < 0 ? string.Empty : line[(firstQuote + 1)..secondQuote];
    }

    private static XDocument LoadCoreProject()
    {
        var projectPath = Path.Combine(GetCoreRoot(), "XForge.DesignSystem.csproj");

        Assert.True(File.Exists(projectPath), $"Expected project file at {projectPath}.");

        return XDocument.Load(projectPath);
    }

    private static XDocument LoadProject(string projectRoot, string directoryName, string projectFileName)
    {
        var projectPath = Path.Combine(projectRoot, directoryName, projectFileName);

        Assert.True(File.Exists(projectPath), $"Expected project file at {projectPath}.");

        return XDocument.Load(projectPath);
    }

    private static string[] GetProjectReferenceNames(XDocument project)
    {
        return project
            .Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFileName(value!))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string[] GetPageRoutes(IEnumerable<string> pageFiles)
    {
        const string marker = "@page";

        return pageFiles
            .SelectMany(path => File.ReadLines(path)
                .Where(line => line.TrimStart().StartsWith(marker, StringComparison.Ordinal))
                .Select(ExtractQuotedValue)
                .OfType<string>()
                .Where(route => !string.IsNullOrWhiteSpace(route)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>Raiz do repositorio (Packages/).</summary>
    private static DirectoryInfo GetRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, "XForge.DesignSystem")) &&
                File.Exists(Path.Combine(current.FullName, "NuGet.config")))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing XForge.DesignSystem and NuGet.config.");
    }

    /// <summary>Pasta do nucleo (src/XForge.DesignSystem).</summary>
    private static string GetCoreRoot()
        => Path.Combine(GetRepositoryRoot().FullName, "XForge.DesignSystem", "src", "XForge.DesignSystem");

    /// <summary>Pasta dos apps de documentacao (docs/app).</summary>
    private static string GetDocsAppRoot()
        => Path.Combine(GetRepositoryRoot().FullName, "XForge.DesignSystem", "docs", "app");
}
