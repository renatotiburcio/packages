using Microsoft.Extensions.DependencyInjection;

namespace XForge.DesignSystem.Core.Theming
{
    /// <summary>
    /// Extension methods for registering XForge theme services.
    /// </summary>
    public static class XFThemeServiceExtensions
    {
        /// <summary>
        /// Registers the XForge theme service for dark mode support with OS auto-detection.
        /// Usage: builder.Services.AddXForgeTheme();
        /// </summary>
        public static IServiceCollection AddXForgeTheme(this IServiceCollection services)
        {
            services.AddSingleton<XFThemeService>();
            services.AddSingleton<XFThemeInjector>();
            return services;
        }
    }
}
