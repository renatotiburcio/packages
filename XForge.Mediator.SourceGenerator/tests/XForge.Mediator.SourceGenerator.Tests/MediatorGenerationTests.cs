using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XForge.Mediator.SourceGenerator.Tests;

public class MediatorGenerationTests
{
    [Fact]
    public void Wrapper_registry_e_sender_sao_gerados()
    {
        var result = GeneratorTestHarness.Run("""
            using System.Threading;
            using System.Threading.Tasks;
            using XForge.Mediator;

            namespace TestApp;

            public record EchoRequest(string Mensagem) : IRequest<string>;

            public class EchoHandler : IRequestHandler<EchoRequest, string>
            {
                public ValueTask<string> Handle(EchoRequest request, CancellationToken cancellationToken)
                    => ValueTask.FromResult("eco:" + request.Mensagem);
            }
            """);

        Assert.Empty(result.CompileErrors);
        Assert.Contains(result.GeneratedFiles, file => file.Key.EndsWith(".g.cs"));
    }

    [Fact]
    public void Dispatch_gerado_executa_o_handler()
    {
        var result = GeneratorTestHarness.Run("""
            using System.Threading;
            using System.Threading.Tasks;
            using XForge.Mediator;

            namespace TestApp;

            public record EchoRequest(string Mensagem) : IRequest<string>;

            public class EchoHandler : IRequestHandler<EchoRequest, string>
            {
                public ValueTask<string> Handle(EchoRequest request, CancellationToken cancellationToken)
                    => ValueTask.FromResult("eco:" + request.Mensagem);
            }
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var request = Activator.CreateInstance(assembly.GetType("TestApp.EchoRequest")!, "ola")!;

        var send = SendMethod(assembly);
        var task = (ValueTask<string>)send.Invoke(CreateSender(assembly), new[] { request, CancellationToken.None })!;

        Assert.Equal("eco:ola", task.Result);
    }

    [Fact]
    public void Comportamento_unit_e_gerado_para_handler_de_request_sem_resposta()
    {
        var result = GeneratorTestHarness.Run("""
            using System.Threading;
            using System.Threading.Tasks;
            using XForge.Mediator;

            namespace TestApp;

            public record LogCommand(string Texto) : IRequest;

            public class LogHandler : IRequestHandler<LogCommand>
            {
                public ValueTask<Unit> Handle(LogCommand request, CancellationToken cancellationToken)
                    => ValueTask.FromResult(Unit.Value);
            }
            """);

        Assert.Empty(result.CompileErrors);

        var generated = string.Join("\n", result.GeneratedFiles.Select(file => file.Value));
        Assert.Contains("typeof(global::XForge.Mediator.Unit)", generated);
        Assert.Contains("LogHandler", generated);
    }

    [Fact]
    public void Handler_ignorado_nao_e_gerado()
    {
        var result = GeneratorTestHarness.Run("""
            using System.Threading;
            using System.Threading.Tasks;
            using XForge.Mediator;

            namespace TestApp;

            public record PingRequest : IRequest<string>;

            [XForgeMediatorIgnore]
            public class PingHandler : IRequestHandler<PingRequest, string>
            {
                public ValueTask<string> Handle(PingRequest request, CancellationToken cancellationToken)
                    => ValueTask.FromResult("pong");
            }
            """);

        Assert.Empty(result.CompileErrors);
        Assert.DoesNotContain(result.GeneratedFiles, file => file.Value.Contains("PingHandler"));
    }

    [Fact]
    public void Behaviors_sao_encadeados_na_ordem_do_DI()
    {
        var result = GeneratorTestHarness.Run("""
            using System.Threading;
            using System.Threading.Tasks;
            using XForge.Mediator;

            namespace TestApp;

            public record EchoRequest(string Mensagem) : IRequest<string>;

            public class EchoHandler : IRequestHandler<EchoRequest, string>
            {
                public ValueTask<string> Handle(EchoRequest request, CancellationToken cancellationToken)
                    => ValueTask.FromResult("base");
            }

            public class ComportamentoA : IPipelineBehavior<EchoRequest, string>
            {
                public ValueTask<string> Handle(EchoRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
                {
                    return Continue(next, cancellationToken);

                    static async ValueTask<string> Continue(RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
                    {
                        var valor = await next(cancellationToken);
                        return "A(" + valor + ")";
                    }
                }
            }
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var services = new ServiceCollection();
        AddGenerated(services, assembly);
        services.AddTransient(assembly.GetType("TestApp.EchoHandler")!);
        services.AddTransient(
            typeof(global::XForge.Mediator.IPipelineBehavior<,>).MakeGenericType(
                assembly.GetType("TestApp.EchoRequest")!,
                typeof(string)),
            assembly.GetType("TestApp.ComportamentoA")!);

        var provider = services.BuildServiceProvider();
        var request = Activator.CreateInstance(assembly.GetType("TestApp.EchoRequest")!, "x")!;

        var send = SendMethod(assembly);
        var sender = provider.GetRequiredService<global::XForge.Mediator.ISender>();
        var task = (ValueTask<string>)send.Invoke(sender, new[] { request, CancellationToken.None })!;

        Assert.Equal("A(base)", task.Result);
    }

    /// <summary>Seleciona o overload tipado Send&lt;TResponse&gt;(IRequest&lt;TResponse&gt;, CancellationToken).</summary>
    private static MethodInfo SendMethod(Assembly assembly)
    {
        return assembly.GetType("XForge.Mediator.Generated.XForgeGeneratedSender")!
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(method => method.Name == "Send" &&
                             method.IsGenericMethodDefinition &&
                             method.GetParameters()[0].ParameterType != typeof(object))
            .MakeGenericMethod(typeof(string));
    }

    private static object CreateSender(Assembly assembly)
    {
        var services = new ServiceCollection();
        AddGenerated(services, assembly);
        services.AddTransient(assembly.GetType("TestApp.EchoHandler")!);
        return services.BuildServiceProvider().GetRequiredService<global::XForge.Mediator.ISender>();
    }

    /// <summary>Invoca a extensão de DI gerada (existe apenas no assembly emitido).</summary>
    private static void AddGenerated(IServiceCollection services, Assembly assembly)
    {
        var extensionsType = assembly.GetType("XForge.Mediator.Generated.XForgeMediatorGeneratedExtensions")!;
        var method = extensionsType.GetMethod("AddXForgeMediatorGenerated", BindingFlags.Public | BindingFlags.Static)!;
        method.Invoke(null, new object[] { services });
    }
}

