using System.Xml.Linq;
using Xunit;

namespace XForge.DesignSystem.Tests.Architecture;

public sealed class XForgeEcosystemBoundaryTest
{
    [Fact]
    public void CoreDesignSystem_ShouldHaveNoExternalProjectReferences()
    {
        var project = LoadProject("XForge.DesignSystem", "XForge.DesignSystem.csproj");
        var references = GetProjectReferenceNames(project);

        // XForge.DesignSystem is a standalone Razor Class Library with no project references.
        Assert.Empty(references);
    }

    [Fact]
    public void CommercialAiPackage_ShouldReferenceCoreAndSharedFoundation()
    {
        var project = LoadProject("XForge.DesignSystem.AI", "XForge.DesignSystem.AI.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.Shared.csproj", references);
        Assert.Contains("XForge.DesignSystem.csproj", references);
    }

    [Fact]
    public void DocsSharedPackage_ShouldReferenceCoreShellAndSharedFoundation()
    {
        var project = LoadProject("XForge.DesignSystem.Docs.Shared", "XForge.DesignSystem.Docs.Shared.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.Shared.csproj", references);
        Assert.Contains("XForge.DesignSystem.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.AI.csproj", references);
    }

    [Fact]
    public void SplitDocsApps_ShouldUseSharedDesignSystemShellLayout()
    {
        var repositoryRoot = GetRepositoryRoot();
        var sharedLayout = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsMainLayout.razor");
        var sharedShell = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "XForgeDocsShell.razor");
        var coreApp = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs", "App.razor");
        var aiApp = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.AI", "App.razor");

        Assert.True(File.Exists(sharedLayout), $"Expected shared docs layout at {sharedLayout}.");

        var layoutContent = File.ReadAllText(sharedLayout);
        Assert.Contains("<XFAppShell", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsRail", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsSidebar", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XForgeDocsTopBar", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFDialogProvider", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFDrawerProvider", layoutContent, StringComparison.Ordinal);
        Assert.Contains("<XFToastProvider", layoutContent, StringComparison.Ordinal);

        Assert.Contains("DefaultLayout=\"@typeof(XForge.DesignSystem.Docs.Shared.Components.Layout.XForgeDocsMainLayout)\"", File.ReadAllText(coreApp), StringComparison.Ordinal);
        Assert.Contains("DefaultLayout=\"@typeof(XForge.DesignSystem.Docs.Shared.Components.Layout.XForgeDocsMainLayout)\"", File.ReadAllText(aiApp), StringComparison.Ordinal);
        var sharedShellContent = File.ReadAllText(sharedShell);
        Assert.DoesNotContain("<XFAppShell", sharedShellContent, StringComparison.Ordinal);
        Assert.Contains("<XFTypography", sharedShellContent, StringComparison.Ordinal);
        Assert.Contains("<XFBadge", sharedShellContent, StringComparison.Ordinal);
    }

