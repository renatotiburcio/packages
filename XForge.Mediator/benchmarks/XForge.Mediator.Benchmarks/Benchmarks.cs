using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ar = Arbiter.Mediation;
using Md = Mediator;
using Mr = MediatR;
using Xf = XForge.Mediator;

namespace XForge.Mediator.Benchmarks;

// Todos os cenários usam ServiceLifetime.Singleton (a configuração recomendada/mais rápida de cada biblioteca)
// e handlers que completam sincronamente, medindo o overhead puro de dispatch.

/// <summary>Registro compartilhado entre os cenários de benchmark.</summary>
internal static class BenchSetup
{
    public static ServiceProvider BuildXForge(bool withBehavior = false)
    {
        return new ServiceCollection()
            .AddXForgeMediator(c =>
            {
                c.RegisterServicesFromAssemblyContaining<XfPing>();
                c.Lifetime = ServiceLifetime.Singleton;
                if (withBehavior)
                {
                    c.AddOpenBehavior(typeof(XfBehavior<,>));
                }
            })
            .BuildServiceProvider();
    }

    public static ServiceProvider BuildMediatR(bool withBehavior = false)
    {
        return new ServiceCollection()
            .AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblyContaining<MrPing>();
                c.Lifetime = ServiceLifetime.Singleton;
                if (withBehavior)
                {
                    c.AddOpenBehavior(typeof(MrBehavior<,>));
                }
            })
            .BuildServiceProvider();
    }

    public static ServiceProvider BuildMediatorSourceGen()
    {
        // O gerador parseia a lambda em compile-time: nada de lógica condicional aqui
        return new ServiceCollection().AddMediator().BuildServiceProvider();
    }

    public static ServiceProvider BuildMediatorSourceGenWithBehavior()
    {
        return new ServiceCollection().AddMediator(o => o.PipelineBehaviors = [typeof(MdBehavior<,>)]).BuildServiceProvider();
    }

    public static ServiceProvider BuildArbiter(bool withBehavior = false)
    {
        var services = new ServiceCollection()
            .AddSingleton<Ar.IMediator, Ar.Mediator>()
            .AddSingleton(typeof(Ar.IRequestHandler<ArPing, ArPong>), typeof(ArPingHandler))
            .AddSingleton(typeof(Ar.IRequestHandler<ArReset, Ar.Unit>), typeof(ArResetHandler));

        for (var i = 0; i < 8; i++)
        {
            services.AddSingleton<Ar.INotificationHandler<ArPinged>, ArPingedHandler>();
        }

        if (withBehavior)
        {
            services.AddSingleton(typeof(Ar.IPipelineBehavior<,>), typeof(ArBehavior<,>));
        }

        return services.BuildServiceProvider();
    }

    public static ServiceProvider BuildXForgePublishers(int handlerCount, Xf.INotificationPublisher? publisher = null)
    {
        var services = new ServiceCollection().AddXForgeMediator(c =>
        {
            c.Lifetime = ServiceLifetime.Singleton;
            c.NotificationPublisher = publisher ?? new Xf.TaskWhenAllPublisher();
        });

        for (var i = 0; i < handlerCount; i++)
        {
            services.AddSingleton<Xf.INotificationHandler<XfPinged>, XfPingedHandler>();
        }

        return services.BuildServiceProvider();
    }

    public static ServiceProvider BuildMediatRPublishers(int handlerCount)
    {
        var services = new ServiceCollection().AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<MrPinged>();
            c.Lifetime = ServiceLifetime.Singleton;
        });

        // O scan já registra a classe de handler: remove e adiciona N instâncias
        services.RemoveAll<Mr.INotificationHandler<MrPinged>>();
        for (var i = 0; i < handlerCount; i++)
        {
            services.AddSingleton<Mr.INotificationHandler<MrPinged>, MrPingedHandler>();
        }

        return services.BuildServiceProvider();
    }

    public static ServiceProvider BuildMediatorSourceGenPublishers(int handlerCount)
    {
        var services = new ServiceCollection().AddMediator();
        // O gerador já registra a classe de handler: remove e adiciona N instâncias
        services.RemoveAll<Md.INotificationHandler<MdPinged>>();
        for (var i = 0; i < handlerCount; i++)
        {
            services.AddSingleton<Md.INotificationHandler<MdPinged>, MdPingedHandler>();
        }

        return services.BuildServiceProvider();
    }

    public static ServiceProvider BuildArbiterPublishers(int handlerCount)
    {
        var services = new ServiceCollection().AddSingleton<Ar.IMediator, Ar.Mediator>();

        for (var i = 0; i < handlerCount; i++)
        {
            services.AddSingleton<Ar.INotificationHandler<ArPinged>, ArPingedHandler>();
        }

        return services.BuildServiceProvider();
    }
}

