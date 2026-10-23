namespace XForge.Mediator;

/// <summary>
/// Delegate that invokes the next step in the request pipeline (the next behavior or the handler itself).
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="cancellationToken">A cancellation token.</param>
/// <returns>The response produced by the rest of the pipeline.</returns>
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

/// <summary>
/// Delegate that invokes the next step in a streaming pipeline.
/// The cancellation token is captured when the pipeline is built.
/// </summary>
/// <typeparam name="TResponse">The streamed item type.</typeparam>
/// <returns>The response stream produced by the rest of the pipeline.</returns>
public delegate IAsyncEnumerable<TResponse> StreamHandlerDelegate<TResponse>();

/// <summary>
/// Wraps a request handler to run pipeline behaviors around it. Behaviors execute in DI registration order
/// (the first registered is the outermost).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>Pipeline handler. Implement this to add cross-cutting behavior around the rest of the pipeline.</summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The rest of the pipeline (next behavior or the handler).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response.</returns>
    ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>
/// Wraps a streaming request handler to run pipeline behaviors around it.
/// </summary>
/// <typeparam name="TRequest">The stream request type.</typeparam>
/// <typeparam name="TResponse">The streamed item type.</typeparam>
public interface IStreamPipelineBehavior<in TRequest, TResponse> where TRequest : IStreamRequest<TResponse>
{
    /// <summary>Pipeline handler. Implement this to add cross-cutting behavior around the rest of the pipeline.</summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The rest of the pipeline (next behavior or the handler).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response stream.</returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
