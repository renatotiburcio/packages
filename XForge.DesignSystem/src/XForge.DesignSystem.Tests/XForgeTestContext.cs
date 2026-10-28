using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace XForge.DesignSystem.Tests;

/// <summary>
/// Contexto bUnit compartilhado: registra o <see cref="TestLocalizer"/> para todas as
/// classes de teste — 56 componentes fazem <c>[Inject] IXFLocalizer?</c> e o Blazor
/// lança quando o serviço não está registrado.
/// </summary>
public class XForgeTestContext : BunitContext
{
    public XForgeTestContext()
    {
        Services.AddSingleton<XForge.Localization.IXFLocalizer, TestLocalizer>();
    }
}
