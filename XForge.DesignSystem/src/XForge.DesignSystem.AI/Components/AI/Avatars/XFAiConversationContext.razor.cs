using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Conversation context manager for AI agents. Tracks session memory,
/// user preferences, context entries, and provides filtering/pinning.
/// Integrates with IAiMemoryService for persistent storage.
/// </summary>
public partial class XFAiConversationContext : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string Title { get; set; } = "Contexto da Conversa";

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show type filter chips.</summary>
    [Parameter] public bool ShowFilters { get; set; } = true;

    /// <summary>Whether to show relevance bars.</summary>
    [Parameter] public bool ShowRelevance { get; set; } = true;

    /// <summary>Whether to show summary footer.</summary>
    [Parameter] public bool ShowSummary { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter] public string EmptyText { get; set; } = "Nenhum contexto capturado ainda.";

    /// <summary>Clear button label.</summary>
    [Parameter] public string ClearLabel { get; set; } = "Limpar";

    /// <summary>ARIA label for the component.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Contexto da conversa";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom content rendered below the context list.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>External context entries (two-way binding).</summary>
    [Parameter] public IReadOnlyList<XFAiContextEntry> Entries { get; set; } = Array.Empty<XFAiContextEntry>();

    /// <summary>Fired when entries change.</summary>
    [Parameter] public EventCallback<IReadOnlyList<XFAiContextEntry>> EntriesChanged { get; set; }

    /// <summary>Fired when an entry is removed.</summary>
    [Parameter] public EventCallback<XFAiContextEntry> OnEntryRemoved { get; set; }

    /// <summary>Fired when all entries are cleared.</summary>
    [Parameter] public EventCallback OnClear { get; set; }

    // -- Internal State --------------------------------------------------

    private readonly List<XFAiContextEntry> _entries = new();
    private XFAiContextEntryType? _filterType;

    private IReadOnlyList<XFAiContextEntry> FilteredEntries => _filterType is null
        ? _entries.AsReadOnly()
        : _entries.Where(e => e.Type == _filterType.Value).ToList().AsReadOnly();

    private string AriaBusyValue => "false";

    // -- Lifecycle -------------------------------------------------------

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_entries, Entries) && !_entries.SequenceEqual(Entries))
        {
            _entries.Clear();
            _entries.AddRange(Entries);
        }
    }

    // -- Public Methods --------------------------------------------------

    /// <summary>
    /// Adds a context entry to the conversation memory.
    /// </summary>
    public void AddEntry(XFAiContextEntry entry)
    {
        _entries.Add(entry);
        EntriesChanged.InvokeAsync(_entries.ToList());
        StateHasChanged();
    }

    /// <summary>
    /// Gets a summary of the current conversation context state.
    /// </summary>
    public XFAiConversationSummary GetSummary()
    {
        return new XFAiConversationSummary
        {
            ContextEntryCount = _entries.Count,
            PinnedCount = _entries.Count(e => e.IsPinned),
            SessionStarted = _entries.Count > 0 ? _entries.Min(e => e.Timestamp) : DateTimeOffset.UtcNow,
            LastActivity = _entries.Count > 0 ? _entries.Max(e => e.Timestamp) : DateTimeOffset.UtcNow
        };
    }

    // -- Internal --------------------------------------------------------

    private async Task RemoveEntryAsync(string entryId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId);
        if (entry is not null)
        {
            _entries.Remove(entry);
            await OnEntryRemoved.InvokeAsync(entry);
            await EntriesChanged.InvokeAsync(_entries.ToList());
            StateHasChanged();
        }
    }

    private async Task ClearAllAsync()
    {
        _entries.Clear();
        _filterType = null;
        await OnClear.InvokeAsync();
        await EntriesChanged.InvokeAsync(_entries.ToList());
        StateHasChanged();
    }

    // -- CSS Helpers -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-conversation-context overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private string GetFilterChipClass(XFAiContextEntryType? type) => type == _filterType
        ? "rounded-full bg-primary px-2.5 py-0.5 text-xs font-medium text-primary-contrast"
        : "rounded-full bg-surface-100 px-2.5 py-0.5 text-xs font-medium text-text-secondary hover:bg-surface-200";

    private static string GetTypeColorClass(XFAiContextEntryType type) => type switch
    {
        XFAiContextEntryType.Preference => "bg-violet-100 text-violet-700 dark:bg-violet-900/30 dark:text-violet-300",
        XFAiContextEntryType.Fact => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiContextEntryType.Action => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-300",
        XFAiContextEntryType.Decision => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        XFAiContextEntryType.Feedback => "bg-pink-100 text-pink-700 dark:bg-pink-900/30 dark:text-pink-300",
        XFAiContextEntryType.System => "bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300",
        _ => "bg-gray-100 text-gray-700"
    };

    private static string GetTypeIcon(XFAiContextEntryType type) => type switch
    {
        XFAiContextEntryType.Preference => "⚙",
        XFAiContextEntryType.Fact => "📌",
        XFAiContextEntryType.Action => "▶",
        XFAiContextEntryType.Decision => "✓",
        XFAiContextEntryType.Feedback => "💬",
        XFAiContextEntryType.System => "●",
        _ => "●"
    };

    private static string GetTypeLabel(XFAiContextEntryType type) => type switch
    {
        XFAiContextEntryType.Preference => "Preferência",
        XFAiContextEntryType.Fact => "Fato",
        XFAiContextEntryType.Action => "Ação",
        XFAiContextEntryType.Decision => "Decisão",
        XFAiContextEntryType.Feedback => "Feedback",
        XFAiContextEntryType.System => "Sistema",
        _ => type.ToString()
    };

    private static string GetRelevanceColor(double relevance) => relevance switch
    {
        >= 0.8 => "bg-green-500",
        >= 0.6 => "bg-yellow-500",
        >= 0.4 => "bg-orange-500",
        _ => "bg-red-500"
    };

    private static string FormatTimestamp(DateTimeOffset timestamp)
    {
        var diff = DateTimeOffset.UtcNow - timestamp;
        if (diff.TotalMinutes < 1) return "agora";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h";
        return timestamp.ToString("dd/MM HH:mm");
    }

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
