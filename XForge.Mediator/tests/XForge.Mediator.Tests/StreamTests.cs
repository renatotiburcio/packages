using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mediator.Tests;

public class StreamTests
{
    private static ServiceProvider BuildProvider(Action<XForgeMediatorServiceConfiguration>? cfg = null)
        => new ServiceCollection().AddXForgeMediator(c =>
        {
            c.RegisterServicesFromAssemblyContaining<Numbers>();
            cfg?.Invoke(c);
        }).BuildServiceProvider();

    [Fact]
    public async Task Stream_request_yields_items()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();

        var items = new List<int>();
        await foreach (var item in mediator.CreateStream(new Numbers { Count = 3 }))
        {
            items.Add(item);
        }

        Assert.Equal(new[] { 0, 1, 2 }, items);
    }

    [Fact]
    public async Task Stream_object_dispatch_discovers_item_type()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();
        object request = new Numbers { Count = 2 };

        var items = new List<object?>();
        await foreach (var item in mediator.CreateStream(request))
        {
            items.Add(item);
        }

        Assert.Equal(new object?[] { 0, 1 }, items);
    }

    [Fact]
    public async Task Stream_behavior_wraps_handler()
    {
        await using var provider = BuildProvider(c => c.AddOpenStreamBehavior(typeof(StreamDoublingBehavior<,>)));
        var mediator = provider.GetRequiredService<ISender>();

        var items = new List<int>();
        await foreach (var item in mediator.CreateStream(new Numbers { Count = 2 }))
        {
            items.Add(item);
        }

        Assert.Equal(new[] { 0, 0, 1, 1 }, items);
    }

    [Fact]
    public void Stream_without_handler_throws_on_creation()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator()
            .BuildServiceProvider();
        using var provider = services;

        // o wrapper resolve o handler de forma adiantada (eager), antes de enumerar o stream
        Assert.Throws<HandlerNotFoundException>(() => provider.GetRequiredService<ISender>().CreateStream(new Numbers()));
    }

    [Fact]
    public async Task Stream_object_without_stream_interface_throws_immediately()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();

        Assert.Throws<InvalidOperationException>(() => mediator.CreateStream("not a stream request"));
    }
}
