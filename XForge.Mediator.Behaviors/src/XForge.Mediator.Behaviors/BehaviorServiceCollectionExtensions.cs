using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XForge.Mediator.Behaviors;

/// <summary>
/// Registro dos behaviors no DI:
/// <c>services.AddXForgeBehaviors();</c> + <c>services.AddXForgeMediator(...)</c>
/// </summary>
public static class BehaviorServiceCollectionExtensions
{
    /// <summary>
    /// Registra os behaviors (Validation + Logging) como IPipelineBehavior abertos.
    /// O behavior de validação executa todos os IValidator do request registrados no container
    /// (configure-os com services.AddXForgeValidation(assembly) do XForge.Validation).
    /// </summary>
    public static IServiceCollection AddXForgeBehaviors(this IServiceCollection services)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Transient(
                typeof(XForge.Mediator.IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>)));

        services.TryAddEnumerable(
            ServiceDescriptor.Transient(
                typeof(XForge.Mediator.IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)));

        return services;
    }
}
