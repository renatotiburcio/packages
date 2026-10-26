using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>
/// Constrói <see cref="MappingPlan{TSource, TDestination}"/>s a partir de <see cref="MapDefinition"/>s.
///
/// Estratégia (v0.2 — inlining recursivo de planos): o plano é construído PRIMEIRO por expression trees
/// recursivas — objetos aninhados (MemberInit) e coleções (Select + materialização) são compilados em
/// UM único delegate por plano (<c>createAndPopulate</c>), a técnica usada por AutoMapper/Mapster.
///
/// Pares com ciclos no grafo de tipos, MaxDepth, ConstructUsing ou callbacks em mapas aninhados caem
/// para o caminho clássico: bindings por membro com <see cref="MappingContext"/> (cache de instâncias
/// para referências circulares e controle de profundidade).
/// </summary>
internal static partial class PlanBuilder
{
    private static readonly MethodInfo BuildGenericMethod =
        typeof(PlanBuilder).GetMethod(nameof(BuildGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo BuildSlowBindingMethod =
        typeof(PlanBuilder).GetMethod(nameof(BuildSlowBinding), BindingFlags.NonPublic | BindingFlags.Static)!;

    public static IMappingPlan Build(MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
    {
        try
        {
            return (IMappingPlan)BuildGenericMethod
                .MakeGenericMethod(definition.SourceType, definition.DestinationType)
                .Invoke(null, new[] { (object)definition, (object)configuration, (object)requireDestinationMemberSource })!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private static IMappingPlan BuildGeneric<TSource, TDestination>(
        MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
        where TSource : notnull
        where TDestination : notnull
    {
        // MaxDepth exige rastreio de profundidade e ConstructUsing exige fábrica custom — caminho clássico direto.
        if (definition.MaxDepth is null && definition.ConstructUsing is null)
        {
            try
            {
                return BuildInlinePlan<TSource, TDestination>(definition, configuration, requireDestinationMemberSource);
            }
            catch (PlanFallbackException)
            {
                // par não inlinável (ex.: destino sem construtor acessível) — caminho clássico reporta os erros
            }
            catch (CycleDetectedException)
            {
                // ciclo no grafo — caminho clássico resolve com cache de instâncias
            }
        }

        return BuildClassicPlan<TSource, TDestination>(definition, configuration, requireDestinationMemberSource);
    }

    // ------------------------------------------------------------------
    // Plano inline (v0.2): criação + população em uma única expression tree
    // ------------------------------------------------------------------

    private static IMappingPlan BuildInlinePlan<TSource, TDestination>(
        MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
        where TSource : notnull
        where TDestination : notnull
    {
        var context = new PlanContext(configuration);
        var sourceParameter = Expression.Parameter(typeof(TSource), "source");
        var creation = BuildCreation(
            definition, typeof(TSource), typeof(TDestination), sourceParameter, context,
            requireDestinationMemberSource, topLevel: true);

        if (context.Errors.Count > 0)
        {
            throw new MappingValidationException(context.Errors);
        }

        // ----- bindings lentos (membros que caíram no fallback: IgnoreIf, dicionários por classe, etc.) -----
        var slowBindings = new List<IMemberBinding<TSource, TDestination>>();

        foreach (var fallback in creation.FallbackMembers)
        {
            var binding = DispatchSlowBinding(
                typeof(TSource), typeof(TDestination), fallback.MemberName, fallback.DestinationMember,
                fallback.SourceMember, fallback.Override, fallback.SourceValueType, fallback.DestinationValueType,
                configuration);

            if (binding is not null)
            {
                slowBindings.Add((IMemberBinding<TSource, TDestination>)binding);
            }
        }

        // ----- createAndPopulate: UM delegate cria E popula (fast path do Map sem callbacks) -----
        var createAndPopulate = CompileCreateAndPopulate<TSource, TDestination>(creation, sourceParameter);

        // ----- create (fábrica): apenas o construtor — caminho com callbacks e CreateDestinationObject -----
        var factory = Expression.Lambda<Func<TSource, TDestination>>(creation.NewInstance, sourceParameter).Compile();

        // ----- fastAction (merge/PopulateObject): atribuições contra a instância de destino -----
        Action<TSource, TDestination>? fastAction = null;

        if (creation.Assignments.Count > 0)
        {
            var destinationParameter = Expression.Parameter(typeof(TDestination), "destination");
            var statements = new List<Expression>(creation.Assignments.Count);

            foreach (var assignment in creation.Assignments)
            {
                statements.Add(Expression.Assign(MemberAccess(destinationParameter, assignment.Member), assignment.Value));
            }

            fastAction = Expression.Lambda<Action<TSource, TDestination>>(
                Expression.Block(typeof(void), statements), sourceParameter, destinationParameter).Compile();
        }

        // ----- callbacks (base primeiro, depois os próprios) -----
        var (before, after) = CollectCallbacks(definition, configuration);

        var requiresContext = slowBindings.Count > 0 ||
                              before.Concat(after).Any(callback => callback is not Action<TSource, TDestination>);

        return new MappingPlan<TSource, TDestination>(
            factory, createAndPopulate, fastAction, slowBindings, before, after, null, requiresContext);
    }

    private static Func<TSource, TDestination> CompileCreateAndPopulate<TSource, TDestination>(
        InlineCreation creation, ParameterExpression sourceParameter)
    {
        if (creation.Assignments.Count == 0)
        {
            return Expression.Lambda<Func<TSource, TDestination>>(creation.NewInstance, sourceParameter).Compile();
        }

        if (creation.HasParameterlessConstructor)
        {
            // new TDest { membros... } — MemberInit: uma única expressão
            var bindings = new List<MemberBinding>(creation.Assignments.Count);

            foreach (var assignment in creation.Assignments)
            {
                bindings.Add(Expression.Bind(assignment.Member, assignment.Value));
            }

            var memberInit = Expression.MemberInit(creation.NewInstance, bindings);
            return Expression.Lambda<Func<TSource, TDestination>>(memberInit, sourceParameter).Compile();
        }

        // destino com construtor parametrizado (records) + membros graváveis restantes: bloco com variável local
        var variable = Expression.Variable(creation.DestinationType, "destination");
        var statements = new List<Expression>(creation.Assignments.Count + 2)
        {
            Expression.Assign(variable, creation.NewInstance),
        };

        foreach (var assignment in creation.Assignments)
        {
            statements.Add(Expression.Assign(MemberAccess(variable, assignment.Member), assignment.Value));
        }

        statements.Add(variable);
        return Expression.Lambda<Func<TSource, TDestination>>(
            Expression.Block(new[] { variable }, statements), sourceParameter).Compile();
    }

    // ------------------------------------------------------------------
    // Helpers compartilhados entre os caminhos inline e clássico
    // ------------------------------------------------------------------

    /// <summary>Dispatch genérico do binding lento (usado pelo fallback inline e pelo caminho clássico).</summary>
    private static object? DispatchSlowBinding(
        Type sourceType, Type destinationType, string memberName, MemberInfo destinationMember,
        MemberInfo? sourceMember, MemberOverride? memberOverride, Type sourceValueType, Type destinationValueType,
        MapperConfiguration configuration)
    {
        return BuildSlowBindingMethod
            .MakeGenericMethod(sourceType, destinationType, sourceValueType, destinationValueType)
            .Invoke(null, new[]
            {
                (object)memberName,
                (object)destinationMember,
                (object?)sourceMember,
                (object?)memberOverride,
                (object)configuration,
            });
    }

    private static Expression MemberAccess(Expression instance, MemberInfo member)
    {
        return member switch
        {
            PropertyInfo property => Expression.Property(instance, property),
            FieldInfo field => Expression.Field(instance, field),
            _ => throw new ArgumentException($"Member '{member.Name}' is not a property or field.", nameof(member)),
        };
    }

    private static (List<Delegate> Before, List<Delegate> After) CollectCallbacks(
        MapDefinition definition, MapperConfiguration configuration)
    {
        var before = new List<Delegate>();
        var after = new List<Delegate>();

        foreach (var (baseSource, baseDestination) in definition.IncludeBases)
        {
            var baseDefinition = configuration.FindDefinition(baseSource, baseDestination);

            if (baseDefinition is not null)
            {
                before.AddRange(baseDefinition.BeforeMap);
                after.AddRange(baseDefinition.AfterMap);
            }
        }

        before.AddRange(definition.BeforeMap);
        after.AddRange(definition.AfterMap);
        return (before, after);
    }
}
