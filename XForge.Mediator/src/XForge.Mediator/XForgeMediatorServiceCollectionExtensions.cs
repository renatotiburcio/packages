using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator.Internal;

namespace XForge.Mediator;

/// <summary>
/// Registration extensions for Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class XForgeMediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IMediator"/>, <see cref="ISender"/> and <see cref="IPublisher"/> with the default configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddXForgeMediator(this IServiceCollection services)
    {
        return AddXForgeMediator(services, (Action<XForgeMediatorServiceConfiguration>?)null);
    }

    /// <summary>
    /// Registers <see cref="IMediator"/>, <see cref="ISender"/> and <see cref="IPublisher"/> and scans the
    /// assemblies supplied via <paramref name="configuration"/> for handlers, behaviors and processors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The registration options.</param>
    /// <returns>The service collection.</returns>
    /// <example>
    /// <code>
    /// services.AddXForgeMediator(cfg =>
    /// {
    ///     cfg.RegisterServicesFromAssemblyContaining&lt;Ping&gt;();
    ///     cfg.AddOpenBehavior(typeof(ValidationBehavior&lt;,&gt;));
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddXForgeMediator(this IServiceCollection services, XForgeMediatorServiceConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        XForgeMediatorDiagnostics.Enabled = configuration.EnableDiagnostics;
        ServiceRegistrar.RegisterMediatorServices(services, configuration);
        return services;
    }

    /// <summary>
    /// Registers <see cref="IMediator"/>, <see cref="ISender"/> and <see cref="IPublisher"/> and scans the
    /// assemblies supplied in <paramref name="configure"/> for handlers, behaviors and processors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The registration options configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddXForgeMediator(this IServiceCollection services, Action<XForgeMediatorServiceConfiguration>? configure)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var configuration = new XForgeMediatorServiceConfiguration();
        configure?.Invoke(configuration);
        return AddXForgeMediator(services, configuration);
    }
}