    [Fact]
    public void SharedDocsChrome_ShouldPreferDesignSystemComponentsForInteractiveElements()
    {
        var repositoryRoot = GetRepositoryRoot();
        var topBarFile = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsTopBar.razor");
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
        var repositoryRoot = GetRepositoryRoot();
        var navigationFile = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Navigation", "XForgeDocsNavigation.cs");
        var sidebarFile = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsSidebar.razor");
        var coreProgram = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs", "Program.cs");
        var aiProgram = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.AI", "Program.cs");

        var navigationContent = File.ReadAllText(navigationFile);
        Assert.Contains("Rail(\"components\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"display\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"forms\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("Rail(\"tokens\"", navigationContent, StringComparison.Ordinal);
        Assert.Contains("GetAiCategory(item.Group)", navigationContent, StringComparison.Ordinal);
        Assert.Contains("GetAiCategoryTitle(group.Key)", navigationContent, StringComparison.Ordinal);
        Assert.Contains("group.Select(item => item.Href)", navigationContent, StringComparison.Ordinal);

        var sidebarContent = File.ReadAllText(sidebarFile);
        Assert.Contains("CurrentSection", sidebarContent, StringComparison.Ordinal);
        Assert.Contains("CurrentMenuItems", sidebarContent, StringComparison.Ordinal);
        Assert.DoesNotContain("@foreach (var section in Sections)", sidebarContent, StringComparison.Ordinal);

        var coreProgramContent = File.ReadAllText(coreProgram);
        Assert.Contains("options.Brands[\"components\"]", coreProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"display\"]", coreProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"forms\"]", coreProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"tokens\"]", coreProgramContent, StringComparison.Ordinal);

        var aiProgramContent = File.ReadAllText(aiProgram);
        Assert.Contains("options.Brands[\"ai-core\"]", aiProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"ai-agents\"]", aiProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"ai-business\"]", aiProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"ai-multimodal\"]", aiProgramContent, StringComparison.Ordinal);
        Assert.Contains("options.Brands[\"ai-governance\"]", aiProgramContent, StringComparison.Ordinal);
        Assert.DoesNotContain("options.Brands[\"ai-prompting\"]", aiProgramContent, StringComparison.Ordinal);
    }

    [Fact]
    public void BrandThemeCssRules_ShouldBeDeclaredInAppCss()
    {
        var repositoryRoot = GetRepositoryRoot();

        // Docs app.css must contain brand CSS blocks
        var docsAppCss = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs", "wwwroot", "app.css");
        var docsCssContent = File.ReadAllText(docsAppCss);
        Assert.Contains("[data-brand=\"brand-components\"]", docsCssContent, StringComparison.Ordinal);
        Assert.Contains("[data-brand=\"brand-tokens\"]", docsCssContent, StringComparison.Ordinal);
        Assert.Contains("[data-theme=\"dark\"][data-brand=\"brand-components\"]", docsCssContent, StringComparison.Ordinal);

        // AI app.css must contain AI brand CSS blocks
        var aiAppCss = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.AI", "wwwroot", "app.css");
        var aiCssContent = File.ReadAllText(aiAppCss);
        Assert.Contains("[data-brand=\"brand-ai-core\"]", aiCssContent, StringComparison.Ordinal);
        Assert.Contains("[data-brand=\"brand-ai-governance\"]", aiCssContent, StringComparison.Ordinal);
        Assert.Contains("[data-theme=\"dark\"][data-brand=\"brand-ai-core\"]", aiCssContent, StringComparison.Ordinal);
    }

    [Fact]
    public void LayoutShouldNotInjectBrandCssAtRuntime()
    {
        var repositoryRoot = GetRepositoryRoot();
        var layoutCode = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "Layout", "XForgeDocsMainLayout.razor.cs");

        var layoutContent = File.ReadAllText(layoutCode);
        // Brand CSS is now static in app.css - no runtime injection
        Assert.DoesNotContain("InjectBrandCssRulesAsync", layoutContent, StringComparison.Ordinal);
        Assert.DoesNotContain("BuildAllBrandThemeCssRules", layoutContent, StringComparison.Ordinal);
    }

    [Fact]
    public void RailComponent_ShouldUseCssVariableBridgeClasses()
    {
        var repositoryRoot = GetRepositoryRoot();
        var railCode = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem", "Components", "Layout", "Shell", "XFRail.razor.cs");
        var railItemCode = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem", "Components", "Layout", "Shell", "XFRailItem.razor.cs");

        var railContent = File.ReadAllText(railCode);
        // Rail uses bg-primary (CSS variable bridge class), not inline style
        Assert.Contains("bg-primary", railContent, StringComparison.Ordinal);
        Assert.DoesNotContain("BuildBrandCssCustomProperties", railContent, StringComparison.Ordinal);

        var railItemContent = File.ReadAllText(railItemCode);
        // Rail items use CSS variable bridge classes for brand-aware contrast
        Assert.Contains("text-primary-contrast", railItemContent, StringComparison.Ordinal);
        Assert.Contains("bg-primary-contrast", railItemContent, StringComparison.Ordinal);
    }

