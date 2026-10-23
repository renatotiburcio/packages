namespace XForge.Mediator.Pipeline;

/// <summary>
/// Runs registered pre-processors for a request, immediately before the handler.
/// Auto-registered by <c>AddXForgeMediator</c> when any <see cref="IRequestPreProcessor{TRequest}"/> is found
/// (or can be registered manually with <c>AddOpenRequestPreProcessor</c>).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class RequestPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _processors;

    /// <summary>Creates the behavior.</summary>
    /// <param name="processors">The registered pre-processors (resolved from DI).</param>
    public RequestPreProcessorBehavior(IEnumerable<IRequestPreProcessor<TRequest>> processors)
    {
        _processors = processors;
    }

    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.Process(request, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>Processes a request before the handler runs.</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public interface IRequestPreProcessor<in TRequest>
{
    /// <summary>Processes the request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    ValueTask Process(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Runs registered post-processors for a request, after the pipeline has produced a response
/// (outermost built-in behavior: it also observes responses recovered by exception handlers).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class RequestPostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _processors;

    /// <summary>Creates the behavior.</summary>
    /// <param name="processors">The registered post-processors (resolved from DI).</param>
    public RequestPostProcessorBehavior(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> processors)
    {
        _processors = processors;
    }

    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var processor in _processors)
        {
            await processor.Process(request, response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}

/// <summary>Processes a request and its response after the pipeline completes.</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    /// <summary>Processes the request/response pair.</summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response produced by the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    ValueTask Process(TRequest request, TResponse response, CancellationToken cancellationToken);
}
