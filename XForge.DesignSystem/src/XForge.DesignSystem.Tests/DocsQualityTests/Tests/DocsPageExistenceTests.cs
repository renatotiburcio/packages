using Xunit;
using XForge.DesignSystem.Docs.QualityTests.Infrastructure;

namespace XForge.DesignSystem.Docs.QualityTests.Tests;

public class DocsPageExistenceTests
{
    [Fact]
    public void Every_Public_XForge_Component_Should_Have_Docs_Page()
    {
        var components = ComponentApiCatalog.All;
        var docsPages = DocsFileCatalog.All.DocsPages;
        var docNames = docsPages.Select(d => d.ComponentName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = new List<string>();
        foreach (var comp in components)
        {
            if (AllowedDocsExceptions.ExemptComponents.Contains(comp.Name)) continue;
            if (!docNames.Contains(comp.Name))
                missing.Add($"{comp.Name} esperado em {comp.Name.Replace("XF", "")}Docs.razor");
        }
        Assert.True(missing.Count == 0, $"Sem docs:\n{string.Join("\n", missing)}\nTotal: {missing.Count}");
    }
}
