using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.CrossPlatform;

namespace XForge.DesignSystem.Tests.Components.CrossPlatform;

public class XFDeviceBridgeExtendedTest : XForgeTestContext
{
    public XFDeviceBridgeExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDeviceBridge> RenderBridge(
        Action<ComponentParameterCollectionBuilder<XFDeviceBridge>>? configure = null)
    {
        return Render<XFDeviceBridge>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment<XFDeviceContext>)(ctx => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, ctx.Platform.ToString());
                builder.CloseElement();
            }));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Default_RendersDeviceBridge()
    {
        var cut = RenderBridge();
        Assert.Contains("xf-device-bridge", cut.Markup);
    }

    [Fact]
    public void Extended_Default_IsOnline()
    {
        var cut = RenderBridge();
        Assert.Contains("xf-device-online", cut.Markup);
        Assert.DoesNotContain("xf-device-offline", cut.Markup);
    }

    [Fact]
    public void Extended_Default_PlatformIsWeb()
    {
        var cut = RenderBridge();
        Assert.Contains("xf-device-web", cut.Markup);
    }

    [Fact]
    public void Extended_Default_FormFactorIsDesktop()
    {
        var cut = RenderBridge();
        Assert.Contains("xf-device-desktop", cut.Markup);
    }

    [Fact]
    public void Extended_ForcePlatformAndroid_RendersAndroid()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.Android));
        Assert.Contains("xf-device-android", cut.Markup);
        Assert.DoesNotContain("xf-device-web", cut.Markup);
    }

    [Fact]
    public void Extended_ForcePlatformIos_RendersIos()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.iOS));
        Assert.Contains("xf-device-ios", cut.Markup);
    }

    [Fact]
    public void Extended_ForcePlatformWindows_RendersWindows()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.Windows));
        Assert.Contains("xf-device-windows", cut.Markup);
    }

    [Fact]
    public void Extended_ForcePlatformMacOs_RendersMacOs()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.macOS));
        Assert.Contains("xf-device-macos", cut.Markup);
    }

    [Fact]
    public void Extended_ForcePlatformLinux_RendersLinux()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.Linux));
        Assert.Contains("xf-device-linux", cut.Markup);
    }

    [Fact]
    public void Extended_ForceFormFactorMobile_RendersMobile()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForceFormFactor, XFDeviceFormFactor.Phone));
        Assert.Contains("xf-device-phone", cut.Markup);
        Assert.DoesNotContain("xf-device-desktop", cut.Markup);
    }

    [Fact]
    public void Extended_ForceFormFactorTablet_RendersTablet()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForceFormFactor, XFDeviceFormFactor.Tablet));
        Assert.Contains("xf-device-tablet", cut.Markup);
    }

    [Fact]
    public void Extended_CustomClass_AppliesClass()
    {
        var cut = RenderBridge(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Extended_ChildContent_ReceivesContext()
    {
        var cut = RenderBridge();
        Assert.Contains("Web", cut.Markup);
    }

    [Fact]
    public void Extended_DataAttributes_SetCorrectly()
    {
        var cut = RenderBridge();
        var div = cut.Find(".xf-device-bridge");
        Assert.Equal("web", div.GetAttribute("data-platform"));
        Assert.Equal("desktop", div.GetAttribute("data-form-factor"));
    }

    [Fact]
    public void Extended_DataAttributes_ForcePlatform_SetCorrectly()
    {
        var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, XFDevicePlatform.Android));
        var div = cut.Find(".xf-device-bridge");
        Assert.Equal("android", div.GetAttribute("data-platform"));
    }

    [Fact]
    public void Extended_SetOnlineStatus_Offline_RendersOfflineClass()
    {
        var cut = RenderBridge();
        var bridge = cut.Instance;
        cut.InvokeAsync(() => bridge.SetOnlineStatus(false));
        cut.Render();
        Assert.Contains("xf-device-offline", cut.Markup);
        Assert.DoesNotContain("xf-device-online", cut.Markup);
    }

    [Fact]
    public void Extended_SetOnlineStatus_BackToOnline()
    {
        var cut = RenderBridge();
        var bridge = cut.Instance;
        cut.InvokeAsync(() => bridge.SetOnlineStatus(false));
        cut.Render();
        cut.InvokeAsync(() => bridge.SetOnlineStatus(true));
        cut.Render();
        Assert.Contains("xf-device-online", cut.Markup);
        Assert.DoesNotContain("xf-device-offline", cut.Markup);
    }

    [Fact]
    public void Extended_SetFormFactor_Mobile()
    {
        var cut = RenderBridge();
        var bridge = cut.Instance;
        cut.InvokeAsync(() => bridge.SetFormFactor(XFDeviceFormFactor.Phone));
        cut.Render();
        Assert.Contains("xf-device-phone", cut.Markup);
        Assert.DoesNotContain("xf-device-desktop", cut.Markup);
    }

    [Fact]
    public void Extended_SetFormFactor_Tablet()
    {
        var cut = RenderBridge();
        var bridge = cut.Instance;
        cut.InvokeAsync(() => bridge.SetFormFactor(XFDeviceFormFactor.Tablet));
        cut.Render();
        Assert.Contains("xf-device-tablet", cut.Markup);
    }

    [Fact]
    public void Extended_OnOnlineStatusChanged_Raised()
    {
        bool? changedTo = null;
        var cut = RenderBridge(p => p.Add(x => x.OnOnlineStatusChanged, EventCallback.Factory.Create<bool>(this, val => changedTo = val)));
        var bridge = cut.Instance;
        cut.InvokeAsync(() => bridge.SetOnlineStatus(false));
        Assert.NotNull(changedTo);
        Assert.False(changedTo);
    }

    [Fact]
    public void Extended_DeviceContext_HasFeature()
    {
        var ctx = new XFDeviceContext(XFDevicePlatform.Web, XFDeviceFormFactor.Desktop,
            new List<XFDeviceFeatureInfo>
            {
                new() { Feature = XFDeviceFeature.Camera, IsAvailable = true, Permission = XFPermissionStatus.Granted, Description = "Camera" }
            }, true);

        Assert.True(ctx.HasFeature(XFDeviceFeature.Camera));
        Assert.False(ctx.HasFeature(XFDeviceFeature.Nfc));
    }

    [Fact]
    public void Extended_DeviceContext_GetPermission()
    {
        var ctx = new XFDeviceContext(XFDevicePlatform.Web, XFDeviceFormFactor.Desktop,
            new List<XFDeviceFeatureInfo>
            {
                new() { Feature = XFDeviceFeature.Camera, IsAvailable = true, Permission = XFPermissionStatus.Granted, Description = "Camera" }
            }, true);

        Assert.Equal(XFPermissionStatus.Granted, ctx.GetPermission(XFDeviceFeature.Camera));
        Assert.Equal(XFPermissionStatus.Unknown, ctx.GetPermission(XFDeviceFeature.Nfc));
    }

    [Fact]
    public void Extended_DeviceContext_IsOnline()
    {
        var ctx = new XFDeviceContext(XFDevicePlatform.Web, XFDeviceFormFactor.Desktop,
            new List<XFDeviceFeatureInfo>(), true);
        Assert.True(ctx.IsOnline);

        var ctxOffline = new XFDeviceContext(XFDevicePlatform.Web, XFDeviceFormFactor.Desktop,
            new List<XFDeviceFeatureInfo>(), false);
        Assert.False(ctxOffline.IsOnline);
    }

    [Fact]
    public void Extended_AllPlatforms()
    {
        foreach (XFDevicePlatform platform in Enum.GetValues<XFDevicePlatform>())
        {
            var cut = RenderBridge(p => p.Add(x => x.ForcePlatform, platform));
            Assert.Contains($"xf-device-{platform.ToString().ToLower()}", cut.Markup);
        }
    }

    [Fact]
    public void Extended_AllFormFactors()
    {
        foreach (XFDeviceFormFactor ff in Enum.GetValues<XFDeviceFormFactor>())
        {
            var cut = RenderBridge(p => p.Add(x => x.ForceFormFactor, ff));
            Assert.Contains($"xf-device-{ff.ToString().ToLower()}", cut.Markup);
        }
    }
}
