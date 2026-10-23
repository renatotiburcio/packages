using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mediator.Tests;

public class RegistrationAndLifetimeTests
{
    [Fact]
    public void Mediator_interfaces_resolve_to_same_instance_within_scope_and_singleton_when_configured()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.Lifetime = ServiceLifetime.Singleton; // opt-in para máxima performance
            })
            .BuildServiceProvider();
        using var scope = services.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        Assert.Same(mediator, scope.ServiceProvider.GetRequiredService<ISender>());
        Assert.Same(mediator, scope.ServiceProvider.GetRequiredService<IPublisher>());
        Assert.Same(mediator, services.GetRequiredService<IMediator>()); // singleton atravessa scopes
    }

    [Fact]
    public async Task Mediator_resolves_handlers_from_the_scope_it_was_resolved_from_by_default()
    {
        var services = new ServiceCollection()
            .AddScoped<IRequestHandler<ScopedCountRequest>, ScopedCountingHandler>()
            .AddXForgeMediator() // Lifetime padrão = Transient → mediator por escopo
            .BuildServiceProvider();
        await using var provider = services;

        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        // mediator transient: instâncias diferentes por resolução, cada uma resolve handlers do próprio scope
        ScopedCountingHandler.Instances = 0;
        await scope1.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());
        await scope1.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());
        await scope2.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());

        Assert.NotSame(
            scope1.ServiceProvider.GetRequiredService<IMediator>(),
            scope1.ServiceProvider.GetRequiredService<IMediator>());
        Assert.Equal(2, ScopedCountingHandler.Instances); // 1 no scope1 + 1 no scope2
    }

    [Fact]
    public async Task Transient_handlers_get_new_instance_per_send()
    {
        var services = new ServiceCollection()
            .AddTransient<IRequestHandler<CountRequest>, InstanceCountingHandler>()
            .AddXForgeMediator()
            .BuildServiceProvider();
        await using var provider = services;

        InstanceCountingHandler.Instances = 0;
        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new CountRequest());
        await mediator.Send(new CountRequest());

        Assert.Equal(2, InstanceCountingHandler.Instances);
    }

    [Fact]
    public async Task Scoped_lifetime_honored_for_handlers()
    {
        var services = new ServiceCollection()
            .AddScoped<IRequestHandler<ScopedCountRequest>, ScopedCountingHandler>()
            .AddXForgeMediator()
            .BuildServiceProvider();
        await using var provider = services;

        ScopedCountingHandler.Instances = 0;
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        await scope1.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());
        await scope1.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());
        await scope2.ServiceProvider.GetRequiredService<IMediator>().Send(new ScopedCountRequest());

        Assert.Equal(2, ScopedCountingHandler.Instances); // 1 no scope1 + 1 no scope2
    }

    [Fact]
    public async Task Open_generic_handler_registered_when_opted_in()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining(typeof(GenericHandler<,>));
                c.RegisterGenericHandlers = true;
            })
            .BuildServiceProvider();
        await using var provider = services;

        var result = await provider.GetRequiredService<IMediator>().Send(new GenericRequest<string> { Value = "ok" });

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Open_generic_handler_ignored_by_default()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c => c.RegisterServicesFromAssemblyContaining(typeof(GenericHandler<,>)))
            .BuildServiceProvider();
        await using var provider = services;

        await Assert.ThrowsAsync<HandlerNotFoundException>(() =>
            provider.GetRequiredService<IMediator>().Send(new GenericRequest<string>()).AsTask());
    }

    [Fact]
    public async Task TypeEvaluator_filters_scanned_types()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.TypeEvaluator = t => t != typeof(PingHandler);
            })
            .BuildServiceProvider();

        var mediator = services.GetRequiredService<IMediator>();
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => mediator.Send(new Ping()).AsTask());
    }

    // ---------- fixtures locais ----------

    public sealed class CountRequest : IRequest<Unit>
    {
    }

    [XForgeMediatorIgnore]
    public sealed class InstanceCountingHandler : IRequestHandler<CountRequest>
    {
        public static int Instances;

        public InstanceCountingHandler()
        {
            Instances++;
        }

        public ValueTask<Unit> Handle(CountRequest request, CancellationToken cancellationToken)
            => Unit.CompletedTask;
    }

    public sealed class ScopedCountRequest : IRequest<Unit>
    {
    }

    [XForgeMediatorIgnore]
    public sealed class ScopedCountingHandler : IRequestHandler<ScopedCountRequest>
    {
        public static int Instances;

        public ScopedCountingHandler()
        {
            Instances++;
        }

        public ValueTask<Unit> Handle(ScopedCountRequest request, CancellationToken cancellationToken)
            => Unit.CompletedTask;
    }

    public class GenericRequest<T> : IRequest<T>
    {
        public T? Value { get; init; }
    }

    public sealed class GenericHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : GenericRequest<TResponse>
    {
        public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            => ValueTask.FromResult(request.Value!);
    }
}
