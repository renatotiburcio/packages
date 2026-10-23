namespace XForge.Mediator.Pipeline;

/// <summary>Default implementation of <see cref="IRequestExceptionHandlerState{TResponse}"/>.</summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class RequestExceptionHandlerState<TResponse> : IRequestExceptionHandlerState<TResponse>
{
    /// <inheritdoc />
    public bool IsHandled { get; private set; }

    /// <inheritdoc />
    public TResponse Response { get; private set; } = default!;

    /// <inheritdoc />
    public void SetHandled(TResponse response)
    {
        IsHandled = true;
        Response = response;
    }
}
