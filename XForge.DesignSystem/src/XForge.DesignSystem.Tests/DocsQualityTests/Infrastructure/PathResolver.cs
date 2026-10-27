namespace XForge.DesignSystem.Docs.QualityTests.Infrastructure;

/// <summary>
/// Resolves project paths for test infrastructure.
/// </summary>
public static class PathResolver
{
    /// <summary>
    /// Gets the Docs/Pages root directory by walking up from the test assembly location.
    /// </summary>
    public static string DocsPagesRoot
    {
        get
        {
            // Start from the test assembly directory and walk up to find the Docs project
            var assemblyDir = Path.GetDirectoryName(typeof(PathResolver).Assembly.Location)!;
            var currentDir = new DirectoryInfo(assemblyDir);

            // Search up the directory tree for the Docs project's Components/Pages folder
            while (currentDir != null)
            {
                // Check for XForge.DesignSystem.Docs/Components/Pages
                var docsPagesPath = Path.Combine(currentDir.FullName, "src", "XForge.DesignSystem.Docs", "Components", "Pages");
                if (Directory.Exists(docsPagesPath))
                    return docsPagesPath;

                // Also check for XForge.DesignSystem.Docs.AI/Components/Pages
                var docsAiPagesPath = Path.Combine(currentDir.FullName, "src", "XForge.DesignSystem.Docs.AI", "Components", "Pages");
                if (Directory.Exists(docsAiPagesPath))
                    return docsAiPagesPath;

                // Check if we're in the workspace root
                var workspacePagesPath = Path.Combine(currentDir.FullName, "XForge.DesignSystem.Docs", "Components", "Pages");
                if (Directory.Exists(workspacePagesPath))
                    return workspacePagesPath;

                currentDir = currentDir.Parent;
            }

            // Fallback: return a path relative to the solution directory
            return Path.Combine(Directory.GetCurrentDirectory(), "src", "XForge.DesignSystem.Docs", "Components", "Pages");
        }
    }

    /// <summary>
    /// Gets the snapshot directory for API contract snapshots.
    /// </summary>
    public static string SnapshotDir
    {
        get
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "src", "XForge.DesignSystem.Tests", "DocsQualityTests", "Snapshots");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
