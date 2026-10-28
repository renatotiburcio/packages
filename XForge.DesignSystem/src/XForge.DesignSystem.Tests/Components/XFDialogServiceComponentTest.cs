using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Dialog;

namespace XForge.DesignSystem.Tests.Components;

public class XFDialogServiceComponentTest : XForgeTestContext
{
    private readonly XFDialogService dialogService = new();

    public XFDialogServiceComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton(dialogService);
    }

    [Fact]
    public async Task Provider_WhenControllerCloses_ReturnsValue()
    {
        var provider = Render<XFDialogProvider>();
        var task = dialogService.ShowAsync(
            new XFDialogOptions { Title = "Choose" },
            controller => builder => builder.AddContent(0, "Body"),
            controller => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, () => controller.CloseAsync("Published")));
                builder.AddContent(2, "Return");
                builder.CloseElement();
            });

        provider.WaitForAssertion(() => Assert.Contains("Choose", provider.Markup));
        provider.FindAll("button").First(button => button.TextContent.Contains("Return")).Click();

        var result = await task;

        Assert.False(result.Cancelled);
        Assert.Equal("Published", result.GetValue<string>());
        Assert.DoesNotContain("Choose", provider.Markup);
    }

    [Fact]
    public async Task Provider_WhenDialogCloses_ReturnsCancelled()
    {
        var provider = Render<XFDialogProvider>();
        var task = dialogService.ShowAsync(new XFDialogOptions { Title = "Confirm" });

        provider.WaitForAssertion(() => Assert.Contains("Confirm", provider.Markup));
        provider.Find("button[aria-label='Fechar']").Click();

        var result = await task;

        Assert.True(result.Cancelled);
    }

    [Fact]
    public async Task Provider_WhenControllerCancels_RemovesDialog()
    {
        var provider = Render<XFDialogProvider>();
        var task = dialogService.ShowAsync(
            new XFDialogOptions { Title = "Choose status" },
            controller => builder => builder.AddContent(0, "Body"),
            controller => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, controller.CancelAsync));
                builder.AddContent(2, "Cancel");
                builder.CloseElement();
            });

        provider.WaitForAssertion(() => Assert.Contains("Choose status", provider.Markup));
        provider.FindAll("button").First(button => button.TextContent.Contains("Cancel")).Click();

        var result = await task;

        Assert.True(result.Cancelled);
        Assert.DoesNotContain("Choose status", provider.Markup);
    }

    [Fact]
    public async Task ConfirmAsync_WhenActionIsRunning_KeepsDialogOpenAndShowsLoading()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var provider = Render<XFDialogProvider>();
        var task = dialogService.ConfirmAsync(
            "Archive",
            "Async action",
            confirmText: "Archive",
            onConfirm: () => completion.Task);

        provider.WaitForAssertion(() => Assert.Contains("Archive", provider.Markup));
        provider.FindAll("button").First(button => button.TextContent.Contains("Archive")).Click();

        provider.WaitForAssertion(() => Assert.Contains("Processando...", provider.Markup));
        Assert.Contains("Archive", provider.Markup);

        completion.SetResult("Done");
        var result = await task;

        provider.WaitForAssertion(() => Assert.DoesNotContain("Archive", provider.Markup));
        Assert.False(result.Cancelled);
        Assert.Equal("Done", result.GetValue<string>());
    }

    [Fact]
    public async Task ConfirmAsync_WithLocalizedTexts_UsesCustomBusyAndDefaultDescription()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var provider = Render<XFDialogProvider>();
        var task = dialogService.ConfirmAsync(
            "Excluir",
            confirmText: "Excluir agora",
            cancelText: "Voltar",
            onConfirm: () => completion.Task,
            busyText: "Excluindo...",
            defaultDescription: "Revise antes de excluir.");

        provider.WaitForAssertion(() => Assert.Contains("Revise antes de excluir.", provider.Markup));
        provider.FindAll("button").First(button => button.TextContent.Contains("Excluir agora")).Click();

        provider.WaitForAssertion(() => Assert.Contains("Excluindo...", provider.Markup));

        completion.SetResult(true);
        var result = await task;

        Assert.False(result.Cancelled);
        Assert.True(result.GetValue<bool>());
    }
}
