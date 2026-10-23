using System.Reflection;

namespace XForge.Mediator.Internal;

/// <summary>Reflection helpers used by the dispatch engine (results are cached by callers).</summary>
internal static class RequestTypeHelper
{
    /// <summary>Gets the closed <c>IRequest&lt;TResponse&gt;</c> response type implemented by <paramref name="requestType"/>.</summary>
    /// <exception cref="InvalidOperationException">The type does not implement <see cref="IRequest{TResponse}"/>.</exception>
    internal static Type GetResponseType(Type requestType)
    {
        foreach (var implementedInterface in requestType.GetInterfaces())
        {
            if (implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == typeof(IRequest<>))
            {
                return implementedInterface.GetGenericArguments()[0];
            }
        }

        throw new InvalidOperationException(
            $"Type '{requestType}' does not implement IRequest<TResponse>. " +
            "Requests dispatched via Send(object) must implement IRequest<TResponse> (directly or via IRequest).");
    }

    /// <summary>Gets the closed <c>IStreamRequest&lt;TResponse&gt;</c> item type implemented by <paramref name="requestType"/>.</summary>
    /// <exception cref="InvalidOperationException">The type does not implement <see cref="IStreamRequest{TResponse}"/>.</exception>
    internal static Type GetStreamResponseType(Type requestType)
    {
        foreach (var implementedInterface in requestType.GetInterfaces())
        {
            if (implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == typeof(IStreamRequest<>))
            {
                return implementedInterface.GetGenericArguments()[0];
            }
        }

        throw new InvalidOperationException(
            $"Type '{requestType}' does not implement IStreamRequest<TResponse>. " +
            "Stream requests dispatched via CreateStream(object) must implement IStreamRequest<TResponse>.");
    }

    /// <summary>Loads all types of an assembly, tolerating unloadable types.</summary>
    internal static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(t => t is not null)!;
        }
    }
}
