// T051: License abstraction (Blazorise.Licensing pattern).
// P1, 8h, no deps.
namespace XForge.DesignSystem.Licensing;

/// <summary>Available license tiers.</summary>
public enum XFLicenseTier { Community, Commercial, Enterprise }

/// <summary>Immutable license record.</summary>
public sealed record XFLicense(
    XFLicenseTier Tier,
    string? Licensee,
    DateTimeOffset? ExpiresAt,
    IReadOnlyCollection<string> ActivatedExtensions)
{
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow;
    public bool IsValid(TierRequirement requirement) => requirement switch
    {
        TierRequirement.Community => true,
        TierRequirement.Commercial => Tier is XFLicenseTier.Commercial or XFLicenseTier.Enterprise && !IsExpired,
        TierRequirement.Enterprise => Tier == XFLicenseTier.Enterprise && !IsExpired,
        _ => false,
    };
}

/// <summary>Capability required to use a feature.</summary>
public enum TierRequirement { Community, Commercial, Enterprise }