[MemoryDiagnoser]
public class SendRequestBenchmarks
{
    private ServiceProvider _spXf = null!, _spMr = null!, _spMd = null!, _spAr = null!;
    private Xf.IMediator _xf = null!;
    private Mr.IMediator _mr = null!;
    private Md.IMediator _md = null!;
    private Ar.IMediator _ar = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf = BenchSetup.BuildXForge();
        _spMr = BenchSetup.BuildMediatR();
        _spMd = BenchSetup.BuildMediatorSourceGen();
        _spAr = BenchSetup.BuildArbiter();
        _xf = _spXf.GetRequiredService<Xf.IMediator>();
        _mr = _spMr.GetRequiredService<Mr.IMediator>();
        _md = _spMd.GetRequiredService<Md.IMediator>();
        _ar = _spAr.GetRequiredService<Ar.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf.Dispose();
        _spMr.Dispose();
        _spMd.Dispose();
        _spAr.Dispose();
    }

    [Benchmark]
    public ValueTask<XfPong> XForge() => _xf.Send(new XfPing("hello"));

    [Benchmark(Baseline = true)]
    public Task<MrPong> MediatR() => _mr.Send(new MrPing("hello"));

    [Benchmark]
    public ValueTask<MdPong> Mediator_SourceGen() => _md.Send(new MdPing("hello"));

    [Benchmark]
    public ValueTask<ArPong?> Arbiter() => _ar.Send(new ArPing("hello"));
}

[MemoryDiagnoser]
public class SendCommandBenchmarks
{
    private ServiceProvider _spXf = null!, _spMr = null!, _spMd = null!, _spAr = null!;
    private Xf.IMediator _xf = null!;
    private Mr.IMediator _mr = null!;
    private Md.IMediator _md = null!;
    private Ar.IMediator _ar = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf = BenchSetup.BuildXForge();
        _spMr = BenchSetup.BuildMediatR();
        _spMd = BenchSetup.BuildMediatorSourceGen();
        _spAr = BenchSetup.BuildArbiter();
        _xf = _spXf.GetRequiredService<Xf.IMediator>();
        _mr = _spMr.GetRequiredService<Mr.IMediator>();
        _md = _spMd.GetRequiredService<Md.IMediator>();
        _ar = _spAr.GetRequiredService<Ar.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf.Dispose();
        _spMr.Dispose();
        _spMd.Dispose();
        _spAr.Dispose();
    }

    [Benchmark]
    public ValueTask<Xf.Unit> XForge() => _xf.Send(new XfReset());

    [Benchmark(Baseline = true)]
    public Task MediatR() => _mr.Send(new MrReset());

    [Benchmark]
    public ValueTask<Md.Unit> Mediator_SourceGen() => _md.Send(new MdReset());

    [Benchmark]
    public ValueTask<Ar.Unit> Arbiter() => _ar.Send(new ArReset());
}

