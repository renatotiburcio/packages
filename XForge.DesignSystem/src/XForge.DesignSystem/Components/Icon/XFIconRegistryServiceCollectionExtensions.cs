using Microsoft.Extensions.DependencyInjection;

namespace XForge.DesignSystem.Components.Icon;

public static class XFIconRegistryServiceCollectionExtensions
{
    public static IServiceCollection AddXFIconRegistry(this IServiceCollection services)
    {
        services.AddSingleton<IXFIconRegistry, XFIconRegistry>();
        return services;
    }
}