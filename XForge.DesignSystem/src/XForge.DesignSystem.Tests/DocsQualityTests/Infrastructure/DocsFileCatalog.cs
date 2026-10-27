namespace XForge.DesignSystem.Docs.QualityTests.Infrastructure;

public static class DocsFileCatalog
{
    private static readonly Lazy<DocsCatalogResult> _catalog = new(Discover);
    public static DocsCatalogResult All => _catalog.Value;

    private static DocsCatalogResult Discover()
    {
        var docsRoot = PathResolver.DocsPagesRoot;
        var docsFiles = new List<DocsPageDescriptor>();
        var overviewFiles = new List<DocsPageDescriptor>();

        if (!Directory.Exists(docsRoot))
            return new DocsCatalogResult { DocsPages = docsFiles, OverviewPages = overviewFiles };

        foreach (var file in Directory.GetFiles(docsRoot, "*Docs.razor", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(docsRoot, file);
            var content = File.ReadAllText(file);
            docsFiles.Add(new DocsPageDescriptor
            {
                FileName = Path.GetFileName(file),
                FullPath = file,
                RelativePath = relativePath,
                Route = ExtractRoute(content),
                ComponentName = ExtractComponentName(Path.GetFileNameWithoutExtension(file)),
                Content = content,
                LineCount = content.Split('\n').Length,
            });
        }

        foreach (var file in Directory.GetFiles(docsRoot, "*Overview.razor", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(docsRoot, file);
            var content = File.ReadAllText(file);
            overviewFiles.Add(new DocsPageDescriptor
            {
                FileName = Path.GetFileName(file),
                FullPath = file,
                RelativePath = relativePath,
                Route = ExtractRoute(content),
                ComponentName = Path.GetFileNameWithoutExtension(file).Replace("Overview", ""),
                Content = content,
                LineCount = content.Split('\n').Length,
            });
        }

        return new DocsCatalogResult
        {
            DocsPages = docsFiles.OrderBy(d => d.ComponentName).ToList(),
            OverviewPages = overviewFiles.OrderBy(d => d.ComponentName).ToList(),
        };
    }

    private static string? ExtractRoute(string content)
    {
        var match = System.Text.RegularExpressions.Regex.Match(content, @"@page\s+""([^""]+)""");
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string ExtractComponentName(string fileName)
    {
        var baseName = fileName.Replace("Docs", "");
        return $"XF{baseName}";
    }
}

public sealed record DocsCatalogResult
{
    public required IReadOnlyList<DocsPageDescriptor> DocsPages { get; init; }
    public required IReadOnlyList<DocsPageDescriptor> OverviewPages { get; init; }
}

public sealed record DocsPageDescriptor
{
    public required string FileName { get; init; }
    public required string FullPath { get; init; }
    public required string RelativePath { get; init; }
    public string? Route { get; init; }
    public required string ComponentName { get; init; }
    public required string Content { get; init; }
    public required int LineCount { get; init; }
}
