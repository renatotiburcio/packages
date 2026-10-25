using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XForge.Validation;

/// <summary>
/// Registro de validadores no Microsoft.Extensions.DependencyInjection:
/// <c>services.AddXForgeValidation(typeof(RegistrarPedidoValidator).Assembly);</c>
/// </summary>
public static class XForgeValidationServiceCollectionExtensions
{
    /// <summary>
    /// Escaneia os assemblies informados e registra (singleton) toda classe concreta que implementa
    /// <see cref="IValidator"/> (qualquer IValidator fechado). Isso permite injetar
    /// IValidator do modelo em handlers, controllers e services.
    /// </summary>
    public static IServiceCollection AddXForgeValidation(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        foreach (var assembly in assemblies.Distinct())
        {
            foreach (var (implementationType, serviceTypes) in Scan(assembly))
            {
                services.TryAddSingleton(implementationType);

                foreach (var serviceType in serviceTypes)
                {
                    services.TryAddSingleton(serviceType, implementationType);
                }
            }
        }

        return services;
    }

    private static List<(Type Implementation, List<Type> Services)> Scan(Assembly assembly)
    {
        var results = new List<(Type, List<Type>)>();

        foreach (var type in GetLoadableTypes(assembly))
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            {
                continue;
            }

            var validatorInterfaces = type.GetInterfaces()
                .Where(interfaceType => interfaceType.IsGenericType &&
                                        interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                .ToList();

            if (validatorInterfaces.Count == 0 || !typeof(IValidator).IsAssignableFrom(type))
            {
                continue;
            }

            results.Add((type, validatorInterfaces));
        }

        return results;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
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
