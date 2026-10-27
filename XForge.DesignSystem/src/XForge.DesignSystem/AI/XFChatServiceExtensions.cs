using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XForge.DesignSystem.AI;

/// <summary>
/// Extension methods for registering AI chat services.
/// </summary>
public static class XFChatServiceExtensions
{
    /// <summary>
    /// Registers the XForge AI chat completion service and HttpClient.
    /// </summary>
    public static IServiceCollection AddXFChatCompletion(this IServiceCollection services, Action<XFChatCompletionOptions>? configure = null)
    {
        services.TryAddSingleton<HttpClient>();

        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<XFChatCompletionOptions>(_ => { });
        }

        services.TryAddScoped<IXFChatCompletionService, XFChatCompletionService>();

        return services;
    }
}
