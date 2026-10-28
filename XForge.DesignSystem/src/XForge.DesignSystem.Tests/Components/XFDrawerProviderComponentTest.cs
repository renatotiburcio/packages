using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFDrawerProviderComponentTest : XForgeTestContext
{
    private readonly XFDrawerService drawerService = new();

    public XFDrawerProviderComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton(drawerService);
    }

    [Fact]
    public async Task Provider_WhenShowAsyncCalled_OpensDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Test Drawer" },
            controller => builder => builder.AddContent(0, "Drawer content"));

        provider.Render();

        Assert.Contains("Test Drawer", provider.Markup);
        Assert.Contains("Drawer content", provider.Markup);
    }

    [Fact]
    public async Task Provider_WhenControllerCloses_CompletesTask()
    {
        XFDrawerController? capturedController = null;
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Close Test" },
            controller =>
            {
                capturedController = controller;
                return builder => builder.AddContent(0, "Content");
            });

        provider.Render();
        Assert.NotNull(capturedController);

        await capturedController!.CloseAsync();
        provider.Render();

        await task;
        Assert.DoesNotContain("Close Test", provider.Markup);
    }

    [Fact]
    public async Task Provider_WithCloseLabel_PassesLabelToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Label Test", CloseLabel = "Fechar painel" },
            controller => builder => builder.AddContent(0, "Content"));

        provider.Render();

        Assert.Contains("Label Test", provider.Markup);
        Assert.Contains("Fechar painel", provider.Markup);
    }

    [Fact]
    public async Task Provider_PassesPlacementAndSizeOptions()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "Options Test",
                Placement = XFDrawerPlacement.Left,
                Size = XFOverlaySize.Lg,
                Modal = false,
                CloseLabel = "Custom close"
            },
            controller => builder => builder.AddContent(0, "Content"));

        provider.Render();

        Assert.Contains("Options Test", provider.Markup);
    }

    [Fact]
    public async Task Provider_WithFooter_RendersFooterContent()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Footer Test" },
            controller => builder => builder.AddContent(0, "Body"),
            controller => builder => builder.AddContent(0, "Footer content"));

        provider.Render();

        Assert.Contains("Body", provider.Markup);
        Assert.Contains("Footer content", provider.Markup);
    }

    [Fact]
    public async Task Provider_WithNoSubscribers_DoesNotThrow()
    {
        var freshService = new XFDrawerService();

        await freshService.ShowAsync(new XFDrawerOptions { Title = "No subscribers" });
    }

    [Fact]
    public async Task Provider_DisposeUnsubscribesFromService()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Before dispose" },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();
        Assert.Contains("Before dispose", provider.Markup);

        provider.Dispose();

        await task;
    }

    [Fact]
    public async Task Provider_SequentialShowCloseShowCycle()
    {
        var provider = Render<XFDrawerProvider>();

        var task1 = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "First" },
            controller => builder => builder.AddContent(0, "Content 1"));

        provider.Render();
        Assert.Contains("First", provider.Markup);

        await provider.Find("button[aria-label='Fechar painel']").ClickAsync();
        await task1;

        provider.Render();
        Assert.DoesNotContain("First", provider.Markup);

        var task2 = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Second" },
            controller => builder => builder.AddContent(0, "Content 2"));

        provider.Render();
        Assert.Contains("Second", provider.Markup);
        Assert.Contains("Content 2", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithAriaLabel_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "Titled Drawer",
                AriaLabel = "Drawer with aria"
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("Drawer with aria", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithAriaLabelledBy_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "Labelled",
                AriaLabelledBy = "custom-heading-id"
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("custom-heading-id", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithDescription_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "With Description",
                Description = "A detailed description"
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("A detailed description", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithTrapFocusFalse_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "No Trap",
                TrapFocus = false
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("No Trap", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithCloseOnEscapeFalse_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "No Escape Close",
                CloseOnEscape = false
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("No Escape Close", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithTone_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "Warning Tone",
                Tone = XFSemanticColor.Warning
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("Warning Tone", provider.Markup);
    }

    [Fact]
    public async Task Provider_OptionsWithSize_PassesToDrawer()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions
            {
                Title = "Large Size",
                Size = XFOverlaySize.Xl
            },
            controller => builder => builder.AddContent(0, "content"));

        provider.Render();

        Assert.Contains("Large Size", provider.Markup);
    }

    [Fact]
    public async Task Provider_NullFooter_DoesNotRenderFooter()
    {
        var provider = Render<XFDrawerProvider>();

        var task = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "No Footer" },
            controller => builder => builder.AddContent(0, "Body"),
            null);

        provider.Render();

        Assert.Contains("Body", provider.Markup);
    }

    [Fact]
    public async Task Provider_ControllerClose_CompletesShowAsyncTask()
    {
        var provider = Render<XFDrawerProvider>();
        var controllerTask = new TaskCompletionSource<XFDrawerController>();

        var showTask = drawerService.ShowAsync(
            new XFDrawerOptions { Title = "Complete Test" },
            controller =>
            {
                controllerTask.SetResult(controller);
                return builder => builder.AddContent(0, "content");
            });

        provider.Render();
        var controller = await controllerTask.Task;
        await controller.CloseAsync();
        await showTask;

        Assert.True(showTask.IsCompleted);
        provider.Render();
        Assert.DoesNotContain("Complete Test", provider.Markup);
    }
}