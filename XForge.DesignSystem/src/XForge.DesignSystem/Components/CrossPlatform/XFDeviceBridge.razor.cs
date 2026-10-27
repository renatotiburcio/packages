namespace XForge.DesignSystem.Components.CrossPlatform;

/// <summary>Context provided by XFDeviceBridge with platform, form factor, features, and connectivity.</summary>
public record XFDeviceContext
{
    public XFDevicePlatform Platform { get; }
    public XFDeviceFormFactor FormFactor { get; }
    public IReadOnlyList<XFDeviceFeatureInfo> Features { get; }
    public bool IsOnline { get; }

    public XFDeviceContext(XFDevicePlatform platform, XFDeviceFormFactor formFactor,
        IReadOnlyList<XFDeviceFeatureInfo> features, bool isOnline)
    {
        Platform = platform;
        FormFactor = formFactor;
        Features = features;
        IsOnline = isOnline;
    }

    public bool HasFeature(XFDeviceFeature feature) =>
        Features.Any(f => f.Feature == feature && f.IsAvailable);

    public XFPermissionStatus GetPermission(XFDeviceFeature feature) =>
        Features.FirstOrDefault(f => f.Feature == feature)?.Permission ?? XFPermissionStatus.Unknown;
}
