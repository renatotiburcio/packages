using Xunit;
using XForge.DesignSystem.Docs.QualityTests.Infrastructure;

namespace XForge.DesignSystem.Docs.QualityTests.Tests;

public class DocsDesignSystemUsageTests
{
    [Fact]
    public void No_Raw_Html_Tables_In_Docs()
    {
        var failures = DocsFileCatalog.All.DocsPages
            .Where(p => p.Content.Contains("<table", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.FileName).ToList();
        Assert.True(failures.Count == 0, $"Tabelas HTML raw:\n{string.Join("\n", failures)}");
    }
}

public class DocsExamplesTests
{
    [Fact]
    public void Every_Page_Should_Have_XFCode_Blocks()
    {
        var failures = DocsFileCatalog.All.DocsPages
            .Where(p => !p.Content.Contains("XFCode", StringComparison.OrdinalIgnoreCase) && p.LineCount > 50)
            .Select(p => $"{p.FileName} ({p.LineCount} linhas)").ToList();
        Assert.True(failures.Count == 0, $"Sem XFCode:\n{string.Join("\n", failures)}");
    }
}

public class DocsKeyboardCoverageTests
{
    [Fact]
    public void Interactive_Should_Have_Keyboard_Docs()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var comp = ComponentApiCatalog.FindByName(page.ComponentName);
            if (comp == null) continue;
            if (!comp.EventCallbacks.Any() && !comp.Name.Contains("Input") && !comp.Name.Contains("Button")) continue;
            if (!page.Content.Contains("keyboard", StringComparison.OrdinalIgnoreCase) &&
                !page.Content.Contains("Teclado", StringComparison.OrdinalIgnoreCase))
                failures.Add($"{page.FileName}");
        }
        Assert.True(failures.Count == 0, $"Sem docs de teclado:\n{string.Join("\n", failures)}");
    }
}

public class DocsOverviewPagePatternTests
{
    [Fact]
    public void Overview_Pages_Should_Have_Layout()
    {
        if (DocsFileCatalog.All.OverviewPages.Count == 0) return;
        var failures = DocsFileCatalog.All.OverviewPages
            .Where(p => !p.Content.Contains("grid", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.FileName).ToList();
        Assert.True(failures.Count == 0, $"Overview sem layout:\n{string.Join("\n", failures)}");
    }
}

public class DocsPerformanceCoverageTests
{
    [Fact]
    public void Complex_Components_Should_Have_Performance_Docs()
    {
        var complexPatterns = new[] { "DataGrid", "Chart", "Scheduler", "VirtualList" };
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            if (!complexPatterns.Any(p => page.ComponentName.Contains(p, StringComparison.OrdinalIgnoreCase))) continue;
            if (!page.Content.Contains("Performance", StringComparison.OrdinalIgnoreCase))
                failures.Add($"{page.FileName}");
        }
        Assert.True(failures.Count == 0, $"Sem docs de performance:\n{string.Join("\n", failures)}");
    }
}

public class DocsPublicApiCoverageTests
{
    [Fact]
    public void Every_Parameter_Should_Be_Documented()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var comp = ComponentApiCatalog.FindByName(page.ComponentName);
            if (comp == null) continue;
            var missing = comp.Parameters
                .Where(p => !p.IsEventCallback && !p.IsRenderFragment && !p.HasUnmatchedValues)
                .Where(p => !page.Content.Contains(p.Name, StringComparison.Ordinal))
                .Select(p => p.Name).ToList();
            if (missing.Any())
                failures.Add($"{page.FileName}: {string.Join(", ", missing)}");
        }
        Assert.True(failures.Count == 0, $"Params nao documentados:\n{string.Join("\n", failures)}");
    }

    [Fact]
    public void Every_EventCallback_Should_Be_Documented()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var comp = ComponentApiCatalog.FindByName(page.ComponentName);
            if (comp == null) continue;
            var missing = comp.EventCallbacks
                .Where(e => !page.Content.Contains(e.Name, StringComparison.Ordinal))
                .Select(e => e.Name).ToList();
            if (missing.Any())
                failures.Add($"{page.FileName}: {string.Join(", ", missing)}");
        }
        Assert.True(failures.Count == 0, $"Eventos nao documentados:\n{string.Join("\n", failures)}");
    }

    [Fact]
    public void Every_RenderFragment_Should_Be_Documented()
    {
        var failures = new List<string>();
        foreach (var page in DocsFileCatalog.All.DocsPages)
        {
            var comp = ComponentApiCatalog.FindByName(page.ComponentName);
            if (comp == null) continue;
            var missing = comp.RenderFragments
                .Where(f => !page.Content.Contains(f.Name, StringComparison.Ordinal))
                .Select(f => f.Name).ToList();
            if (missing.Any())
                failures.Add($"{page.FileName}: {string.Join(", ", missing)}");
        }
        Assert.True(failures.Count == 0, $"RenderFragments nao documentados:\n{string.Join("\n", failures)}");
    }
}