[MemoryDiagnoser]
public class SendObjectBenchmarks
{
    private ServiceProvider _spXf = null!, _spMr = null!, _spMd = null!, _spAr = null!;
    private Xf.ISender _xf = null!;
    private Mr.ISender _mr = null!;
    private Md.ISender _md = null!;
    private Ar.IMediator _ar = null!;
    private object _xfRequest = null!, _mrRequest = null!, _mdRequest = null!, _arRequest = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf = BenchSetup.BuildXForge();
        _spMr = BenchSetup.BuildMediatR();
        _spMd = BenchSetup.BuildMediatorSourceGen();
        _spAr = BenchSetup.BuildArbiter();
        _xf = _spXf.GetRequiredService<Xf.ISender>();
        _mr = _spMr.GetRequiredService<Mr.ISender>();
        _md = _spMd.GetRequiredService<Md.ISender>();
        _ar = _spAr.GetRequiredService<Ar.IMediator>();
        _xfRequest = new XfPing("hello");
        _mrRequest = new MrPing("hello");
        _mdRequest = new MdPing("hello");
        _arRequest = new ArPing("hello");

        // Aquece o cache de dispatch por tipo (1ª chamada usa reflection)
        _xf.Send(_xfRequest).AsTask().GetAwaiter().GetResult();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf.Dispose();
        _spMr.Dispose();
        _spMd.Dispose();
        _spAr.Dispose();
    }

    [Benchmark]
    public ValueTask<object?> XForge() => _xf.Send(_xfRequest);

    [Benchmark(Baseline = true)]
    public Task<object?> MediatR() => _mr.Send(_mrRequest);

    [Benchmark]
    public ValueTask<object?> Mediator_SourceGen() => _md.Send(_mdRequest);

    [Benchmark]
    public ValueTask<object?> Arbiter() => _ar.Send(_arRequest);
}

[MemoryDiagnoser]
public class SendWithBehaviorBenchmarks
{
    private ServiceProvider _spXf = null!, _spMr = null!, _spMd = null!, _spAr = null!;
    private Xf.IMediator _xf = null!;
    private Mr.IMediator _mr = null!;
    private Md.IMediator _md = null!;
    private Ar.IMediator _ar = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf = BenchSetup.BuildXForge(withBehavior: true);
        _spMr = BenchSetup.BuildMediatR(withBehavior: true);
        _spMd = BenchSetup.BuildMediatorSourceGenWithBehavior();
        _spAr = BenchSetup.BuildArbiter(withBehavior: true);
        _xf = _spXf.GetRequiredService<Xf.IMediator>();
        _mr = _spMr.GetRequiredService<Mr.IMediator>();
        _md = _spMd.GetRequiredService<Md.IMediator>();
        _ar = _spAr.GetRequiredService<Ar.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf.Dispose();
        _spMr.Dispose();
        _spMd.Dispose();
        _spAr.Dispose();
    }

    [Benchmark]
    public ValueTask<XfPong> XForge() => _xf.Send(new XfPing("hello"));

    [Benchmark(Baseline = true)]
    public Task<MrPong> MediatR() => _mr.Send(new MrPing("hello"));

    [Benchmark]
    public ValueTask<MdPong> Mediator_SourceGen() => _md.Send(new MdPing("hello"));

    [Benchmark]
    public ValueTask<ArPong?> Arbiter() => _ar.Send(new ArPing("hello"));
}

