namespace XForge.DesignSystem.Docs.QualityTests.Infrastructure;

public static class AllowedDocsExceptions
{
    public static IReadOnlySet<string> ExemptComponents { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "XFKeyCode",
        "XFFocusTrap",
        "XFBreakpointObserver",
        "XFNavigationGuard",
        "XFSortableList",
        "XFDragDrop",
    };

    public static IReadOnlyDictionary<string, string> DocsFileNameOverrides { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "XFRadio", "RadioDocs.razor" },
    };
}
