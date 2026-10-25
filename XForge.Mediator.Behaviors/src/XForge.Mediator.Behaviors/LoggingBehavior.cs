using System.Diagnostics;
using XForge.Mediator;

namespace XForge.Mediator.Behaviors;

/// <summary>
/// Behavior de logging: registra inicio e conclusao do request com duracao.
/// Register: services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var start = System.Diagnostics.Stopwatch.GetTimestamp();

        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);

            var elapsed = (Stopwatch.GetTimestamp() - start) * 1000d / Stopwatch.Frequency;
            System.Console.WriteLine($"[XForge.Mediator] {requestName} OK em {elapsed:F1} ms");

            return response;
        }
        catch (Exception exception)
        {
            var elapsed = (Stopwatch.GetTimestamp() - start) * 1000d / Stopwatch.Frequency;
            System.Console.WriteLine($"[XForge.Mediator] {requestName} FALHOU em {elapsed:F1} ms: {exception.Message}");
            throw;
        }
    }
}
