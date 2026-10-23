using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Tests;

public class ExceptionTests
{
    private static ServiceProvider BuildProvider(Action<XForgeMediatorServiceConfiguration>? cfg = null)
        => new ServiceCollection().AddXForgeMediator(c =>
        {
            c.RegisterServicesFromAssemblyContaining<Boom>();
            cfg?.Invoke(c);
        }).BuildServiceProvider();

    [Fact]
    public async Task Exception_handler_recovers_with_response()
    {
        await using var provider = BuildProvider();

        var result = await provider.GetRequiredService<IMediator>().Send(new Boom());

        Assert.Equal("recovered", result);
    }

    [Fact]
    public async Task Exception_action_runs_before_recovery()
    {
        var action = new BoomAction { Seen = new List<string>() };
        var services = new ServiceCollection()
            .AddSingleton<IRequestExceptionAction<Boom, BoomException>>(action)
            .AddXForgeMediator(c => c.RegisterServicesFromAssemblyContaining<Boom>())
            .BuildServiceProvider();
        await using var provider = services;

        var result = await provider.GetRequiredService<IMediator>().Send(new Boom());

        Assert.Equal("recovered", result);
        Assert.Equal(new[] { "boom" }, action.Seen);
    }

    [Fact]
    public async Task Unhandled_exception_is_rethrown()
    {
        var services = new ServiceCollection()
            .AddTransient<IRequestHandler<Boom, string>, BoomHandler>()
            .AddXForgeMediator() // nenhum exception handler registrado
            .BuildServiceProvider();
        await using var provider = services;

        var exception = await Assert.ThrowsAsync<BoomException>(() =>
            provider.GetRequiredService<IMediator>().Send(new Boom()).AsTask());

        Assert.Equal("boom", exception.Message);
    }

    [Fact]
    public async Task Handler_for_base_exception_catches_derived()
    {
        var services = new ServiceCollection()
            .AddTransient<IRequestHandler<Boom, string>, BoomHandler>()
            .AddSingleton<IRequestExceptionHandler<Boom, string, Exception>>(new AnyExceptionRecovery())
            .AddXForgeMediator() // sem scan: garante que só o handler de Exception base existe
            .BuildServiceProvider();
        await using var provider = services;

        var result = await provider.GetRequiredService<IMediator>().Send(new Boom());

        Assert.Equal("recovered-base", result);
    }

    [Fact]
    public async Task Exception_inside_user_behavior_is_processed()
    {
        var services = new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<Boom>();
                c.AddBehavior(typeof(OuterBehavior<,>));
            })
            .AddSingleton<OrderTracker>()
            .BuildServiceProvider();
        await using var provider = services;

        var result = await provider.GetRequiredService<IMediator>().Send(new Boom());

        // exception behavior é interno aos behaviors do usuário: recupera antes de voltar ao outer
        Assert.Equal("recovered", result);
    }

    [Fact]
    public async Task Send_object_also_recovers_from_exceptions()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();
        object request = new Boom();

        Assert.Equal("recovered", await mediator.Send(request));
    }
}
