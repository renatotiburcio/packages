using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Extensions;

/// <summary>
/// Extension methods for registering AI services in the DI container.
/// </summary>
public static class XFAiServiceCollectionExtensions
{
    /// <summary>
    /// Creates a fluent builder for configuring AI services.
    /// Use this for full control over provider selection, memory, audit, and web search.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>A fluent builder for AI configuration.</returns>
    public static XFAiBuilder AddXFAi(this IServiceCollection services)
    {
        return new XFAiBuilder(services);
    }

    /// <summary>
    /// Registers all AI foundation services with in-memory implementations.
    /// Quick setup for development and testing.
    /// Call this method in your app's Program.cs to enable AI components.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXFAiDefaults(this IServiceCollection services)
    {
        // In-memory implementations (replace with production implementations as needed)
        services.TryAddSingleton<IAiProvider, InMemoryAiProvider>();
        services.TryAddSingleton<IAiMemoryService, InMemoryAiMemoryService>();
        services.TryAddSingleton<IAiAuditService, InMemoryAiAuditService>();

        return services;
    }

    /// <summary>
    /// Registers AI multi-modal services (vision + audio) with in-memory implementations.
    /// </summary>
    public static IServiceCollection AddXFAiMultiModal(this IServiceCollection services)
    {
        services.TryAddSingleton<IAiVisionProvider, InMemoryAiVisionProvider>();
        services.TryAddSingleton<IAiAudioProvider, InMemoryAiAudioProvider>();
        return services;
    }

    /// <summary>
    /// Registers an AI vision provider.
    /// </summary>
    public static IServiceCollection AddXFAiVisionProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAiVisionProvider
    {
        services.AddSingleton<IAiVisionProvider, TProvider>();
        return services;
    }

    /// <summary>
    /// Registers an AI audio provider.
    /// </summary>
    public static IServiceCollection AddXFAiAudioProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAiAudioProvider
    {
        services.AddSingleton<IAiAudioProvider, TProvider>();
        return services;
    }

    /// <summary>
    /// Registers an AI provider as the default IAiProvider.
    /// </summary>
    public static IServiceCollection AddXFAiProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAiProvider
    {
        services.AddSingleton<IAiProvider, TProvider>();
        services.AddSingleton<IAiEmbeddingService>(sp =>
        {
            var provider = sp.GetRequiredService<IAiProvider>();
            return new XFAiEmbeddingService(provider);
        });

        return services;
    }

    /// <summary>
    /// Registers an AI provider instance as the default IAiProvider.
    /// </summary>
    public static IServiceCollection AddXFAiProvider(this IServiceCollection services, IAiProvider provider)
    {
        services.AddSingleton(provider);
        services.AddSingleton<IAiEmbeddingService>(new XFAiEmbeddingService(provider));

        return services;
    }

    /// <summary>
    /// Registers a named AI provider for multi-provider scenarios.
    /// </summary>
    public static IServiceCollection AddXFAiProvider(this IServiceCollection services, string name, IAiProvider provider)
    {
        services.AddKeyedSingleton(name, provider);
        return services;
    }
}
