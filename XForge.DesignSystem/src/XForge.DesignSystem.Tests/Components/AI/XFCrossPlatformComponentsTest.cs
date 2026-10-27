using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.CrossPlatform;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFCrossPlatformComponentsTest : BunitContext
{
    #region XFResponsiveContainer

    [Fact]
    public void ResponsiveContainer_RendersDefault()
    {
        Services.AddSingleton<XFBreakpointService>();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFResponsiveContainer>();

        Assert.Contains("xf-responsive-container", cut.Markup);
        Assert.Contains("data-breakpoint", cut.Markup);
    }

    [Fact]
    public void ResponsiveContainer_ShowsBreakpointContext()
    {
        Services.AddSingleton<XFBreakpointService>();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFResponsiveContainer>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Mobile:{ctx.IsMobile} Tablet:{ctx.IsTablet} Desktop:{ctx.IsDesktop}");
            }));

        Assert.Contains("Mobile:", cut.Markup);
    }

    [Fact]
    public void ResponsiveContainer_CustomMobileUpTo_Applied()
    {
        Services.AddSingleton<XFBreakpointService>();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFResponsiveContainer>(p => p
            .Add(x => x.MobileUpTo, XFBreakpoint.Md));

        Assert.Contains("xf-responsive-container", cut.Markup);
    }

    [Fact]
    public void ResponsiveContainer_CustomClass_Applied()
    {
        Services.AddSingleton<XFBreakpointService>();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFResponsiveContainer>(p => p
            .Add(x => x.Class, "my-custom-class"));

        Assert.Contains("my-custom-class", cut.Markup);
    }

    #endregion

    #region XFGestureHandler

    [Fact]
    public void GestureHandler_RendersDefault()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFGestureHandler>();

        Assert.Contains("xf-gesture-handler", cut.Markup);
        Assert.Contains("touch-none", cut.Markup);
        Assert.Contains("select-none", cut.Markup);
    }

    [Fact]
    public void GestureHandler_Disabled_ShowsDisabledState()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFGestureHandler>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("pointer-events-none", cut.Markup);
        Assert.Contains("opacity-50", cut.Markup);
    }

    [Fact]
    public void GestureHandler_WithChildContent_Renders()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFGestureHandler>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, "Gesture area");
            }));

        Assert.Contains("Gesture area", cut.Markup);
    }

    [Fact]
    public void GestureHandler_CustomClass_Applied()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<XFGestureHandler>(p => p
            .Add(x => x.Class, "swipe-zone"));

        Assert.Contains("swipe-zone", cut.Markup);
    }

    #endregion

    #region XFDeviceBridge

    [Fact]
    public void DeviceBridge_RendersDefault()
    {
        var cut = Render<XFDeviceBridge>();

        Assert.Contains("xf-device-bridge", cut.Markup);
        Assert.Contains("data-platform", cut.Markup);
        Assert.Contains("data-form-factor", cut.Markup);
    }

    [Fact]
    public void DeviceBridge_ShowsDeviceContext()
    {
        var cut = Render<XFDeviceBridge>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Platform:{ctx.Platform} FormFactor:{ctx.FormFactor}");
            }));

        Assert.Contains("Platform:Web", cut.Markup);
        Assert.Contains("FormFactor:Desktop", cut.Markup);
    }

    [Fact]
    public void DeviceBridge_ForcePlatform_Applied()
    {
        var cut = Render<XFDeviceBridge>(p => p
            .Add(x => x.ForcePlatform, XFDevicePlatform.iOS)
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Platform:{ctx.Platform}");
            }));

        Assert.Contains("Platform:iOS", cut.Markup);
    }

    [Fact]
    public void DeviceBridge_ForceFormFactor_Applied()
    {
        var cut = Render<XFDeviceBridge>(p => p
            .Add(x => x.ForceFormFactor, XFDeviceFormFactor.Phone)
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"FormFactor:{ctx.FormFactor}");
            }));

        Assert.Contains("FormFactor:Phone", cut.Markup);
    }

    [Fact]
    public void DeviceBridge_HasFeature_ReturnsCorrectValue()
    {
        var cut = Render<XFDeviceBridge>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Camera:{ctx.HasFeature(XFDeviceFeature.Camera)}");
            }));

        Assert.Contains("Camera:True", cut.Markup);
    }

    [Fact]
    public void DeviceBridge_OnlineClass_Applied()
    {
        var cut = Render<XFDeviceBridge>();

        Assert.Contains("xf-device-online", cut.Markup);
    }

    #endregion

    #region XFOfflineStorage

    [Fact]
    public void OfflineStorage_RendersDefault()
    {
        var cut = Render<XFOfflineStorage>();

        Assert.Contains("xf-offline-storage", cut.Markup);
        Assert.Contains("xf-storage-online", cut.Markup);
    }

    [Fact]
    public void OfflineStorage_ShowsStorageContext()
    {
        var cut = Render<XFOfflineStorage>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Entries:{ctx.EntryCount} Online:{ctx.IsOnline}");
            }));

        Assert.Contains("Entries:0", cut.Markup);
        Assert.Contains("Online:True", cut.Markup);
    }

    [Fact]
    public void OfflineStorage_SetItem_IncreasesCount()
    {
        var cut = Render<XFOfflineStorage>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Entries:{ctx.EntryCount}");
            }));

        var instance = cut.Instance;
        instance.SetItem("test-key", "test-value");

        cut.Render();
        Assert.Contains("Entries:1", cut.Markup);
    }

    [Fact]
    public void OfflineStorage_GetItem_ReturnsValue()
    {
        var cut = Render<XFOfflineStorage>();
        var instance = cut.Instance;

        instance.SetItem("key1", "value1");
        var result = instance.GetItem("key1");

        Assert.Equal("value1", result);
    }

    [Fact]
    public void OfflineStorage_RemoveItem_DecreasesCount()
    {
        var cut = Render<XFOfflineStorage>(p => p
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Entries:{ctx.EntryCount}");
            }));

        var instance = cut.Instance;
        instance.SetItem("key1", "value1");
        instance.SetItem("key2", "value2");
        cut.Render();
        Assert.Contains("Entries:2", cut.Markup);

        instance.RemoveItem("key1");
        cut.Render();
        Assert.Contains("Entries:1", cut.Markup);
    }

    [Fact]
    public void OfflineStorage_StorageQuota_Shown()
    {
        var cut = Render<XFOfflineStorage>(p => p
            .Add(x => x.StorageQuotaBytes, 1024 * 1024)
            .Add(x => x.ChildContent, ctx => builder =>
            {
                builder.AddContent(0, $"Usage:{ctx.Stats.UsagePercent:F1}%");
            }));

        Assert.Contains("Usage:", cut.Markup);
    }

    #endregion

    #region Models

    [Fact]
    public void GestureEvent_RecordCreation()
    {
        var evt = new XFGestureEvent
        {
            Type = XFGestureType.SwipeRight,
            State = XFGestureState.Recognized,
            StartX = 0, StartY = 0,
            CurrentX = 100, CurrentY = 0,
            DeltaX = 100, DeltaY = 0,
            Distance = 100,
            VelocityX = 1.0, VelocityY = 0,
            Scale = 1, Rotation = 0,
            DurationMs = 100,
            PointerCount = 1
        };

        Assert.Equal(XFGestureType.SwipeRight, evt.Type);
        Assert.Equal(XFGestureState.Recognized, evt.State);
        Assert.Equal(100, evt.Distance);
        Assert.Equal(1, evt.PointerCount);
    }

    [Fact]
    public void StorageStats_UsagePercent_Calculated()
    {
        var stats = new XFStorageStats
        {
            TotalEntries = 10,
            TotalSizeBytes = 512 * 1024,
            StorageQuotaBytes = 1024 * 1024
        };

        Assert.Equal(50.0, stats.UsagePercent);
    }

    [Fact]
    public void DeviceContext_HasFeature_ReturnsFalse_ForUnavailable()
    {
        var ctx = new XFDeviceContext(
            XFDevicePlatform.Web,
            XFDeviceFormFactor.Desktop,
            new List<XFDeviceFeatureInfo>
            {
                new() { Feature = XFDeviceFeature.Biometrics, IsAvailable = false }
            },
            true);

        Assert.False(ctx.HasFeature(XFDeviceFeature.Biometrics));
    }

    #endregion
}
