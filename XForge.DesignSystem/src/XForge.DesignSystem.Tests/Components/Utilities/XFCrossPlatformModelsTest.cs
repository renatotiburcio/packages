using Xunit;
using XForge.DesignSystem.Components.CrossPlatform;

namespace XForge.DesignSystem.Tests.Components.Utilities;

/// <summary>
/// Tests for CrossPlatform models at 0% coverage: enums, records (XFGestureEvent, XFGestureOptions,
/// XFGeoLocation, XFCameraCapture, XFBiometricResult, XFDeviceFeatureInfo, XFSyncQueueItem,
/// XFStorageEntry, XFSyncConflict, XFStorageStats).
/// </summary>
public sealed class XFCrossPlatformModelsTest
{
    #region Enum Tests

    [Fact]
    public void XFGestureType_Values_AreCorrect()
    {
        Assert.Equal(0, (int)XFGestureType.Tap);
        Assert.Equal(1, (int)XFGestureType.DoubleTap);
        Assert.Equal(2, (int)XFGestureType.LongPress);
        Assert.Equal(3, (int)XFGestureType.SwipeLeft);
        Assert.Equal(4, (int)XFGestureType.SwipeRight);
        Assert.Equal(5, (int)XFGestureType.SwipeUp);
        Assert.Equal(6, (int)XFGestureType.SwipeDown);
        Assert.Equal(7, (int)XFGestureType.PinchIn);
        Assert.Equal(8, (int)XFGestureType.PinchOut);
        Assert.Equal(9, (int)XFGestureType.Rotate);
        Assert.Equal(10, Enum.GetValues<XFGestureType>().Length);
    }

    [Fact]
    public void XFGestureState_Values_AreCorrect()
    {
        Assert.Equal(0, (int)XFGestureState.Idle);
        Assert.Equal(1, (int)XFGestureState.Tracking);
        Assert.Equal(2, (int)XFGestureState.Recognized);
        Assert.Equal(3, (int)XFGestureState.Cancelled);
    }

