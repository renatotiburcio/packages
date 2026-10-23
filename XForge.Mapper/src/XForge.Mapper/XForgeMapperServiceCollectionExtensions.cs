using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XForge.Mapper;

/// <summary>
/// Registration extensions for Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class XForgeMapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="MapperConfiguration"/> and <see cref="IMapper"/> (singleton) configured
    /// via the fluent DSL (see the <see cref="IMapperConfigurationExpression"/> methods).
    /// </summary>
    public static IServiceCollection AddXForgeMapper(this IServiceCollection services,
        Action<IMapperConfigurationExpression>? configure = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var configuration = new MapperConfiguration(configure ?? DefaultConfigure);
        services.TryAddSingleton(configuration);
        services.TryAddSingleton<IMapper>(static serviceProvider =>
            serviceProvider.GetRequiredService<MapperConfiguration>().CreateMapper());
        return services;
    }

    /// <summary>Registers a pre-built <see cref="MapperConfiguration"/> and its <see cref="IMapper"/>.</summary>
    public static IServiceCollection AddXForgeMapper(this IServiceCollection services, MapperConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.TryAddSingleton(configuration);
        services.TryAddSingleton<IMapper>(static serviceProvider =>
            serviceProvider.GetRequiredService<MapperConfiguration>().CreateMapper());
        return services;
    }

    private static void DefaultConfigure(IMapperConfigurationExpression configuration)
    {
    }
}
