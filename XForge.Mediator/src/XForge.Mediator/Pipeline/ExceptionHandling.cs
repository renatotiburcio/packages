namespace XForge.Mediator.Pipeline;

/// <summary>
/// Tracks whether an exception was handled and the replacement response for the request.
/// Passed to <see cref="IRequestExceptionHandler{TRequest, TResponse, TException}.Handle"/>.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequestExceptionHandlerState<TResponse>
{
    /// <summary>Gets a value indicating whether the exception was handled.</summary>
    bool IsHandled { get; }

    /// <summary>Gets the response that replaced the exception (when <see cref="IsHandled"/> is <see langword="true"/>).</summary>
    TResponse Response { get; }

    /// <summary>Marks the exception as handled with the supplied response.</summary>
    /// <param name="response">The response to return from the pipeline.</param>
    void SetHandled(TResponse response);
}

/// <summary>Handles an exception thrown by a request handler (or inner pipeline) and produces a recovery response.</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="TException">The exception type (matches the exception type hierarchy).</typeparam>
public interface IRequestExceptionHandler<in TRequest, TResponse, in TException>
    where TException : Exception
{
    /// <summary>Handles the exception.</summary>
    /// <param name="request">The request.</param>
    /// <param name="exception">The thrown exception.</param>
    /// <param name="state">State that receives the recovery response via <see cref="IRequestExceptionHandlerState{TResponse}.SetHandled"/>.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when handling finishes.</returns>
    ValueTask Handle(TRequest request, TException exception, IRequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken);
}

/// <summary>Reacts to an exception thrown by a request handler (side effects such as logging/metrics). Cannot recover.</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TException">The exception type (matches the exception type hierarchy).</typeparam>
public interface IRequestExceptionAction<in TRequest, in TException>
    where TException : Exception
{
    /// <summary>Processes the exception.</summary>
    /// <param name="request">The request.</param>
    /// <param name="exception">The thrown exception.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    ValueTask Process(TRequest request, TException exception, CancellationToken cancellationToken);
}
