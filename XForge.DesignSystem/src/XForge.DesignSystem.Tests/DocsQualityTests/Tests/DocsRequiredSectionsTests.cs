using Xunit;
using XForge.DesignSystem.Docs.QualityTests.Infrastructure;

namespace XForge.DesignSystem.Docs.QualityTests.Tests;

public class DocsRequiredSectionsTests
{
    [Fact]
    public void Every_Docs_Page_Should_Contain_All_Required_Sections()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var missing = new List<string>();
            foreach (var section in DocumentationContract.RequiredSections)
            {
                if (!section.Aliases.Any(a => page.Content.Contains(a, StringComparison.OrdinalIgnoreCase)))
                    missing.Add(section.Id);
            }
            if (missing.Count > 0)
                failures.Add($"{page.FileName}: {string.Join(", ", missing)}");
        }
        Assert.True(failures.Count == 0, $"Paginas incompletas:\n{string.Join("\n", failures)}");
    }
}
