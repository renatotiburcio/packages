using Xunit;
using XForge.DesignSystem.Docs.QualityTests.Infrastructure;

namespace XForge.DesignSystem.Docs.QualityTests.Tests;

public class DocsAccessibilityCoverageTests
{
    [Fact]
    public void Interactive_Components_Should_Document_Accessibility()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var comp = ComponentApiCatalog.FindByName(page.ComponentName);
            if (comp == null) continue;
            var isInteractive = comp.EventCallbacks.Any() || comp.Name.Contains("Input") ||
                comp.Name.Contains("Button") || comp.Name.Contains("Select") ||
                comp.Name.Contains("Dialog") || comp.Name.Contains("Drawer");
            if (!isInteractive) continue;
            var a11yCount = DocumentationContract.AccessibilityTerms
                .Count(t => page.Content.Contains(t, StringComparison.OrdinalIgnoreCase));
            if (a11yCount < 3)
                failures.Add($"{page.FileName}: apenas {a11yCount} termos a11y (min: 3)");
        }
        Assert.True(failures.Count == 0, $"Sem docs a11y:\n{string.Join("\n", failures)}");
    }
}
