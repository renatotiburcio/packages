using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mediator.Tests;

public class PublishTests
{
    private static ServiceProvider BuildProvider(Action<XForgeMediatorServiceConfiguration>? cfg = null)
        => new ServiceCollection().AddXForgeMediator(c =>
        {
            c.RegisterServicesFromAssemblyContaining<Pinged>();
            cfg?.Invoke(c);
        }).BuildServiceProvider();

    [Fact]
    public async Task Publish_invokes_all_registered_handlers()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IPublisher>();
        var received = new List<string>();

        await mediator.Publish(new Pinged { Received = received });

        Assert.Equal(new[] { "h1", "h2" }, received.OrderBy(x => x));
    }

    [Fact]
    public async Task Publish_object_uses_runtime_type()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var received = new List<string>();
        object notification = new Pinged { Received = received };

        await mediator.Publish(notification);

        Assert.Equal(2, received.Count);
    }

    [Fact]
    public async Task Publish_without_handlers_is_noop()
    {
        var services = new ServiceCollection().AddXForgeMediator().BuildServiceProvider();
        await using var provider = services;
        var mediator = provider.GetRequiredService<IPublisher>();

        await mediator.Publish(new Pinged()); // nenhum handler registrado — não deve lançar
    }

    [Fact]
    public async Task Foreach_await_publisher_runs_handlers_sequentially()
    {
        await using var provider = BuildProvider(c => c.NotificationPublisher = new ForeachAwaitPublisher());
        var mediator = provider.GetRequiredService<IPublisher>();
        var received = new List<string>();

        await mediator.Publish(new Pinged { Received = received });

        // sequencial: ordem de registro preservada
        Assert.Equal(new[] { "h1", "h2" }, received);
    }

    [Fact]
    public async Task Custom_publisher_instance_is_used()
    {
        var publisher = new CountingPublisher();
        await using var provider = BuildProvider(c => c.NotificationPublisher = publisher);
        var mediator = provider.GetRequiredService<IPublisher>();

        await mediator.Publish(new Pinged());

        Assert.Equal(1, publisher.CallCount);
    }

    [Fact]
    public async Task Publish_null_notification_throws()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IPublisher>();

        await Assert.ThrowsAsync<ArgumentNullException>(() => mediator.Publish((Pinged)null!).AsTask());
    }

    private sealed class CountingPublisher : INotificationPublisher
    {
        public int CallCount { get; private set; }

        public async ValueTask Publish<TNotification>(IEnumerable<NotificationHandlerExecutor> handlerExecutors,
            TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            CallCount++;
            foreach (var executor in handlerExecutors)
            {
                await executor.HandlerCallback(executor.HandlerInstance, notification, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
