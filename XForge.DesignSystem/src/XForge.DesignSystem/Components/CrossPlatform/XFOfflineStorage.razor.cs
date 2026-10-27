namespace XForge.DesignSystem.Components.CrossPlatform;

/// <summary>Context provided by XFOfflineStorage with storage state and operations.</summary>
public record XFOfflineStorageContext
{
    public int EntryCount { get; }
    public int PendingSyncCount { get; }
    public int ConflictCount { get; }
    public bool IsOnline { get; }
    public bool IsSyncing { get; }
    public XFStorageStats Stats { get; }

    public XFOfflineStorageContext(int entryCount, int pendingSyncCount, int conflictCount,
        bool isOnline, bool isSyncing, XFStorageStats stats)
    {
        EntryCount = entryCount;
        PendingSyncCount = pendingSyncCount;
        ConflictCount = conflictCount;
        IsOnline = isOnline;
        IsSyncing = isSyncing;
        Stats = stats;
    }
}
