using System;
using System.Collections.Generic;
using System.Linq;
using XForge.DesignSystem.Components;
using XForge.DesignSystem.Components.Utilities;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Generators.Snippets;
using Xunit;
using XForge.DesignSystem.Utilities;
using XForge.DesignSystem.Components.CrossPlatform;
using XForge.DesignSystem.Components.Forms.DatePicker;

namespace XForge.DesignSystem.Tests.Domain;

public sealed class XFSearchRankerTests
{
    [Theory]
    [InlineData("hello", "hello", "hello", 100)]
    [InlineData("hello-world", "hello", "hello", 95)]
    [InlineData("hello world", "greeting", "hello", 80)]
    [InlineData("hi-world", "greeting", "hello", 0)]
    [InlineData("", "greeting", "hello", 0)]
    public void Score_ReturnsCorrectRank(string label, string slug, string query, int expected)
    {
        Assert.Equal(expected, XFSearchRanker.Score(label, slug, query));
    }

    [Fact]
    public void Score_EmptyQuery_ReturnsZero()
    {
        Assert.Equal(0, XFSearchRanker.Score("anything", "anything", ""));
    }

    [Fact]
    public void Score_NullQuery_ReturnsZero()
    {
        Assert.Equal(0, XFSearchRanker.Score("anything", "anything", null!));
    }

    [Theory]
    [InlineData("say hello", "greeting", 60)]
    [InlineData("say hello world", "greeting", 60)]
    [InlineData("helloworld", "greeting", 80)]
    [InlineData("greeting", "helloworld", 75)]
    public void Score_Matches_ReturnsCorrectTier(string label, string slug, int expected)
    {
        Assert.Equal(expected, XFSearchRanker.Score(label, slug, "hello"));
    }

    [Fact]
    public void IsFuzzyMatch_AllCharsInOrder_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsFuzzyMatch("h.e-l.l.o", "hello"));
    }

    [Fact]
    public void IsFuzzyMatch_SkipsChars_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsFuzzyMatch("hxeclxlo", "hello"));
    }

    [Fact]
    public void IsFuzzyMatch_MissingChar_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsFuzzyMatch("hllo", "hello"));
    }

    [Fact]
    public void IsFuzzyMatch_EmptyQuery_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsFuzzyMatch("anything", ""));
    }

    [Fact]
    public void IsWordPrefixMatch_MultiWord_MatchesAll()
    {
        Assert.True(XFSearchRanker.IsWordPrefixMatch("hello world", "hel wo"));
    }

    [Fact]
    public void IsWordPrefixMatch_SingleWord_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsWordPrefixMatch("hello world", "hel"));
    }

    [Fact]
    public void IsWordPrefixMatch_PartialMismatch_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsWordPrefixMatch("hello world", "hel xyz"));
    }

    [Fact]
    public void Rank_FiltersNonMatches()
    {
        var items = new[] { "hello", "world", "help" };
        var results = XFSearchRanker.Rank(items, "hel", s => s, s => s);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Rank_EmptyQuery_ReturnsEmpty()
    {
        var items = new[] { "hello" };
        Assert.Empty(XFSearchRanker.Rank(items, "", s => s, s => s));
    }

    [Fact]
    public void Rank_NullQuery_ReturnsEmpty()
    {
        var items = new[] { "hello" };
        Assert.Empty(XFSearchRanker.Rank(items, null, s => s, s => s));
    }

    [Fact]
    public void Rank_RespectsMaxResults()
    {
        var items = Enumerable.Range(0, 100).Select(i => "item" + i).ToArray();
        var results = XFSearchRanker.Rank(items, "item", s => s, s => s, maxResults: 5);
        Assert.Equal(5, results.Count);
    }

    [Fact]
    public void Rank_OrdersByScoreDescending()
    {
        var items = new[] { "help me", "hello" };
        var results = XFSearchRanker.Rank(items, "hel", s => s, s => s);
        Assert.Equal("hello", results[0].Entry);
        Assert.Equal("help me", results[1].Entry);
    }
}

public sealed class XFComponentDiscoveryTests
{
    private static readonly XFComponentInfo[] TestComponents = new[]
    {
        new XFComponentInfo("XButton", "Primary action button", "Buttons", new[] { "click", "action" }, new[] { "check" }),
        new XFComponentInfo("XCard", "Card container", "Layout", new[] { "container" }, Array.Empty<string>()),
        new XFComponentInfo("XInput", "Text input", "Forms", new[] { "text", "input" }, new[] { "pencil" }),
        new XFComponentInfo("XDataGrid", "Data grid", "Data", new[] { "table", "grid" }, new[] { "table" }),
    };

    private readonly XFComponentDiscovery _sut = new(TestComponents);

