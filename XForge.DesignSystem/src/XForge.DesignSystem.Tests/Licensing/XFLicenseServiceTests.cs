// T051: Tests for XFLicenseService.
using Xunit;
using XForge.DesignSystem.Licensing;

namespace XForge.DesignSystem.Tests.Licensing;

public class XFLicenseTests
{
    [Fact] public void Community_IsAlwaysValid()
    {
        var l = new XFLicense(XFLicenseTier.Community, null, null, Array.Empty<string>());
        Assert.True(l.IsValid(TierRequirement.Community));
    }

    [Fact] public void Commercial_Expires_Invalid()
    {
        var l = new XFLicense(XFLicenseTier.Commercial, "Acme", DateTimeOffset.UtcNow.AddDays(-1), Array.Empty<string>());
        Assert.True(l.IsExpired);
        Assert.False(l.IsValid(TierRequirement.Commercial));
    }

    [Fact] public void Enterprise_SatisfiesCommercial()
    {
        var l = new XFLicense(XFLicenseTier.Enterprise, "Acme", null, Array.Empty<string>());
        Assert.True(l.IsValid(TierRequirement.Enterprise));
        Assert.True(l.IsValid(TierRequirement.Commercial));
    }
}

public class XFLicenseServiceTests
{
    [Fact] public void Default_IsCommunity() => Assert.Equal(XFLicenseTier.Community, new XFLicenseService().Current.Tier);

    [Fact] public void Activate_FiresEvent()
    {
        var s = new XFLicenseService();
        var fired = false;
        s.LicenseChanged += (_, _) => fired = true;
        s.Activate(new XFLicense(XFLicenseTier.Commercial, "Acme", null, Array.Empty<string>()));
        Assert.True(fired);
        Assert.Equal(XFLicenseTier.Commercial, s.Current.Tier);
    }

    [Fact] public void IsPermitted_ExtensionNotInActivated_False()
    {
        var s = new XFLicenseService();
        s.Activate(new XFLicense(XFLicenseTier.Enterprise, "Acme", null, new[] { "DataGrid" }));
        Assert.True(s.IsPermitted(TierRequirement.Commercial, "DataGrid"));
        Assert.False(s.IsPermitted(TierRequirement.Commercial, "Charts"));
    }
}