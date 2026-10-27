using XForge.DesignSystem.Components.CrossPlatform;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFOfflineStorageComponentTest
{
    [Fact]
    public void XFOfflineStorageContext_CreatesWithCorrectProperties()
    {
        var stats = new XFStorageStats
        {
            TotalEntries = 5,
            TotalSizeBytes = 1024,
            PendingSyncItems = 2,
            ConflictItems = 1,
            StorageQuotaBytes = 5242880
        };
        var context = new XFOfflineStorageContext(5, 2, 1, true, false, stats);

        Assert.Equal(5, context.EntryCount);
        Assert.Equal(2, context.PendingSyncCount);
        Assert.Equal(1, context.ConflictCount);
        Assert.True(context.IsOnline);
        Assert.False(context.IsSyncing);
        Assert.Equal(1024, context.Stats.TotalSizeBytes);
    }

    [Fact]
    public void XFOfflineStorageContext_ImplementsEquality()
    {
        var stats = new XFStorageStats();
        var context1 = new XFOfflineStorageContext(0, 0, 0, true, false, stats);
        var context2 = new XFOfflineStorageContext(0, 0, 0, true, false, stats);

        Assert.Equal(context1, context2);
    }

    [Fact]
    public void XFStorageStats_UsagePercentage_CalculatesCorrectly()
    {
        var stats = new XFStorageStats
        {
            TotalSizeBytes = 2621440,
            StorageQuotaBytes = 5242880
        };

        Assert.Equal(50.0, stats.UsagePercent);
    }
}