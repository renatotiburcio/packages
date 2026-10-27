using System.Linq;


using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace XForge.Generators.DocsCompiler;

/// <summary>
/// Symbol representing a discoverable XForge component at compile time.
/// </summary>
public sealed record XFFComponentSymbol(
    string Name,
    string Namespace,
    string Category,
    IReadOnlyList<string> Parameters,
    IReadOnlyList<string> Events,
    IReadOnlyList<string> PublicMethods,
    Accessibility Accessibility);

/// <summary>
/// Aggregated result of a Docs Compiler pass.
/// </summary>
public sealed record XFFDocsCompileResult(
    IReadOnlyList<XFFComponentSymbol> Components,
    int TotalParameters,
    int TotalEvents);

/// <summary>
/// Interface for Docs Compiler pass (testable).
/// </summary>
public interface IXFDocsCompiler
{
    XFFDocsCompileResult Compile(ImmutableArray<INamedTypeSymbol> candidateTypes);
}

public sealed class XFDocsCompiler : IXFDocsCompiler
{
    public XFFDocsCompileResult Compile(ImmutableArray<INamedTypeSymbol> candidateTypes)
    {
        var components = new List<XFFComponentSymbol>();
        int totalParams = 0;
        int totalEvents = 0;
        foreach (var t in candidateTypes)
        {
            if (t.TypeKind != TypeKind.Class) continue;
            if (t.IsAbstract) continue;
            if (!InheritsFromComponentBase(t)) continue;
            var parameters = new List<string>();
            var events = new List<string>();
            foreach (var m in t.GetMembers())
            {
                if (m is IPropertySymbol p)
                {
                    if (p.GetAttributes().Any(a => a.AttributeClass?.Name == "ParameterAttribute"))
                    {
                        parameters.Add(p.Name);
                    }
                }
            }
            components.Add(new XFFComponentSymbol(t.Name, t.ContainingNamespace?.ToString() ?? "", "Uncategorized", parameters, events, new List<string>(), t.DeclaredAccessibility));
            totalParams += parameters.Count;
            totalEvents += events.Count;
        }
        return new XFFDocsCompileResult(components, totalParams, totalEvents);
    }

    private static bool InheritsFromComponentBase(INamedTypeSymbol t)
    {
        var cur = t.BaseType;
        while (cur is not null)
        {
            if (cur.Name == "ComponentBase") return true;
            cur = cur.BaseType;
        }
        return false;
    }
}
