using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mediator.Tests;

public class SendTests
{
    private static ServiceProvider BuildProvider(Action<XForgeMediatorServiceConfiguration>? cfg = null)
        => new ServiceCollection().AddXForgeMediator(c =>
        {
            c.RegisterServicesFromAssemblyContaining<Ping>();
            cfg?.Invoke(c);
        }).BuildServiceProvider();

    [Fact]
    public async Task Send_typed_request_returns_response()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var pong = await mediator.Send(new Ping());

        Assert.Equal("ping-pong", pong.Message);
    }

    [Fact]
    public async Task Send_unit_command_via_convenience_handler()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var log = new List<string>();

        await mediator.Send(new Reset { Log = log });

        Assert.Equal(new[] { "reset" }, log);
    }

    [Fact]
    public async Task Send_query_marker()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetUser { Id = 42 });

        Assert.Equal("user-42", result);
    }

    [Fact]
    public async Task Send_object_discovers_response_from_runtime_type()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();
        object request = new Ping();

        var response = await mediator.Send(request);

        var pong = Assert.IsType<Pong>(response);
        Assert.Equal("ping-pong", pong.Message);
    }

    [Fact]
    public async Task Send_object_with_explicit_response_type()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();

        var pong = await mediator.Send<Pong>(new Ping());

        Assert.Equal("ping-pong", pong.Message);
    }

    [Fact]
    public async Task Send_dispatches_by_runtime_type_for_derived_requests()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        Assert.Equal("derived", await mediator.Send((IRequest<string>)new DerivedNotification()));
        Assert.Equal("derived", await mediator.Send((object)new DerivedNotification())); // descoberta por runtime type
    }

    [Fact]
    public async Task Send_object_for_base_typed_reference_uses_runtime_type()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<ISender>();
        object request = new DerivedNotification();

        Assert.Equal("derived", await mediator.Send(request));
    }

    [Fact]
    public async Task Send_without_handler_throws_handler_not_found()
    {
        var services = new ServiceCollection().AddXForgeMediator().BuildServiceProvider();
        await using var provider = services;
        var mediator = provider.GetRequiredService<IMediator>();

        var exception = await Assert.ThrowsAsync<HandlerNotFoundException>(() => mediator.Send(new Ping()).AsTask());

        Assert.Equal(typeof(Ping), exception.RequestType);
        Assert.Equal(typeof(Pong), exception.ResponseType);
    }

    [Fact]
    public async Task Send_object_without_response_interface_throws()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send("not a request").AsTask());
    }

    [Fact]
    public async Task Send_with_mismatched_response_type_throws()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send<string>(new Ping()).AsTask());
    }

    [Fact]
    public async Task Send_null_request_throws()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ArgumentNullException>(() => mediator.Send((IRequest<Pong>)null!).AsTask());
    }

    [Fact]
    public async Task Cancellation_token_is_forwarded_to_handler()
    {
        var services = new ServiceCollection()
            .AddSingleton<CapturedToken>()
            .AddTransient<IRequestHandler<TokenPing, Pong>, CapturingHandler>()
            .AddXForgeMediator()
            .BuildServiceProvider();
        await using var provider = services;

        using var cts = new CancellationTokenSource();
        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new TokenPing(), cts.Token);

        Assert.Equal(cts.Token, provider.GetRequiredService<CapturedToken>().Token);
    }

    public sealed class TokenPing : IRequest<Pong>
    {
    }

    private sealed class CapturedToken
    {
        public CancellationToken Token { get; set; }
    }

    [XForgeMediatorIgnore]
    private sealed class CapturingHandler : IRequestHandler<TokenPing, Pong>
    {
        private readonly CapturedToken _captured;

        public CapturingHandler(CapturedToken captured) => _captured = captured;

        public ValueTask<Pong> Handle(TokenPing request, CancellationToken cancellationToken)
        {
            _captured.Token = cancellationToken;
            return ValueTask.FromResult(new Pong("ok"));
        }
    }
}
