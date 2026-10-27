using XForge.DesignSystem.Components.CrossPlatform;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFDeviceBridgeComponentTest
{
    [Fact]
    public void XFDeviceContext_CreatesWithCorrectProperties()
    {
        var features = new List<XFDeviceFeatureInfo>
        {
            new() { Feature = XFDeviceFeature.Camera, IsAvailable = true, Permission = XFPermissionStatus.Granted, Description = "Camera" }
        };
        var context = new XFDeviceContext(XFDevicePlatform.iOS, XFDeviceFormFactor.Phone, features, true);

        Assert.Equal(XFDevicePlatform.iOS, context.Platform);
        Assert.Equal(XFDeviceFormFactor.Phone, context.FormFactor);
        Assert.Single(context.Features);
        Assert.True(context.IsOnline);
    }

    [Fact]
    public void XFDeviceContext_HasFeature_ReturnsCorrectResult()
    {
        var features = new List<XFDeviceFeatureInfo>
        {
            new() { Feature = XFDeviceFeature.Camera, IsAvailable = true, Permission = XFPermissionStatus.Granted, Description = "Camera" },
            new() { Feature = XFDeviceFeature.Nfc, IsAvailable = false, Permission = XFPermissionStatus.Unknown, Description = "NFC" }
        };
        var context = new XFDeviceContext(XFDevicePlatform.Web, XFDeviceFormFactor.Desktop, features, false);

        Assert.True(context.HasFeature(XFDeviceFeature.Camera));
        Assert.False(context.HasFeature(XFDeviceFeature.Nfc));
    }

    [Fact]
    public void XFDeviceContext_GetPermission_ReturnsCorrectStatus()
    {
        var features = new List<XFDeviceFeatureInfo>
        {
            new() { Feature = XFDeviceFeature.Geolocation, IsAvailable = true, Permission = XFPermissionStatus.Denied, Description = "Geo" }
        };
        var context = new XFDeviceContext(XFDevicePlatform.Android, XFDeviceFormFactor.Phone, features, true);

        Assert.Equal(XFPermissionStatus.Denied, context.GetPermission(XFDeviceFeature.Geolocation));
        Assert.Equal(XFPermissionStatus.Unknown, context.GetPermission(XFDeviceFeature.Bluetooth));
    }
}