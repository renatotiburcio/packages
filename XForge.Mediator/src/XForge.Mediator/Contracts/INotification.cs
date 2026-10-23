namespace XForge.Mediator;

/// <summary>
/// Marker for messages dispatched by <c>Publish</c> to all registered handlers.
/// </summary>
/// <remarks>
/// Notifications are multi-cast: every registered <see cref="INotificationHandler{TNotification}"/> receives
/// the same instance. Publishing a notification without handlers is a no-op.
/// </remarks>
public interface INotification
{
}
