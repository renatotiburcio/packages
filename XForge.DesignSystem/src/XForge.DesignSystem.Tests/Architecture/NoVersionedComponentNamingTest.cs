using System.Text.RegularExpressions;
using Xunit;

namespace XForge.DesignSystem.Tests.Architecture;

public sealed class NoVersionedComponentNamingTest
{
    private static readonly Regex VersionedProductNamePattern = new(
        @"(?i)(XFAi[A-Za-z0-9_]*V[2-9]\b|XF[A-Za-z0-9_]*V[2-9]\b|Ai[A-Za-z0-9_]*V[2-9]Docs\b|@page\s+""[^""]*-v[2-9]\b)",
        RegexOptions.Compiled);

    [Fact]
    public void Product_Source_Should_Not_Expose_Versioned_Component_Names()
    {
        var root = FindWorkspaceRoot();
        var productRoots = new[]
        {
            Path.Combine(root, "XForge.DesignSystem"),
            Path.Combine(root, "XForge.DesignSystem.Tests")
        };

        var violations = productRoots
            .Where(Directory.Exists)
            .SelectMany(path => Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            .Where(IsScannedFile)
            .SelectMany(GetViolations)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Versioned component names are not allowed in product source. Use canonical names and evolve components in place:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    private static string FindWorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "XForge.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate workspace root containing XForge.slnx.");
    }

    private static bool IsScannedFile(string path)
    {
        if (path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
            path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var extension = Path.GetExtension(path);
        return extension is ".cs" or ".razor";
    }

    private static IEnumerable<string> GetViolations(string path)
    {
        var fileName = Path.GetFileName(path);
        if (VersionedProductNamePattern.IsMatch(fileName))
        {
            yield return path;
        }

        var lineNumber = 0;
        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;
            if (VersionedProductNamePattern.IsMatch(line))
            {
                yield return $"{path}:{lineNumber}";
            }
        }
    }
}
