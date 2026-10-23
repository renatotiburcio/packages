using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Tests;

public class BehaviorTests
{
    [Fact]
    public async Task Behaviors_execute_in_registration_order_outermost_first()
    {
        var services = new ServiceCollection()
            .AddSingleton<OrderTracker>()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.AddBehavior(typeof(OuterBehavior<,>));   // registrado primeiro → mais externo
                c.AddBehavior(typeof(InnerBehavior<,>));
            })
            .BuildServiceProvider();
        await using var provider = services;

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new Ping());

        var steps = provider.GetRequiredService<OrderTracker>().Steps;
        // usuários (outer, inner) → post-processor → pre-processor → handler
        Assert.Equal(new[] { "outer-pre", "inner-pre", "pre", "post", "inner-post", "outer-post" }, steps);
    }

    [Fact]
    public async Task Generic_behavior_helpers_work()
    {
        var services = new ServiceCollection()
            .AddSingleton<OrderTracker>()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.AddOpenBehavior(typeof(OuterBehavior<,>));
            })
            .BuildServiceProvider();
        await using var provider = services;

        var pong = await provider.GetRequiredService<IMediator>().Send(new Ping());

        Assert.Equal("ping-pong", pong.Message);
        Assert.Contains("outer-pre", provider.GetRequiredService<OrderTracker>().Steps);
    }

    [Fact]
    public async Task Closed_behavior_applies_only_to_its_request_type()
    {
        var services = new ServiceCollection()
            .AddSingleton<OrderTracker>()
            .AddTransient(typeof(IPipelineBehavior<Ping, Pong>), typeof(OuterBehavior<Ping, Pong>))
            .AddXForgeMediator(c => c.RegisterServicesFromAssemblyContaining<Ping>())
            .BuildServiceProvider();
        await using var provider = services;
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.Send(new Ping());
        await mediator.Send(new GetUser { Id = 1 }); // não deve passar pelo behavior de Ping

        // Ping tem pre/post processors auto-registrados (mesmo assembly) + o behavior fechado do usuário
        Assert.Equal(new[] { "outer-pre", "pre", "post", "outer-post" }, provider.GetRequiredService<OrderTracker>().Steps);
    }

    [Fact]
    public async Task Behavior_can_short_circuit_and_skip_handler()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.AddBehavior(typeof(ShortCircuitBehavior<,>));
            })
            .BuildServiceProvider();
        await using var provider = services;

        var pong = await provider.GetRequiredService<IMediator>().Send(new Ping());

        Assert.Null(pong); // handler nunca executou → default(Pong) = null
    }

    [Fact]
    public async Task Behaviors_without_handlers_still_throw_handler_not_found()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c => c.AddBehavior(typeof(OuterBehavior<,>)))
            .BuildServiceProvider();
        await using var provider = services;

        await Assert.ThrowsAsync<HandlerNotFoundException>(() => provider.GetRequiredService<IMediator>().Send(new Ping()).AsTask());
    }

    [Fact]
    public async Task Pre_and_post_processors_auto_register_and_run_in_order()
    {
        var services = new ServiceCollection()
            .AddSingleton<OrderTracker>()
            .AddXForgeMediator(c => c.RegisterServicesFromAssemblyContaining<Ping>())
            .BuildServiceProvider();
        await using var provider = services;

        var pong = await provider.GetRequiredService<IMediator>().Send(new Ping());

        Assert.Equal("ping-pong", pong.Message);
        // pre-processors rodam antes do handler; post-processors depois
        Assert.Equal(new[] { "pre", "post" }, provider.GetRequiredService<OrderTracker>().Steps);
    }

    [Fact]
    public async Task Pipeline_has_no_behaviors_when_no_processors_exist()
    {
        var services = new ServiceCollection()
            .AddTransient<IRequestHandler<GetUser, string>, GetUserHandler>()
            .AddXForgeMediator()
            .BuildServiceProvider();
        await using var provider = services;

        // sem nenhum processor registrado, pre/post não entram no container e o fast path fica limpo;
        // o processamento de exceções é integrado ao wrapper (não é um behavior no container)
        Assert.Empty(provider.GetServices<IPipelineBehavior<GetUser, string>>());

        var result = await provider.GetRequiredService<IMediator>().Send(new GetUser { Id = 7 });
        Assert.Equal("user-7", result);
    }

    [Fact]
    public async Task Manual_processor_registration_via_options()
    {
        var services = new ServiceCollection()
            .AddSingleton<OrderTracker>()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.AddRequestPreProcessor(typeof(PingPreProcessor));
                c.AddRequestPostProcessor(typeof(PingPostProcessor));
            })
            .BuildServiceProvider();
        await using var provider = services;

        var pong = await provider.GetRequiredService<IMediator>().Send(new Ping());

        Assert.Equal("ping-pong", pong.Message);
        Assert.Contains("pre", provider.GetRequiredService<OrderTracker>().Steps);
    }

    [Fact]
    public async Task Post_processor_sees_handler_response()
    {
        Pong? seen = null;
        var services = new ServiceCollection()
            .AddTransient<IRequestPostProcessor<Ping, Pong>>(_ => new CapturePostProcessor(r => seen = r))
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Ping>();
                c.AutoRegisterRequestProcessors = true;
            })
            .BuildServiceProvider();
        await using var provider = services;

        await provider.GetRequiredService<IMediator>().Send(new Ping());

        Assert.Equal("ping-pong", seen?.Message);
    }

    [XForgeMediatorIgnore]
    private sealed class CapturePostProcessor : IRequestPostProcessor<Ping, Pong>
    {
        private readonly Action<Pong> _capture;

        public CapturePostProcessor(Action<Pong> capture) => _capture = capture;

        public ValueTask Process(Ping request, Pong response, CancellationToken cancellationToken)
        {
            _capture(response);
            return ValueTask.CompletedTask;
        }
    }
}
