namespace XForge.Mapper.Internal;

/// <summary>
/// Estado compartilhado durante o build inline de UM plano. <see cref="VisitedPairs"/> detecta ciclos
/// no grafo de tipos durante a construção recursiva das expression trees (caminho atual — pares são
/// removidos ao sair do escopo, permitindo diamantes sem falso positivo).
/// </summary>
internal sealed class PlanContext
{
    public PlanContext(MapperConfiguration configuration)
    {
        Configuration = configuration;
    }

    public MapperConfiguration Configuration { get; }

    public List<string> Errors { get; } = new();

    /// <summary>Pares (origem, destino) em construção no caminho atual — reentrada = ciclo.</summary>
    public HashSet<(Type Source, Type Destination)> VisitedPairs { get; } = new();
}

/// <summary>
/// Exceção interna de fluxo de build: o membro/par não pôde ser inlinado (IgnoreIf, dicionário com
/// valores por classe, par aninhado não mapeável, etc.) — cai para o caminho lento com contexto.
/// Pré-alocada (nunca exposta ao usuário; usada apenas como sinal de controle).
/// </summary>
internal sealed class PlanFallbackException : Exception
{
    internal static readonly PlanFallbackException Instance = new();

    private PlanFallbackException()
        : base("Plan member fell back to the context-aware path.")
    {
    }
}

/// <summary>
/// Exceção interna de fluxo de build: ciclo detectado no grafo de tipos durante o inlining — o plano
/// inteiro cai para o caminho clássico (bindings por membro com cache de instâncias). Pré-alocada.
/// </summary>
internal sealed class CycleDetectedException : Exception
{
    internal static readonly CycleDetectedException Instance = new();

    private CycleDetectedException()
        : base("Type graph cycle detected while inlining the mapping plan.")
    {
    }
}
