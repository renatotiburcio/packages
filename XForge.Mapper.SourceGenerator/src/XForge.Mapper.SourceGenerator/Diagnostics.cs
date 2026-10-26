using Microsoft.CodeAnalysis;

namespace XForge.Mapper.Generators;

/// <summary>Diagnósticos do source generator (prefixo XFG).</summary>
internal static class Diagnostics
{
    private const string Category = "XForge.Mapper.SourceGenerator";

    public static readonly DiagnosticDescriptor ClassMustBePartial = new(
        "XFG001",
        "Classe [XForgeMap] deve ser partial",
        "A classe '{0}' marcada com [XForgeMap] deve ser declarada como partial",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnsupportedMemberTypes = new(
        "XFG002",
        "Membro de destino não suportado em v0.1 (tipos incompatíveis)",
        "Membro '{0}' de '{1}' → '{2}': tipos incompatíveis para geração automática — " +
        "mapeie manualmente ou use o motor de runtime (XForge.Mapper)",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnmappedDestinationMember = new(
        "XFG003",
        "Membro de destino sem contraparte na origem",
        "Membro '{0}' de '{1}' não encontrado em '{2}' e não será mapeado " +
        "(paridade com o motor de runtime, que ignora silenciosamente)",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnsupportedSignature = new(
        "XFG004",
        "Assinatura de método parcial não suportada",
        "Método parcial '{0}' não corresponde a nenhuma forma suportada " +
        "(Dest Map(Src), void Map(Src, Dest) ou List&lt;Dest&gt; Map(List&lt;Src&gt;))",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
