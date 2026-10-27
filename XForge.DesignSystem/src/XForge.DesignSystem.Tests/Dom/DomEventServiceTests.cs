using XForge.Core.Dom;
using FluentAssertions;
using Microsoft.JSInterop;
using Xunit;

namespace XForge.Tests.Dom;

public class DomEventServiceTests
{
    [Fact]
    public void ActiveSubscriptionCount_StartsAtZero()
    {
        var svc = new DomEventService();
        svc.ActiveSubscriptionCount.Should().Be(0);
    }

    [Fact]
    public void RegisterListener_Stores()
    {
        var svc = new DomEventService();
        var fake = new FakeListener();
        svc.RegisterListener("btn1", fake);
        // No public read API; verify via SubscribeAsync path.
    }

    [Fact]
    public async Task SubscribeAsync_WithoutRegisteredListener_Throws()
    {
        var svc = new DomEventService();
        var dotnet = DotNetObjectReference.Create(new object());
        var act = async () => await svc.SubscribeAsync("click", "missing", dotnet);
        await act.Should().ThrowAsync<System.InvalidOperationException>();
    }

    [Fact]
    public async Task SubscribeAsync_IncrementsCounter()
    {
        var svc = new DomEventService();
        svc.RegisterListener("el1", new FakeListener());
        var dotnet = DotNetObjectReference.Create(new object());
        var sub = await svc.SubscribeAsync("click", "el1", dotnet);
        svc.ActiveSubscriptionCount.Should().Be(1);
        await sub.DisposeAsync();
        svc.ActiveSubscriptionCount.Should().Be(0);
    }

    private sealed class FakeListener : IDomEventListener
    {
        public Task<IAsyncDisposable> SubscribeAsync(string eventName, string elementId, DotNetObjectReference<object> callback)
            => Task.FromResult<IAsyncDisposable>(new FakeSub());
    }

    private sealed class FakeSub : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}