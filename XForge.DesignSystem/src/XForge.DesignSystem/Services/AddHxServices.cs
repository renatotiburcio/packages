using System;
using Microsoft.Extensions.DependencyInjection;
namespace XForge.DesignSystem.Services;

/// <summary>
/// Legacy registration method. Use AddXForgeComponents() instead.
/// </summary>
[Obsolete("Use services.AddXForgeComponents() instead. This class will be removed in a future version.")]
public static class HxServiceCollectionExtensions
{
    [Obsolete("Use services.AddXForgeComponents() instead.")]
    public static IServiceCollection AddHxServices(this IServiceCollection services, Action<HxServicesOptions>? configure = null)
    {
        return services;
    }
}

/// <summary>
/// Legacy options class. Use XForgeComponentsOptions instead.
/// </summary>
[Obsolete("Use XForgeComponentsOptions instead.")]
public class HxServicesOptions
{
    public bool UseUtcClock { get; set; } = true;
    public TimeProvider? CustomTimeProvider { get; set; }
}