    [Fact]
    public void GetAll_ReturnsAllComponents()
    {
        Assert.Equal(4, _sut.GetAll().Count());
    }

    [Fact]
    public void GetByName_ExactMatch_ReturnsComponent()
    {
        var result = _sut.GetByName("XButton");
        Assert.NotNull(result);
        Assert.Equal("XButton", result!.Name);
    }

    [Fact]
    public void GetByName_CaseInsensitive_ReturnsComponent()
    {
        var result = _sut.GetByName("xbutton");
        Assert.NotNull(result);
        Assert.Equal("XButton", result!.Name);
    }

    [Fact]
    public void GetByName_NullInput_ReturnsNull()
    {
        Assert.Null(_sut.GetByName(null!));
    }

    [Fact]
    public void GetByName_NonExistent_ReturnsNull()
    {
        Assert.Null(_sut.GetByName("NonExistent"));
    }

    [Fact]
    public void GetByCategory_ExactMatch_ReturnsFiltered()
    {
        var results = _sut.GetByCategory("Forms").ToList();
        Assert.Single(results);
        Assert.Equal("XInput", results[0].Name);
    }

    [Fact]
    public void GetByCategory_CaseInsensitive_ReturnsFiltered()
    {
        var results = _sut.GetByCategory("forms").ToList();
        Assert.Single(results);
    }

    [Fact]
    public void GetByCategory_NoMatch_ReturnsEmpty()
    {
        Assert.Empty(_sut.GetByCategory("NonExistent"));
    }
}

public sealed class XFSnippetTests
{
    [Fact]
    public void ToMarkdown_ReturnsFormattedBlock()
    {
        var snippet = new XFSnippet("XButton", "Basic", "<XButton>Click</XButton>", "A button.", "razor", new[] { "btn" });
        var md = snippet.ToMarkdown();
        Assert.Contains("### Basic", md);
        Assert.Contains("```razor", md);
        Assert.Contains("<XButton>Click</XButton>", md);
    }

    [Fact]
    public void ToMarkdown_NullDescription_StillWorks()
    {
        var snippet = new XFSnippet("XButton", "Basic", "<XButton/>");
        var md = snippet.ToMarkdown();
        Assert.Contains("### Basic", md);
    }

    [Fact]
    public void Catalog_ContainsExpectedEntries()
    {
        Assert.True(XFSnippetCatalog.All.Count > 0);
        Assert.Contains(XFSnippetCatalog.All, s => s.Component == "XButton");
    }

    [Fact]
    public void Catalog_ForComponent_ReturnsFiltered()
    {
        var snippets = XFSnippetCatalog.ForComponent("XButton").ToList();
        Assert.True(snippets.All(s => s.Component == "XButton"));
    }

    [Fact]
    public void DefaultGenerator_Count_MatchesCatalog()
    {
        var gen = new XFDefaultSnippetsGenerator();
        Assert.Equal(XFSnippetCatalog.All.Count, gen.Count);
    }

    [Fact]
    public void DefaultGenerator_Generate_ContainsNamespace()
    {
        var gen = new XFDefaultSnippetsGenerator();
        var code = gen.Generate();
        Assert.Contains("namespace XForge.DesignSystem.Generated", code);
        Assert.Contains("// <auto-generated/>", code);
    }

    [Fact]
    public void DefaultGenerator_Generate_ContainsSnippetConstants()
    {
        var gen = new XFDefaultSnippetsGenerator();
        var code = gen.Generate();
        Assert.Contains("XButton_Basic_Button", code);
    }
}

public sealed class XFDatePickerDateRenderArgsTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var args = new XFDatePickerDateRenderArgs
        {
            Date = new DateOnly(2026, 6, 27),
            IsCurrentMonth = true,
            IsSelected = false,
            IsToday = true,
            IsDisabled = false,
            CssClass = "custom"
        };
        Assert.Equal(new DateOnly(2026, 6, 27), args.Date);
        Assert.True(args.IsCurrentMonth);
        Assert.False(args.IsSelected);
        Assert.True(args.IsToday);
        Assert.False(args.IsDisabled);
        Assert.Equal("custom", args.CssClass);
    }

    [Fact]
    public void CssClass_CanBeSet()
    {
        var args = new XFDatePickerDateRenderArgs();
        Assert.Null(args.CssClass);
        args.CssClass = "highlight";
        Assert.Equal("highlight", args.CssClass);
    }
}

public sealed class XFCrossPlatformModelsTests
{
    [Fact]
    public void XFGestureEvent_Properties()
    {
        var evt = new XFGestureEvent
        {
            Type = XFGestureType.SwipeLeft,
            State = XFGestureState.Recognized,
            StartX = 100,
            StartY = 200,
            CurrentX = 50,
            CurrentY = 200,
            DeltaX = -50,
            DeltaY = 0,
            Distance = 50,
            VelocityX = -0.5,
            VelocityY = 0,
            Scale = 1.0,
            Rotation = 0,
            DurationMs = 300,
            PointerCount = 1
        };
        Assert.Equal(XFGestureType.SwipeLeft, evt.Type);
        Assert.Equal(XFGestureState.Recognized, evt.State);
        Assert.Equal(-50, evt.DeltaX);
        Assert.Equal(50, evt.Distance);
    }

