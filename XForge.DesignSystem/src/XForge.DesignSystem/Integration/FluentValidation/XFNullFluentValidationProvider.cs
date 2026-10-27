using Microsoft.Extensions.DependencyInjection;


// T048: Null implementation used when FluentValidation is not registered.
namespace XForge.DesignSystem.Integration.FluentValidation;

/// <summary>
/// Default no-op validation provider. Replace via DI when FluentValidation validators are registered.
/// </summary>
public sealed class XFNullFluentValidationProvider : IXFFluentValidationProvider
{
    public object? ResolveValidator<T>() => null;
    public bool HasValidator<T>() => false;
}

/// <summary>DI extension to register the provider.</summary>
public static class XFFluentValidationServiceCollectionExtensions
{
    public static IServiceCollection AddXFFluentValidation(this IServiceCollection services)
    {
        services.AddSingleton<IXFFluentValidationProvider, XFNullFluentValidationProvider>();
        return services;
    }
}
