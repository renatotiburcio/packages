using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.CrossPlatform;

namespace XForge.DesignSystem.Tests.Components.CrossPlatform;

public class XFOfflineStorageExtendedTest : XForgeTestContext
{
    public XFOfflineStorageExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFOfflineStorage> RenderStorage(
        Action<ComponentParameterCollectionBuilder<XFOfflineStorage>>? configure = null)
    {
        return Render<XFOfflineStorage>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment<XFOfflineStorageContext>)(ctx =>
                (RenderFragment)(b => b.AddContent(0, $"Entries: {ctx.EntryCount}"))));
            configure?.Invoke(parameters);
        });
    }

    [Fact]
    public void DefaultRender_RendersContainer()
    {
        var cut = RenderStorage();
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void DefaultRender_ShowsOnlineClass()
    {
        var cut = RenderStorage();
        Assert.Contains("xf-storage-online", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersChildContentWithContext()
    {
        var cut = RenderStorage();
        Assert.Contains("Entries: 0", cut.Markup);
    }

    [Fact]
    public void SetItem_AddsToContext()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        cut.Render();
        Assert.Contains("Entries: 1", cut.Markup);
    }

    [Fact]
    public void SetItem_MultipleItems_UpdatesCount()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        cut.Instance.SetItem("key2", "value2");
        cut.Instance.SetItem("key3", "value3");
        cut.Render();
        Assert.Contains("Entries: 3", cut.Markup);
    }

    [Fact]
    public void GetItem_ReturnsValue()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "hello");
        Assert.Equal("hello", cut.Instance.GetItem("key1"));
    }

    [Fact]
    public void GetItem_NonExistentKey_ReturnsNull()
    {
        var cut = RenderStorage();
        Assert.Null(cut.Instance.GetItem("nonexistent"));
    }

    [Fact]
    public void HasItem_ExistingKey_ReturnsTrue()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        Assert.True(cut.Instance.HasItem("key1"));
    }

    [Fact]
    public void HasItem_NonExistentKey_ReturnsFalse()
    {
        var cut = RenderStorage();
        Assert.False(cut.Instance.HasItem("nonexistent"));
    }

    [Fact]
    public void RemoveItem_ExistingKey_ReturnsTrue()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        var result = cut.Instance.RemoveItem("key1");
        Assert.True(result);
        cut.Render();
        Assert.Contains("Entries: 0", cut.Markup);
    }

    [Fact]
    public void RemoveItem_NonExistentKey_ReturnsFalse()
    {
        var cut = RenderStorage();
        var result = cut.Instance.RemoveItem("nonexistent");
        Assert.False(result);
    }

    [Fact]
    public void GetKeys_ReturnsAllKeys()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("a", "1");
        cut.Instance.SetItem("b", "2");
        var keys = cut.Instance.GetKeys();
        Assert.Equal(2, keys.Count);
        Assert.Contains("a", keys);
        Assert.Contains("b", keys);
    }

    [Fact]
    public void GetAllEntries_ReturnsAllEntries()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("x", "10");
        cut.Instance.SetItem("y", "20");
        var entries = cut.Instance.GetAllEntries();
        Assert.Equal(2, entries.Count);
        Assert.Equal("10", entries["x"].Value);
        Assert.Equal("20", entries["y"].Value);
    }

    [Fact]
    public void Clear_RemovesAllEntries()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("a", "1");
        cut.Instance.SetItem("b", "2");
        cut.Instance.Clear();
        cut.Render();
        Assert.Contains("Entries: 0", cut.Markup);
    }

    [Fact]
    public void SetItem_UpdatesExistingKey()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "original");
        cut.Instance.SetItem("key1", "updated");
        Assert.Equal("updated", cut.Instance.GetItem("key1"));
        cut.Render();
        Assert.Contains("Entries: 1", cut.Markup);
    }

    [Fact]
    public void SetOnlineStatus_Offline_RendersOfflineClass()
    {
        var cut = RenderStorage();
        cut.Instance.SetOnlineStatus(false);
        cut.Render();
        Assert.Contains("xf-storage-offline", cut.Markup);
    }

    [Fact]
    public void SetOnlineStatus_Online_RendersOnlineClass()
    {
        var cut = RenderStorage();
        cut.Instance.SetOnlineStatus(false);
        cut.Instance.SetOnlineStatus(true);
        cut.Render();
        Assert.Contains("xf-storage-online", cut.Markup);
    }

    [Fact]
    public void GetPendingSyncItems_ReturnsPendingItems()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        var pending = cut.Instance.GetPendingSyncItems();
        Assert.True(pending.Count >= 1);
    }

    [Fact]
    public void GetConflicts_ReturnsEmptyInitially()
    {
        var cut = RenderStorage();
        var conflicts = cut.Instance.GetConflicts();
        Assert.Empty(conflicts);
    }

    [Fact]
    public void ResolveConflict_WithNoConflict_DoesNothing()
    {
        var cut = RenderStorage();
        cut.Instance.ResolveConflict("nonexistent", true);
        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public void CustomClass_AppendedToContainer()
    {
        var cut = RenderStorage(p => p.Add(x => x.Class, "my-storage"));
        Assert.Contains("my-storage", cut.Markup);
    }

    [Fact]
    public void Style_RendersOnContainer()
    {
        var cut = RenderStorage(p => p.Add(x => x.Style, "padding: 10px"));
        Assert.Contains("padding: 10px", cut.Markup);
    }

    [Fact]
    public void AdditionalAttributes_Render()
    {
        var cut = RenderStorage(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "value" } }));
        Assert.Contains("data-test", cut.Markup);
    }

    [Fact]
    public void StorageQuotaBytes_ParameterAccepted()
    {
        var cut = RenderStorage(p => p.Add(x => x.StorageQuotaBytes, 10 * 1024 * 1024));
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void ConflictStrategy_ParameterAccepted()
    {
        var cut = RenderStorage(p => p.Add(x => x.ConflictStrategy, XFSyncConflictStrategy.Manual));
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void OnSyncItemProcessed_CallbackAccepted()
    {
        var cut = RenderStorage(p => p
            .Add(p => p.OnSyncItemProcessed, EventCallback.Factory.Create<XFSyncQueueItem>(this, _ => { })));
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void OnConflictDetected_CallbackAccepted()
    {
        var cut = RenderStorage(p => p
            .Add(p => p.OnConflictDetected, EventCallback.Factory.Create<XFSyncConflict>(this, _ => { })));
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void OnStatsChanged_CallbackAccepted()
    {
        var cut = RenderStorage(p => p
            .Add(p => p.OnStatsChanged, EventCallback.Factory.Create<XFStorageStats>(this, _ => { })));
        Assert.Contains("xf-offline-storage", cut.Markup);
    }

    [Fact]
    public void XFOfflineStorageContext_CanCreate()
    {
        var stats = new XFStorageStats();
        var ctx = new XFOfflineStorageContext(5, 2, 1, true, false, stats);
        Assert.Equal(5, ctx.EntryCount);
        Assert.Equal(2, ctx.PendingSyncCount);
        Assert.Equal(1, ctx.ConflictCount);
        Assert.True(ctx.IsOnline);
        Assert.False(ctx.IsSyncing);
    }

    [Fact]
    public void XFStorageStats_CanCreate()
    {
        var stats = new XFStorageStats
        {
            TotalEntries = 10,
            TotalSizeBytes = 1024,
            PendingSyncItems = 3,
            ConflictItems = 1,
            StorageQuotaBytes = 5 * 1024 * 1024
        };
        Assert.Equal(10, stats.TotalEntries);
        Assert.Equal(1024, stats.TotalSizeBytes);
    }

    [Fact]
    public void XFStorageEntry_CanCreate()
    {
        var entry = new XFStorageEntry
        {
            Key = "test",
            Value = "data",
            SizeBytes = 4
        };
        Assert.Equal("test", entry.Key);
        Assert.Equal("data", entry.Value);
        Assert.Equal(4, entry.SizeBytes);
    }

    [Fact]
    public void XFSyncQueueItem_CanCreate()
    {
        var item = new XFSyncQueueItem
        {
            Key = "test",
            ChangeType = XFStorageChangeType.Created,
            Status = XFSyncItemStatus.Pending
        };
        Assert.Equal("test", item.Key);
        Assert.Equal(XFStorageChangeType.Created, item.ChangeType);
    }

    [Fact]
    public void XFSyncConflict_CanCreate()
    {
        var conflict = new XFSyncConflict
        {
            Key = "test",
            LocalValue = new XFStorageEntry { Key = "test", Value = "local" },
            RemoteValue = new XFStorageEntry { Key = "test", Value = "remote" }
        };
        Assert.Equal("test", conflict.Key);
    }

    [Fact]
    public void Enum_XFSyncConflictStrategy_HasValues()
    {
        Assert.Contains(XFSyncConflictStrategy.LastWriteWins, Enum.GetValues<XFSyncConflictStrategy>());
        Assert.Contains(XFSyncConflictStrategy.Manual, Enum.GetValues<XFSyncConflictStrategy>());
    }

    [Fact]
    public void Enum_XFStorageChangeType_HasValues()
    {
        Assert.Contains(XFStorageChangeType.Created, Enum.GetValues<XFStorageChangeType>());
        Assert.Contains(XFStorageChangeType.Updated, Enum.GetValues<XFStorageChangeType>());
        Assert.Contains(XFStorageChangeType.Deleted, Enum.GetValues<XFStorageChangeType>());
    }

    [Fact]
    public void Enum_XFSyncItemStatus_HasValues()
    {
        Assert.Contains(XFSyncItemStatus.Pending, Enum.GetValues<XFSyncItemStatus>());
        Assert.Contains(XFSyncItemStatus.Synced, Enum.GetValues<XFSyncItemStatus>());
    }

    [Fact]
    public void ProcessSyncQueue_WhenOffline_ReturnsZero()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        cut.Instance.SetOnlineStatus(false);
        var result = cut.Instance.ProcessSyncQueueAsync().Result;
        Assert.Equal(0, result);
    }

    [Fact]
    public void Dispose_ClearsAll()
    {
        var cut = RenderStorage();
        cut.Instance.SetItem("key1", "value1");
        cut.Instance.SetItem("key2", "value2");
        cut.Instance.Dispose();
        cut.Render();
        Assert.Contains("Entries: 0", cut.Markup);
    }
}
