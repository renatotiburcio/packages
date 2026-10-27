using Microsoft.Extensions.DependencyInjection;


// T051: Default license service.
namespace XForge.DesignSystem.Licensing;

/// <summary>Default in-memory license service. Production should use a file-based license provider.</summary>
public sealed class XFLicenseService : IXFLicenseService
{
    private XFLicense _current = new(XFLicenseTier.Community, null, null, Array.Empty<string>());

    public XFLicense Current => _current;
    public event EventHandler<XFLicenseChangedEventArgs>? LicenseChanged;

    public void Activate(XFLicense license) { _current = license; LicenseChanged?.Invoke(this, new(license)); }

    public bool IsPermitted(TierRequirement requirement, string? extensionId = null)
    {
        if (extensionId is not null && _current.ActivatedExtensions.Count > 0 && !_current.ActivatedExtensions.Contains(extensionId)) return false;
        return _current.IsValid(requirement);
    }
}

/// <summary>DI extension.</summary>
public static class XFLicenseServiceCollectionExtensions
{
    public static IServiceCollection AddXFLicensing(this IServiceCollection services)
    {
        services.AddSingleton<IXFLicenseService, XFLicenseService>();
        return services;
    }
}