[MemoryDiagnoser]
public class PublishNotificationBenchmarks
{
    private ServiceProvider _spXf2 = null!, _spMr2 = null!, _spMd2 = null!, _spAr2 = null!;
    private ServiceProvider _spXf8 = null!, _spMr8 = null!, _spMd8 = null!, _spAr8 = null!;
    private Xf.IPublisher _xf2 = null!, _xf8 = null!;
    private Mr.IPublisher _mr2 = null!, _mr8 = null!;
    private Md.IPublisher _md2 = null!, _md8 = null!;
    private Ar.IMediator _ar2 = null!, _ar8 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf2 = BenchSetup.BuildXForgePublishers(2);
        _spMr2 = BenchSetup.BuildMediatRPublishers(2);
        _spMd2 = BenchSetup.BuildMediatorSourceGenPublishers(2);
        _spAr2 = BenchSetup.BuildArbiterPublishers(2);
        _spXf8 = BenchSetup.BuildXForgePublishers(8);
        _spMr8 = BenchSetup.BuildMediatRPublishers(8);
        _spMd8 = BenchSetup.BuildMediatorSourceGenPublishers(8);
        _spAr8 = BenchSetup.BuildArbiterPublishers(8);
        _xf2 = _spXf2.GetRequiredService<Xf.IPublisher>();
        _mr2 = _spMr2.GetRequiredService<Mr.IPublisher>();
        _md2 = _spMd2.GetRequiredService<Md.IPublisher>();
        _ar2 = _spAr2.GetRequiredService<Ar.IMediator>();
        _xf8 = _spXf8.GetRequiredService<Xf.IPublisher>();
        _mr8 = _spMr8.GetRequiredService<Mr.IPublisher>();
        _md8 = _spMd8.GetRequiredService<Md.IPublisher>();
        _ar8 = _spAr8.GetRequiredService<Ar.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf2.Dispose();
        _spMr2.Dispose();
        _spMd2.Dispose();
        _spAr2.Dispose();
        _spXf8.Dispose();
        _spMr8.Dispose();
        _spMd8.Dispose();
        _spAr8.Dispose();
    }

    [Benchmark]
    public ValueTask XForge_2Handlers() => _xf2.Publish(new XfPinged());

    [Benchmark(Baseline = true)]
    public Task MediatR_2Handlers() => _mr2.Publish(new MrPinged());

    [Benchmark]
    public ValueTask Mediator_SourceGen_2Handlers() => _md2.Publish(new MdPinged());

    [Benchmark]
    public ValueTask Arbiter_2Handlers() => _ar2.Publish(new ArPinged());

    [Benchmark]
    public ValueTask XForge_8Handlers() => _xf8.Publish(new XfPinged());

    [Benchmark]
    public Task MediatR_8Handlers() => _mr8.Publish(new MrPinged());

    [Benchmark]
    public ValueTask Mediator_SourceGen_8Handlers() => _md8.Publish(new MdPinged());

    [Benchmark]
    public ValueTask Arbiter_8Handlers() => _ar8.Publish(new ArPinged());
}

[MemoryDiagnoser]
public class StreamRequestBenchmarks
{
    private ServiceProvider _spXf = null!, _spMr = null!, _spMd = null!;
    private Xf.ISender _xf = null!;
    private Mr.ISender _mr = null!;
    private Md.ISender _md = null!;

    [GlobalSetup]
    public void Setup()
    {
        _spXf = BenchSetup.BuildXForge();
        _spMr = BenchSetup.BuildMediatR();
        _spMd = BenchSetup.BuildMediatorSourceGen();
        _xf = _spXf.GetRequiredService<Xf.ISender>();
        _mr = _spMr.GetRequiredService<Mr.ISender>();
        _md = _spMd.GetRequiredService<Md.ISender>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _spXf.Dispose();
        _spMr.Dispose();
        _spMd.Dispose();
    }

    [Benchmark]
    public async ValueTask<int> XForge()
    {
        var sum = 0;
        await foreach (var item in _xf.CreateStream(new XfNumbers(8)))
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark(Baseline = true)]
    public async ValueTask<int> MediatR()
    {
        var sum = 0;
        await foreach (var item in _mr.CreateStream(new MrNumbers(8)))
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark]
    public async ValueTask<int> Mediator_SourceGen()
    {
        var sum = 0;
        await foreach (var item in _md.CreateStream(new MdNumbers(8)))
        {
            sum += item;
        }
        return sum;
    }
}