    [Fact]
    public void XFGeoLocation_Properties()
    {
        var loc = new XFGeoLocation
        {
            Latitude = -23.5505,
            Longitude = -46.6333,
            Altitude = 760,
            Accuracy = 5.0,
            Speed = 1.5,
            Heading = 90
        };
        Assert.Equal(-23.5505, loc.Latitude);
        Assert.Equal(-46.6333, loc.Longitude);
        Assert.Equal(760, loc.Altitude);
    }

    [Fact]
    public void XFStorageStats_UsagePercent_CalculatesCorrectly()
    {
        var stats = new XFStorageStats { TotalSizeBytes = 500, StorageQuotaBytes = 1000 };
        Assert.Equal(50.0, stats.UsagePercent);
    }

    [Fact]
    public void XFStorageStats_UsagePercent_ZeroQuota_ReturnsZero()
    {
        var stats = new XFStorageStats { TotalSizeBytes = 500, StorageQuotaBytes = 0 };
        Assert.Equal(0, stats.UsagePercent);
    }

    [Fact]
    public void XFSyncQueueItem_Properties()
    {
        var item = new XFSyncQueueItem
        {
            Key = "user/1",
            ChangeType = XFStorageChangeType.Updated,
            Status = XFSyncItemStatus.Pending,
            Payload = "data",
            RetryCount = 2
        };
        Assert.Equal("user/1", item.Key);
        Assert.Equal(XFStorageChangeType.Updated, item.ChangeType);
        Assert.Equal(XFSyncItemStatus.Pending, item.Status);
        Assert.Equal(2, item.RetryCount);
    }

    [Fact]
    public void XFSyncQueueItem_Id_IsUnique()
    {
        var a = new XFSyncQueueItem();
        var b = new XFSyncQueueItem();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void XFStorageEntry_Properties()
    {
        var entry = new XFStorageEntry
        {
            Key = "settings",
            Value = "{}",
            ETag = "abc123",
            SizeBytes = 2
        };
        Assert.Equal("settings", entry.Key);
        Assert.Equal("{}", entry.Value);
        Assert.Equal("abc123", entry.ETag);
    }

    [Fact]
    public void XFSyncConflict_Properties()
    {
        var conflict = new XFSyncConflict
        {
            Key = "item/1",
            LocalValue = new XFStorageEntry { Key = "item/1", Value = "local" },
            RemoteValue = new XFStorageEntry { Key = "item/1", Value = "remote" }
        };
        Assert.Equal("item/1", conflict.Key);
        Assert.Equal("local", conflict.LocalValue.Value);
        Assert.Equal("remote", conflict.RemoteValue.Value);
    }

    [Fact]
    public void XFGestureOptions_Defaults()
    {
        var opts = new XFGestureOptions();
        Assert.Equal(50.0, opts.SwipeThreshold);
        Assert.Equal(0.3, opts.SwipeVelocityThreshold);
        Assert.Equal(500, opts.LongPressDurationMs);
        Assert.True(opts.EnableSwipe);
        Assert.False(opts.EnableRotate);
    }

    [Fact]
    public void XFBiometricResult_Properties()
    {
        var result = new XFBiometricResult
        {
            IsAuthenticated = true,
            UserId = "user1",
            Status = XFPermissionStatus.Granted
        };
        Assert.True(result.IsAuthenticated);
        Assert.Equal("user1", result.UserId);
        Assert.Equal(XFPermissionStatus.Granted, result.Status);
    }

    [Fact]
    public void XFDeviceFeatureInfo_Properties()
    {
        var info = new XFDeviceFeatureInfo
        {
            Feature = XFDeviceFeature.Camera,
            IsAvailable = true,
            Permission = XFPermissionStatus.Granted,
            Description = "Rear camera"
        };
        Assert.Equal(XFDeviceFeature.Camera, info.Feature);
        Assert.True(info.IsAvailable);
    }

    [Fact]
    public void XFStorageStats_AllProperties()
    {
        var stats = new XFStorageStats
        {
            TotalEntries = 42,
            TotalSizeBytes = 1024,
            PendingSyncItems = 3,
            ConflictItems = 1,
            StorageQuotaBytes = 2048
        };
        Assert.Equal(42, stats.TotalEntries);
        Assert.Equal(1024, stats.TotalSizeBytes);
        Assert.Equal(50.0, stats.UsagePercent);
    }
}
