// T200: Sprint 5 - Component coverage smoke tests.
// Verifica que cada componente XF* do design system e alcancavel em runtime.
//
// O catalogo passado era uma lista [InlineData] manual que envelheceu: 64 entradas
// apontavam para componentes do satelite AI (descartado) e de ferramentas inexistentes.
// Agora a cobertura e descoberta por reflexao — nunca mais fica velha.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class ComponentCoverageTests
{
    private static readonly string[] ComponentAssemblies =
    [
        "XForge.DesignSystem",
        "XForge.DesignSystem.Shared"
    ];

    private static IReadOnlyList<Type> DiscoverComponents()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => ComponentAssemblies.Contains(assembly.GetName().Name, StringComparer.Ordinal))
            .ToArray();

        Assert.NotEmpty(assemblies);

        return assemblies
            .SelectMany(assembly =>
            {
                try { return assembly.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(type => type.IsPublic && !type.IsAbstract)
            .Where(type => typeof(ComponentBase).IsAssignableFrom(type))
            .Where(type => CleanName(type.Name).StartsWith("XF", StringComparison.Ordinal))
            .Distinct()
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
    }

    [Fact]
    public void ComponentCatalog_ShouldCoverEveryPublicComponent()
    {
        var components = DiscoverComponents();

        // Piso de sanidade: se a descoberta quebrar, o teste nao pode passar com zero.
        Assert.True(components.Count >= 200,
            $"Esperava descobrir ao menos 200 componentes XF*, encontrou {components.Count}.");

        // Todo componente precisa ser instanciavel pelo runtime (nao aberto, publico).
        foreach (var component in components)
        {
            Assert.True(component.IsPublic, $"{component.FullName} deveria ser publico.");
            Assert.False(component.IsAbstract, $"{component.FullName} nao deveria ser abstrato.");
        }
    }

    [Theory]
    [InlineData("XFAccordion")]
    [InlineData("XFButton")]
    [InlineData("XFDataGrid")]
    [InlineData("XFRichTextEditor")]
    [InlineData("XFTreeView")]
    [InlineData("XFTOC")]
    [InlineData("XFTextInput")]
    [InlineData("XFChart")]
    public void CoreComponents_ShouldBeReachable(string componentName)
    {
        var names = DiscoverComponents()
            .Select(type => CleanName(type.Name))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(componentName, names);
    }

    /// <summary>Nome do tipo sem o sufixo de aridade (XFDataGrid`1 -&gt; XFDataGrid).</summary>
    private static string CleanName(string typeName)
    {
        var tick = typeName.IndexOf('`', StringComparison.Ordinal);

        return tick > 0 ? typeName[..tick] : typeName;
    }

    [Fact]
    public void ComponentCatalog_ShouldNotContainAiSatelliteComponents()
    {
        // O satelite XForge.DesignSystem.AI (projeto) foi descartado. O nucleo mantem
        // apenas o chatbot proprio (XFAiChatWithEmoji, Components/AI/Smart).
        var aiComponents = DiscoverComponents()
            .Where(type => CleanName(type.Name).StartsWith("XFAi", StringComparison.Ordinal))
            .Select(type => CleanName(type.Name))
            .ToArray();

        var unexpected = aiComponents.Except(new[] { "XFAiChatWithEmoji" }, StringComparer.Ordinal).ToArray();

        Assert.True(unexpected.Length == 0,
            $"Componentes do satelite AI encontrados no nucleo: {string.Join(", ", unexpected)}");
    }
}