    [Fact]
    public void CoreDocsApp_ShouldNotReferenceCommercialAiPackage()
    {
        var project = LoadProject("XForge.DesignSystem.Docs", "XForge.DesignSystem.Docs.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.csproj", references);
        Assert.Contains("XForge.DesignSystem.Docs.Shared.csproj", references);
        Assert.DoesNotContain("XForge.DesignSystem.AI.csproj", references);
    }

    [Fact]
    public void AiDocsApp_ShouldReferenceCommercialAiPackage()
    {
        var project = LoadProject("XForge.DesignSystem.Docs.AI", "XForge.DesignSystem.Docs.AI.csproj");
        var references = GetProjectReferenceNames(project);

        Assert.Contains("XForge.DesignSystem.csproj", references);
        Assert.Contains("XForge.DesignSystem.AI.csproj", references);
        Assert.Contains("XForge.DesignSystem.Docs.Shared.csproj", references);
    }

    [Fact]
    public void DocsCatalogRoutes_ShouldHaveMigratedPagesForSeedItems()
    {
        var repositoryRoot = GetRepositoryRoot();
        var corePages = Directory.GetFiles(Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs"), "*.razor", SearchOption.AllDirectories);
        var aiPages = Directory.GetFiles(Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.AI"), "*.razor", SearchOption.AllDirectories);

        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/button\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/split-button\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/fab\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/icon\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/animate\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/card\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/badge\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/text-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/textarea\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/select\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/data-grid\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/table\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/layout/app-shell\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/dialog\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/toast\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/tabs\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/alert\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/breadcrumb\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/avatar\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/barcode\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/qrcode\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/code\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/image\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/list\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/markdown\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/video\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/chart\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/sparkline\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/gantt\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/maps\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/sankey\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/scheduler\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/pdf-viewer\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/pivot-datagrid\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/timeline\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/chip\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/progress\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/skeleton\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/progress-ring\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/chat\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/lottie\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/feedback/splash-screen\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/date-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/date-picker\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/edit-form\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/field\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/radio\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/rating\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/upload\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/validation\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/color-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/color-picker\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/combobox\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/multi-select\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/transfer-list\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/time-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/time-span\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/speech-to-text\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/one-time-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/password-strength\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/range-slider\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/input-mask\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/captcha\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/signature-pad\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/accordion\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/overlays/drawer\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/overlays/dropdown-menu\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/overlays/popover\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/overlays/context-menu\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/overview\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/colors\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/spacing\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/borders\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/shadows\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/motion\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/tokens/typography\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/pagination\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/menu\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/toolbar\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/steps\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/navigation/toc\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/tree-view\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/collapse\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/focus-trap\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/key-code\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/drag-drop\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/sortable-list\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/carousel\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/divider\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/autocomplete\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/display/calendar\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/components/tooltip\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/checkbox\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/switch\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/number-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/currency-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/file-input\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/slider\"", StringComparison.Ordinal));
        Assert.Contains(corePages, path => File.ReadAllText(path).Contains("@page \"/forms/search\"", StringComparison.Ordinal));

        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/prompt-box\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/chat\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/workflow-designer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/workflow-executor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/source-citations\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/audit-timeline\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/providers\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/human-approval\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/validation-summary\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/risk-badge\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/document-reader\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/command-palette\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/dashboard-insight\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/suggestion-panel\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/explain-button\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/rewrite-button\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/agent-console\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/accounting-assistant\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/sql-query-builder\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/vision-panel\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/voice-assistant\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/financial-conciliator\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/data-grid-assistant\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/report-builder\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/form-assistant\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/nfe-analyzer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/sales-goal-advisor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/stock-advisor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/video-analyzer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/multimodal-chat\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/memory-viewer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/avatar\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/agent-personality\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/conversation-context\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/proactive-insight\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/onboarding-wizard\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/automation-rule\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/batch-processor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/scheduler\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/virtual-scroller\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/lazy-component\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/performance-monitor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/bundle-analyzer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/memory-profiler\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/theme-editor\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/design-token-explorer\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/component-generator\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/figma-sync\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/storybook\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/auth-provider\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/rbac-component\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/audit-logger\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/compliance-panel\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/micro-frontend\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/cross-platform\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/device-bridge\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/gesture-handler\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/offline-storage\"", StringComparison.Ordinal));
        Assert.Contains(aiPages, path => File.ReadAllText(path).Contains("@page \"/ai/responsive-container\"", StringComparison.Ordinal));
    }

    [Fact]
    public void SplitDocsPages_ShouldUseSharedStandardSections()
    {
        var repositoryRoot = GetRepositoryRoot();
        var splitPages = Directory
            .GetFiles(Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs"), "*.razor", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.AI"), "*.razor", SearchOption.AllDirectories))
            .Where(path => path.EndsWith("Docs.razor", StringComparison.OrdinalIgnoreCase) || path.EndsWith("OverviewDocs.razor", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.NotEmpty(splitPages);
        foreach (var page in splitPages)
        {
            var content = File.ReadAllText(page);
            Assert.Contains("<XForgeDocsShell", content, StringComparison.Ordinal);
            // Allow either shared standard component OR premium inline structure
            Assert.True(
                content.Contains("<XForgeDocsPageStandard", StringComparison.Ordinal) ||
                content.Contains("private RenderFragment ApiTable", StringComparison.Ordinal),
                $"Page {page} should use XForgeDocsPageStandard or inline premium structure with ApiTable");
        }

        var standardComponent = Path.Combine(repositoryRoot.FullName, "XForge.DesignSystem.Docs.Shared", "Components", "XForgeDocsPageStandard.razor");
        var standardContent = File.ReadAllText(standardComponent);
        Assert.Contains("<XFCode", standardContent, StringComparison.Ordinal);
        Assert.Contains("<XFTable", standardContent, StringComparison.Ordinal);
        Assert.Contains("Acessibilidade", standardContent, StringComparison.Ordinal);
        Assert.Contains("Teclado", standardContent, StringComparison.Ordinal);
        Assert.Contains("Performance", standardContent, StringComparison.Ordinal);
        Assert.Contains("Changelog", standardContent, StringComparison.Ordinal);
    }

    [Fact]
    public void NewDocsApps_ShouldBuildTailwindAndScanSharedDocsSources()
    {
        var repositoryRoot = GetRepositoryRoot();

        AssertDocsTailwindPipeline(repositoryRoot, "XForge.DesignSystem.Docs", includeAiSources: false);
        AssertDocsTailwindPipeline(repositoryRoot, "XForge.DesignSystem.Docs.AI", includeAiSources: true);
    }

    private static XDocument LoadProject(string directoryName, string projectFileName)
    {
        var repositoryRoot = GetRepositoryRoot();
        var projectPath = Path.Combine(repositoryRoot.FullName, directoryName, projectFileName);

        Assert.True(File.Exists(projectPath), $"Expected project file at {projectPath}.");

        return XDocument.Load(projectPath);
    }

    private static DirectoryInfo GetRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "XForge.slnx")))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing XForge.slnx.");
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
                .Select(ExtractRoute)
                .OfType<string>()
                .Where(route => !string.IsNullOrWhiteSpace(route)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string? ExtractRoute(string line)
    {
        var firstQuote = line.IndexOf('"', StringComparison.Ordinal);

        if (firstQuote < 0)
        {
            return null;
        }

        var secondQuote = line.IndexOf('"', firstQuote + 1);

        return secondQuote < 0 ? null : line[(firstQuote + 1)..secondQuote];
    }

    private static void AssertDocsTailwindPipeline(DirectoryInfo repositoryRoot, string projectDirectoryName, bool includeAiSources)
    {
        var projectDirectory = Path.Combine(repositoryRoot.FullName, projectDirectoryName);
        var projectFile = Path.Combine(projectDirectory, $"{projectDirectoryName}.csproj");
        var packageFile = Path.Combine(projectDirectory, "package.json");
        var tailwindEntry = Path.Combine(projectDirectory, "wwwroot", "app.css");
        var generatedCss = Path.Combine(projectDirectory, "wwwroot", "css", "app.css");
        var indexHtml = Path.Combine(projectDirectory, "wwwroot", "index.html");

        Assert.True(File.Exists(packageFile), $"Expected Tailwind package.json at {packageFile}.");
        Assert.True(File.Exists(tailwindEntry), $"Expected Tailwind input at {tailwindEntry}.");
        Assert.True(File.Exists(generatedCss), $"Expected generated Tailwind CSS at {generatedCss}.");

        var projectContent = File.ReadAllText(projectFile);
        Assert.Contains("BuildTailwindCSS", projectContent, StringComparison.Ordinal);
        Assert.Contains("npm run build:css", projectContent, StringComparison.Ordinal);

        var packageContent = File.ReadAllText(packageFile);
        Assert.Contains("\"build:css\"", packageContent, StringComparison.Ordinal);
        Assert.Contains("tailwindcss -i ./wwwroot/app.css -o ./wwwroot/css/app.css --minify", packageContent, StringComparison.Ordinal);

        var tailwindContent = File.ReadAllText(tailwindEntry);
        Assert.Contains("@import \"tailwindcss\";", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("../../XForge.DesignSystem.Docs.Shared/**/*.razor", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("../../XForge.DesignSystem/**/*.razor", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("--color-bg:", tailwindContent, StringComparison.Ordinal);
        Assert.Contains("--color-surface-100:", tailwindContent, StringComparison.Ordinal);

        if (includeAiSources)
        {
            Assert.Contains("../../XForge.DesignSystem.AI/**/*.razor", tailwindContent, StringComparison.Ordinal);
        }

        var indexContent = File.ReadAllText(indexHtml);
        Assert.Contains("css/app.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-designsystem.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-select.css", indexContent, StringComparison.Ordinal);
        Assert.Contains("_content/XForge.DesignSystem/css/xf-scrollbar.css", indexContent, StringComparison.Ordinal);
    }

    [Fact]
    public void SplitDocsApps_ShouldRegisterShellServicesAndLegacyLoadingExperience()
    {
        var repositoryRoot = GetRepositoryRoot();

        AssertDocsRuntimeShellParity(repositoryRoot, "XForge.DesignSystem.Docs", includeAiServices: false);
        AssertDocsRuntimeShellParity(repositoryRoot, "XForge.DesignSystem.Docs.AI", includeAiServices: true);
    }

    private static void AssertDocsRuntimeShellParity(DirectoryInfo repositoryRoot, string projectDirectoryName, bool includeAiServices)
    {
        var projectDirectory = Path.Combine(repositoryRoot.FullName, projectDirectoryName);
        var programFile = Path.Combine(projectDirectory, "Program.cs");
        var indexHtml = Path.Combine(projectDirectory, "wwwroot", "index.html");
        var favicon = Path.Combine(projectDirectory, "wwwroot", "favicon.png");

        var programContent = File.ReadAllText(programFile);
        Assert.Contains("AddScoped<XFDialogService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFDrawerService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFToastService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddScoped<XFNavigationGuardService>", programContent, StringComparison.Ordinal);
        Assert.Contains("AddSingleton<XFBreakpointService>", programContent, StringComparison.Ordinal);

        if (includeAiServices)
        {
            Assert.Contains("AddXFAi()", programContent, StringComparison.Ordinal);
            Assert.Contains("AddXFAiMultiModal()", programContent, StringComparison.Ordinal);
        }

        var indexContent = File.ReadAllText(indexHtml);
        Assert.Contains("Carregando XForge", indexContent, StringComparison.Ordinal);
        Assert.Contains("blazor-error-ui", indexContent, StringComparison.Ordinal);
        Assert.Contains("error-countdown", indexContent, StringComparison.Ordinal);
        Assert.Contains("favicon.png", indexContent, StringComparison.Ordinal);
        Assert.True(File.Exists(favicon), $"Expected favicon copied from legacy docs at {favicon}.");
    }
}
