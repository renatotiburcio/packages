// T051: License service interface.
namespace XForge.DesignSystem.Licensing;

/// <summary>
/// Resolves the active <see cref="XFLicense"/> and checks whether features may be used.
/// </summary>
public interface IXFLicenseService
{
    /// <summary>Currently active license. Defaults to <see cref="XFLicenseTier.Community"/>.</summary>
    XFLicense Current { get; }
    /// <summary>Thrown when a feature is requested with an insufficient license.</summary>
    event EventHandler<XFLicenseChangedEventArgs>? LicenseChanged;
    /// <summary>Activate a new license (e.g. read from appsettings.json).</summary>
    void Activate(XFLicense license);
    /// <summary>Check if a feature is permitted under the current license.</summary>
    bool IsPermitted(TierRequirement requirement, string? extensionId = null);
}

public sealed class XFLicenseChangedEventArgs(XFLicense license) : EventArgs { public XFLicense License { get; } = license; }

/// <summary>Thrown by features that require a paid license.</summary>
public sealed class XFLicenseException(string message) : Exception(message);