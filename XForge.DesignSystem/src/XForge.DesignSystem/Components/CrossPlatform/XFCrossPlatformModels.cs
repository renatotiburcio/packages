namespace XForge.DesignSystem.Components.CrossPlatform;

// ── Enums ──────────────────────────────────────────────────────────────────

/// <summary>Detected gesture type from pointer/touch events.</summary>
public enum XFGestureType
{
    Tap,
    DoubleTap,
    LongPress,
    SwipeLeft,
    SwipeRight,
    SwipeUp,
    SwipeDown,
    PinchIn,
    PinchOut,
    Rotate
}

/// <summary>Gesture recognition state.</summary>
public enum XFGestureState
{
    Idle,
    Tracking,
    Recognized,
    Cancelled
}

/// <summary>Device platform detection.</summary>
public enum XFDevicePlatform
{
    Web,
    iOS,
    Android,
    Windows,
    macOS,
    Linux
}

/// <summary>Device form factor.</summary>
public enum XFDeviceFormFactor
{
    Phone,
    Tablet,
    Desktop,
    Tv,
    Watch
}

/// <summary>Native device feature type.</summary>
public enum XFDeviceFeature
{
    Camera,
    Geolocation,
    Biometrics,
    Accelerometer,
    Gyroscope,
    Magnetometer,
    Barometer,
    Nfc,
    Bluetooth,
    Haptics,
    FileSystem,
    Notifications
}

/// <summary>Permission status for device features.</summary>
public enum XFPermissionStatus
{
    Unknown,
    Granted,
    Denied,
    Restricted,
    PromptRequired
}

/// <summary>Sync conflict resolution strategy.</summary>
public enum XFSyncConflictStrategy
{
    ClientWins,
    ServerWins,
    LastWriteWins,
    Manual,
    Merge
}

/// <summary>Sync item status.</summary>
public enum XFSyncItemStatus
{
    Pending,
    Syncing,
    Synced,
    Conflict,
    Failed
}

/// <summary>Offline storage change type.</summary>
public enum XFStorageChangeType
{
    Created,
    Updated,
    Deleted
}

// ── Records ────────────────────────────────────────────────────────────────

/// <summary>Gesture event data from pointer/touch tracking.</summary>
public record XFGestureEvent
{
    public XFGestureType Type { get; init; }
    public XFGestureState State { get; init; }
    public double StartX { get; init; }
    public double StartY { get; init; }
    public double CurrentX { get; init; }
    public double CurrentY { get; init; }
    public double DeltaX { get; init; }
    public double DeltaY { get; init; }
    public double Distance { get; init; }
    public double VelocityX { get; init; }
    public double VelocityY { get; init; }
    public double Scale { get; init; }
    public double Rotation { get; init; }
    public long DurationMs { get; init; }
    public int PointerCount { get; init; }
}

/// <summary>Gesture handler configuration options.</summary>
public record XFGestureOptions
{
    public double SwipeThreshold { get; init; } = 50.0;
    public double SwipeVelocityThreshold { get; init; } = 0.3;
    public long LongPressDurationMs { get; init; } = 500;
    public long DoubleTapWindowMs { get; init; } = 300;
    public double PinchThreshold { get; init; } = 0.1;
    public bool EnableSwipe { get; init; } = true;
    public bool EnablePinch { get; init; } = true;
    public bool EnableLongPress { get; init; } = true;
    public bool EnableDoubleTap { get; init; } = true;
    public bool EnableRotate { get; init; }
}

/// <summary>Geolocation coordinates.</summary>
public record XFGeoLocation
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Altitude { get; init; }
    public double? Accuracy { get; init; }
    public double? Speed { get; init; }
    public double? Heading { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>Camera capture result.</summary>
public record XFCameraCapture
{
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "image/jpeg";
    public int Width { get; init; }
    public int Height { get; init; }
    public string? FileName { get; init; }
}

/// <summary>Biometric authentication result.</summary>
public record XFBiometricResult
{
    public bool IsAuthenticated { get; init; }
    public string? UserId { get; init; }
    public XFPermissionStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>Device feature capability descriptor.</summary>
public record XFDeviceFeatureInfo
{
    public XFDeviceFeature Feature { get; init; }
    public bool IsAvailable { get; init; }
    public XFPermissionStatus Permission { get; init; }
    public string? Description { get; init; }
}

/// <summary>Sync queue item for offline storage.</summary>
public record XFSyncQueueItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Key { get; init; } = "";
    public XFStorageChangeType ChangeType { get; init; }
    public XFSyncItemStatus Status { get; init; } = XFSyncItemStatus.Pending;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public string? Payload { get; init; }
    public int RetryCount { get; init; }
    public string? LastError { get; init; }
}

/// <summary>Storage entry with metadata.</summary>
public record XFStorageEntry
{
    public string Key { get; init; } = "";
    public string Value { get; init; } = "";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? ETag { get; init; }
    public long SizeBytes { get; init; }
}

/// <summary>Sync conflict descriptor.</summary>
public record XFSyncConflict
{
    public string Key { get; init; } = "";
    public XFStorageEntry LocalValue { get; init; } = new();
    public XFStorageEntry RemoteValue { get; init; } = new();
    public DateTimeOffset DetectedAt { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>Offline storage statistics.</summary>
public record XFStorageStats
{
    public int TotalEntries { get; init; }
    public long TotalSizeBytes { get; init; }
    public int PendingSyncItems { get; init; }
    public int ConflictItems { get; init; }
    public DateTimeOffset? LastSyncAt { get; init; }
    public long StorageQuotaBytes { get; init; }
    public double UsagePercent => StorageQuotaBytes > 0 ? (double)TotalSizeBytes / StorageQuotaBytes * 100 : 0;
}