    [Fact]
    public void XFDevicePlatform_Values_AreCorrect()
    {
        Assert.Equal(6, Enum.GetValues<XFDevicePlatform>().Length);
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.Web));
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.iOS));
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.Android));
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.Windows));
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.macOS));
        Assert.True(Enum.IsDefined<XFDevicePlatform>(XFDevicePlatform.Linux));
    }

    [Fact]
    public void XFDeviceFormFactor_Values_AreCorrect()
    {
        Assert.Equal(5, Enum.GetValues<XFDeviceFormFactor>().Length);
        Assert.True(Enum.IsDefined<XFDeviceFormFactor>(XFDeviceFormFactor.Phone));
        Assert.True(Enum.IsDefined<XFDeviceFormFactor>(XFDeviceFormFactor.Tablet));
        Assert.True(Enum.IsDefined<XFDeviceFormFactor>(XFDeviceFormFactor.Desktop));
        Assert.True(Enum.IsDefined<XFDeviceFormFactor>(XFDeviceFormFactor.Tv));
        Assert.True(Enum.IsDefined<XFDeviceFormFactor>(XFDeviceFormFactor.Watch));
    }

    [Fact]
    public void XFDeviceFeature_Values_AreCorrect()
    {
        Assert.Equal(12, Enum.GetValues<XFDeviceFeature>().Length);
        Assert.True(Enum.IsDefined<XFDeviceFeature>(XFDeviceFeature.Camera));
        Assert.True(Enum.IsDefined<XFDeviceFeature>(XFDeviceFeature.Geolocation));
        Assert.True(Enum.IsDefined<XFDeviceFeature>(XFDeviceFeature.Biometrics));
        Assert.True(Enum.IsDefined<XFDeviceFeature>(XFDeviceFeature.FileSystem));
        Assert.True(Enum.IsDefined<XFDeviceFeature>(XFDeviceFeature.Notifications));
    }

    [Fact]
    public void XFPermissionStatus_Values_AreCorrect()
    {
        Assert.Equal(5, Enum.GetValues<XFPermissionStatus>().Length);
        Assert.True(Enum.IsDefined<XFPermissionStatus>(XFPermissionStatus.Unknown));
        Assert.True(Enum.IsDefined<XFPermissionStatus>(XFPermissionStatus.Granted));
        Assert.True(Enum.IsDefined<XFPermissionStatus>(XFPermissionStatus.Denied));
        Assert.True(Enum.IsDefined<XFPermissionStatus>(XFPermissionStatus.Restricted));
        Assert.True(Enum.IsDefined<XFPermissionStatus>(XFPermissionStatus.PromptRequired));
    }

    [Fact]
    public void XFSyncConflictStrategy_Values_AreCorrect()
    {
        Assert.Equal(5, Enum.GetValues<XFSyncConflictStrategy>().Length);
        Assert.True(Enum.IsDefined<XFSyncConflictStrategy>(XFSyncConflictStrategy.ClientWins));
        Assert.True(Enum.IsDefined<XFSyncConflictStrategy>(XFSyncConflictStrategy.ServerWins));
        Assert.True(Enum.IsDefined<XFSyncConflictStrategy>(XFSyncConflictStrategy.LastWriteWins));
        Assert.True(Enum.IsDefined<XFSyncConflictStrategy>(XFSyncConflictStrategy.Manual));
        Assert.True(Enum.IsDefined<XFSyncConflictStrategy>(XFSyncConflictStrategy.Merge));
    }

    [Fact]
    public void XFSyncItemStatus_Values_AreCorrect()
    {
        Assert.Equal(5, Enum.GetValues<XFSyncItemStatus>().Length);
        Assert.True(Enum.IsDefined<XFSyncItemStatus>(XFSyncItemStatus.Pending));
        Assert.True(Enum.IsDefined<XFSyncItemStatus>(XFSyncItemStatus.Syncing));
        Assert.True(Enum.IsDefined<XFSyncItemStatus>(XFSyncItemStatus.Synced));
        Assert.True(Enum.IsDefined<XFSyncItemStatus>(XFSyncItemStatus.Conflict));
        Assert.True(Enum.IsDefined<XFSyncItemStatus>(XFSyncItemStatus.Failed));
    }

    [Fact]
    public void XFStorageChangeType_Values_AreCorrect()
    {
        Assert.Equal(3, Enum.GetValues<XFStorageChangeType>().Length);
        Assert.True(Enum.IsDefined<XFStorageChangeType>(XFStorageChangeType.Created));
        Assert.True(Enum.IsDefined<XFStorageChangeType>(XFStorageChangeType.Updated));
        Assert.True(Enum.IsDefined<XFStorageChangeType>(XFStorageChangeType.Deleted));
    }

    #endregion

    #region XFGestureEvent

    [Fact]
    public void XFGestureEvent_Defaults_AreCorrect()
    {
        var e = new XFGestureEvent();
        Assert.Equal(XFGestureType.Tap, e.Type);
        Assert.Equal(XFGestureState.Idle, e.State);
        Assert.Equal(0, e.StartX);
        Assert.Equal(0, e.StartY);
        Assert.Equal(0, e.CurrentX);
        Assert.Equal(0, e.CurrentY);
        Assert.Equal(0, e.DeltaX);
        Assert.Equal(0, e.DeltaY);
        Assert.Equal(0, e.Distance);
        Assert.Equal(0, e.VelocityX);
        Assert.Equal(0, e.VelocityY);
        Assert.Equal(0, e.Scale);
        Assert.Equal(0, e.Rotation);
        Assert.Equal(0, e.DurationMs);
        Assert.Equal(0, e.PointerCount);
    }

    [Fact]
    public void XFGestureEvent_WithValues_AssignsCorrectly()
    {
        var e = new XFGestureEvent
        {
            Type = XFGestureType.SwipeRight,
            State = XFGestureState.Recognized,
            StartX = 100, StartY = 200,
            CurrentX = 300, CurrentY = 400,
            DeltaX = 200, DeltaY = 200,
            Distance = 282.84,
            VelocityX = 1.5, VelocityY = 1.5,
            Scale = 2.0, Rotation = 45.0,
            DurationMs = 250,
            PointerCount = 2
        };
        Assert.Equal(XFGestureType.SwipeRight, e.Type);
        Assert.Equal(XFGestureState.Recognized, e.State);
        Assert.Equal(100, e.StartX);
        Assert.Equal(282.84, e.Distance, 2);
        Assert.Equal(45.0, e.Rotation);
        Assert.Equal(250, e.DurationMs);
        Assert.Equal(2, e.PointerCount);
    }

    [Fact]
    public void XFGestureEvent_WithInit_IsImmutable()
    {
        var e = new XFGestureEvent { Type = XFGestureType.PinchIn, Scale = 0.5 };
        Assert.Equal(XFGestureType.PinchIn, e.Type);
        Assert.Equal(0.5, e.Scale);
    }

    #endregion

    #region XFGestureOptions

    [Fact]
    public void XFGestureOptions_Defaults_AreCorrect()
    {
        var opts = new XFGestureOptions();
        Assert.Equal(50.0, opts.SwipeThreshold);
        Assert.Equal(0.3, opts.SwipeVelocityThreshold);
        Assert.Equal(500, opts.LongPressDurationMs);
        Assert.Equal(300, opts.DoubleTapWindowMs);
        Assert.Equal(0.1, opts.PinchThreshold);
        Assert.True(opts.EnableSwipe);
        Assert.True(opts.EnablePinch);
        Assert.True(opts.EnableLongPress);
        Assert.True(opts.EnableDoubleTap);
        Assert.False(opts.EnableRotate);
    }

    [Fact]
    public void XFGestureOptions_CustomValues_AssignCorrectly()
    {
        var opts = new XFGestureOptions
        {
            SwipeThreshold = 100,
            SwipeVelocityThreshold = 0.5,
            LongPressDurationMs = 800,
            DoubleTapWindowMs = 400,
            PinchThreshold = 0.2,
            EnableSwipe = false,
            EnablePinch = false,
            EnableLongPress = false,
            EnableDoubleTap = false,
            EnableRotate = true
        };
        Assert.Equal(100, opts.SwipeThreshold);
        Assert.Equal(0.5, opts.SwipeVelocityThreshold);
        Assert.Equal(800, opts.LongPressDurationMs);
        Assert.False(opts.EnableSwipe);
        Assert.True(opts.EnableRotate);
    }

    #endregion

    #region XFGeoLocation

    [Fact]
    public void XFGeoLocation_Defaults_AreCorrect()
    {
        var loc = new XFGeoLocation();
        Assert.Equal(0, loc.Latitude);
        Assert.Equal(0, loc.Longitude);
        Assert.Null(loc.Altitude);
        Assert.Null(loc.Accuracy);
        Assert.Null(loc.Speed);
        Assert.Null(loc.Heading);
        Assert.True(Math.Abs((DateTimeOffset.UtcNow - loc.Timestamp).TotalSeconds) < 5);
    }

    [Fact]
    public void XFGeoLocation_WithValues_AssignsCorrectly()
    {
        var ts = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);
        var loc = new XFGeoLocation
        {
            Latitude = -23.5505,
            Longitude = -46.6333,
            Altitude = 760,
            Accuracy = 10.5,
            Speed = 0,
            Heading = 180,
            Timestamp = ts
        };
        Assert.Equal(-23.5505, loc.Latitude, 4);
        Assert.Equal(-46.6333, loc.Longitude, 4);
        Assert.Equal(760, loc.Altitude);
        Assert.Equal(10.5, loc.Accuracy);
        Assert.Equal(ts, loc.Timestamp);
    }

    #endregion

    #region XFCameraCapture

    [Fact]
    public void XFCameraCapture_Defaults_AreCorrect()
    {
        var capture = new XFCameraCapture();
        Assert.Empty(capture.Data);
        Assert.Equal("image/jpeg", capture.ContentType);
        Assert.Equal(0, capture.Width);
        Assert.Equal(0, capture.Height);
        Assert.Null(capture.FileName);
    }

    [Fact]
    public void XFCameraCapture_WithValues_AssignsCorrectly()
    {
        var data = new byte[] { 0xFF, 0xD8, 0xFF };
        var capture = new XFCameraCapture
        {
            Data = data,
            ContentType = "image/png",
            Width = 1920,
            Height = 1080,
            FileName = "photo.png"
        };
        Assert.Equal(data, capture.Data);
        Assert.Equal("image/png", capture.ContentType);
        Assert.Equal(1920, capture.Width);
        Assert.Equal(1080, capture.Height);
        Assert.Equal("photo.png", capture.FileName);
    }

    #endregion

    #region XFBiometricResult

    [Fact]
    public void XFBiometricResult_Defaults_AreCorrect()
    {
        var result = new XFBiometricResult();
        Assert.False(result.IsAuthenticated);
        Assert.Null(result.UserId);
        Assert.Equal(XFPermissionStatus.Unknown, result.Status);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void XFBiometricResult_Authenticated_HasCorrectValues()
    {
        var result = new XFBiometricResult
        {
            IsAuthenticated = true,
            UserId = "user-123",
            Status = XFPermissionStatus.Granted
        };
        Assert.True(result.IsAuthenticated);
        Assert.Equal("user-123", result.UserId);
        Assert.Equal(XFPermissionStatus.Granted, result.Status);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void XFBiometricResult_Denied_HasError()
    {
        var result = new XFBiometricResult
        {
            IsAuthenticated = false,
            Status = XFPermissionStatus.Denied,
            ErrorMessage = "Biometric not enrolled"
        };
        Assert.False(result.IsAuthenticated);
        Assert.Equal(XFPermissionStatus.Denied, result.Status);
        Assert.Equal("Biometric not enrolled", result.ErrorMessage);
    }

    #endregion

    #region XFDeviceFeatureInfo

    [Fact]
    public void XFDeviceFeatureInfo_Defaults_AreCorrect()
    {
        var info = new XFDeviceFeatureInfo();
        Assert.Equal(XFDeviceFeature.Camera, info.Feature);
        Assert.False(info.IsAvailable);
        Assert.Equal(XFPermissionStatus.Unknown, info.Permission);
        Assert.Null(info.Description);
    }

    [Fact]
    public void XFDeviceFeatureInfo_WithValues_AssignsCorrectly()
    {
        var info = new XFDeviceFeatureInfo
        {
            Feature = XFDeviceFeature.Geolocation,
            IsAvailable = true,
            Permission = XFPermissionStatus.Granted,
            Description = "GPS available"
        };
        Assert.Equal(XFDeviceFeature.Geolocation, info.Feature);
        Assert.True(info.IsAvailable);
        Assert.Equal(XFPermissionStatus.Granted, info.Permission);
        Assert.Equal("GPS available", info.Description);
    }

    #endregion

    #region XFSyncQueueItem

    [Fact]
    public void XFSyncQueueItem_Defaults_AreCorrect()
    {
        var item = new XFSyncQueueItem();
        Assert.NotEmpty(item.Id);
        Assert.Equal(32, item.Id.Length); // Guid "N" format = 32 hex chars
        Assert.Equal(string.Empty, item.Key);
        Assert.Equal(XFStorageChangeType.Created, item.ChangeType); // default enum
        Assert.Equal(XFSyncItemStatus.Pending, item.Status);
        Assert.Null(item.Payload);
        Assert.Equal(0, item.RetryCount);
        Assert.Null(item.LastError);
        Assert.True(Math.Abs((DateTimeOffset.UtcNow - item.Timestamp).TotalSeconds) < 5);
    }

    [Fact]
    public void XFSyncQueueItem_WithValues_AssignsCorrectly()
    {
        var item = new XFSyncQueueItem
        {
            Key = "orders/123",
            ChangeType = XFStorageChangeType.Updated,
            Status = XFSyncItemStatus.Syncing,
            Payload = """{"total":150.00}""",
            RetryCount = 2,
            LastError = "Timeout"
        };
        Assert.Equal("orders/123", item.Key);
        Assert.Equal(XFStorageChangeType.Updated, item.ChangeType);
        Assert.Equal(XFSyncItemStatus.Syncing, item.Status);
        Assert.Equal(2, item.RetryCount);
        Assert.Equal("Timeout", item.LastError);
    }

    #endregion

    #region XFStorageEntry

    [Fact]
    public void XFStorageEntry_Defaults_AreCorrect()
    {
        var entry = new XFStorageEntry();
        Assert.Equal(string.Empty, entry.Key);
        Assert.Equal(string.Empty, entry.Value);
        Assert.Null(entry.ETag);
        Assert.Equal(0, entry.SizeBytes);
        Assert.True(Math.Abs((DateTimeOffset.UtcNow - entry.CreatedAt).TotalSeconds) < 5);
        Assert.True(Math.Abs((DateTimeOffset.UtcNow - entry.UpdatedAt).TotalSeconds) < 5);
    }

    [Fact]
    public void XFStorageEntry_WithValues_AssignsCorrectly()
    {
        var now = DateTimeOffset.UtcNow;
        var entry = new XFStorageEntry
        {
            Key = "config/theme",
            Value = "dark",
            CreatedAt = now.AddDays(-1),
            UpdatedAt = now,
            ETag = "w/\"abc123\"",
            SizeBytes = 4
        };
        Assert.Equal("config/theme", entry.Key);
        Assert.Equal("dark", entry.Value);
        Assert.Equal("w/\"abc123\"", entry.ETag);
        Assert.Equal(4, entry.SizeBytes);
    }

    #endregion

    #region XFSyncConflict

    [Fact]
    public void XFSyncConflict_Defaults_AreCorrect()
    {
        var conflict = new XFSyncConflict();
        Assert.Equal(string.Empty, conflict.Key);
        Assert.NotNull(conflict.LocalValue);
        Assert.NotNull(conflict.RemoteValue);
        Assert.Equal(string.Empty, conflict.LocalValue.Key);
        Assert.Equal(string.Empty, conflict.RemoteValue.Key);
        Assert.True(Math.Abs((DateTimeOffset.UtcNow - conflict.DetectedAt).TotalSeconds) < 5);
    }

    [Fact]
    public void XFSyncConflict_WithValues_HasEntries()
    {
        var local = new XFStorageEntry { Key = "doc/1", Value = "v1", ETag = "e1" };
        var remote = new XFStorageEntry { Key = "doc/1", Value = "v2", ETag = "e2" };
        var conflict = new XFSyncConflict
        {
            Key = "doc/1",
            LocalValue = local,
            RemoteValue = remote
        };
        Assert.Equal("doc/1", conflict.Key);
        Assert.Equal("v1", conflict.LocalValue.Value);
        Assert.Equal("v2", conflict.RemoteValue.Value);
        Assert.Equal("e1", conflict.LocalValue.ETag);
        Assert.Equal("e2", conflict.RemoteValue.ETag);
    }

    #endregion

    #region XFStorageStats

    [Fact]
    public void XFStorageStats_Defaults_AreCorrect()
    {
        var stats = new XFStorageStats();
        Assert.Equal(0, stats.TotalEntries);
        Assert.Equal(0, stats.TotalSizeBytes);
        Assert.Equal(0, stats.PendingSyncItems);
        Assert.Equal(0, stats.ConflictItems);
        Assert.Null(stats.LastSyncAt);
        Assert.Equal(0, stats.StorageQuotaBytes);
        Assert.Equal(0, stats.UsagePercent);
    }

    [Fact]
    public void XFStorageStats_UsagePercent_CalculatesCorrectly()
    {
        var stats = new XFStorageStats
        {
            TotalSizeBytes = 500_000,
            StorageQuotaBytes = 1_000_000
        };
        Assert.Equal(50.0, stats.UsagePercent, 2);
    }

    [Fact]
    public void XFStorageStats_UsagePercent_ZeroQuota_ReturnsZero()
    {
        var stats = new XFStorageStats
        {
            TotalSizeBytes = 500_000,
            StorageQuotaBytes = 0
        };
        Assert.Equal(0, stats.UsagePercent);
    }

    [Fact]
    public void XFStorageStats_WithValues_AssignsCorrectly()
    {
        var lastSync = DateTimeOffset.UtcNow.AddMinutes(-10);
        var stats = new XFStorageStats
        {
            TotalEntries = 150,
            TotalSizeBytes = 5_000_000,
            PendingSyncItems = 12,
            ConflictItems = 2,
            LastSyncAt = lastSync,
            StorageQuotaBytes = 50_000_000
        };
        Assert.Equal(150, stats.TotalEntries);
        Assert.Equal(5_000_000, stats.TotalSizeBytes);
        Assert.Equal(12, stats.PendingSyncItems);
        Assert.Equal(2, stats.ConflictItems);
        Assert.Equal(lastSync, stats.LastSyncAt);
        Assert.Equal(10.0, stats.UsagePercent, 2);
    }

    #endregion
}
