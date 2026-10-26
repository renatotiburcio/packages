using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>
/// Construção inline (v0.2): toda a árvore de criação+população é compilada em expression trees
/// recursivas — objetos aninhados viram MemberInit, coleções viram Select + materialização, tudo
/// dentro de UM delegate por plano. Ciclos são detectados em build-time via <see cref="PlanContext.VisitedPairs"/>.
/// </summary>
internal static partial class PlanBuilder
{
    /// <summary>Resultado do build inline de um par: criação + atribuições + membros que caíram no fallback.</summary>
    private sealed class InlineCreation
    {
        public InlineCreation(NewExpression newInstance, Type destinationType, bool hasParameterlessConstructor)
        {
            NewInstance = newInstance;
            DestinationType = destinationType;
            HasParameterlessConstructor = hasParameterlessConstructor;
        }

        /// <summary><c>new TDest(...)</c> com argumentos já vinculados (por nome da origem ou defaults).</summary>
        public NewExpression NewInstance { get; }

        public Type DestinationType { get; }

        /// <summary>Construtor sem parâmetros → MemberInit; caso contrário (records) → bloco com variável.</summary>
        public bool HasParameterlessConstructor { get; }

        /// <summary>Atribuições de membros (valor já convertido; expressão ancora no parâmetro de origem).</summary>
        public List<InlineAssignment> Assignments { get; } = new();

        /// <summary>Membros vinculados a partir da origem (atribuições + argumentos de construtor).</summary>
        public int BoundFromSource { get; set; }

        /// <summary>Membros que não puderam ser inlinados — resolvidos como bindings lentos pelo plano.</summary>
        public List<InlineFallback> FallbackMembers { get; } = new();
    }

    private readonly struct InlineAssignment
    {
        public InlineAssignment(MemberInfo member, Expression value)
        {
            Member = member;
            Value = value;
        }

        public MemberInfo Member { get; }
        public Expression Value { get; }
    }

    /// <summary>Dados necessários para converter um membro de fallback em binding lento (top-level).</summary>
    private readonly struct InlineFallback
    {
        public InlineFallback(string memberName, MemberInfo destinationMember, MemberInfo? sourceMember,
            MemberOverride? memberOverride, Type sourceValueType, Type destinationValueType)
        {
            MemberName = memberName;
            DestinationMember = destinationMember;
            SourceMember = sourceMember;
            Override = memberOverride;
            SourceValueType = sourceValueType;
            DestinationValueType = destinationValueType;
        }

        public string MemberName { get; }
        public MemberInfo DestinationMember { get; }
        public MemberInfo? SourceMember { get; }
        public MemberOverride? Override { get; }
        public Type SourceValueType { get; }
        public Type DestinationValueType { get; }
    }

    private enum InlineCollectionKind
    {
        ListKind,
        ArrayKind,
        HashSetKind,
        InterfaceKind,
    }

    private static readonly MethodInfo MaterializeListMethod =
        typeof(PlanBuilder).GetMethod(nameof(MaterializeList), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo MaterializeArrayMethod =
        typeof(PlanBuilder).GetMethod(nameof(MaterializeArray), BindingFlags.NonPublic | BindingFlags.Static)!;

    // ------------------------------------------------------------------
    // Construção recursiva da criação de um par
    // ------------------------------------------------------------------

    private static InlineCreation BuildCreation(
        MapDefinition definition, Type sourceType, Type destinationType, Expression sourceParameter,
        PlanContext context, bool requireDestinationMemberSource, bool topLevel)
    {
        var pair = (sourceType, destinationType);

        if (!context.VisitedPairs.Add(pair))
        {
            throw CycleDetectedException.Instance; // reentrada no caminho atual = ciclo → plano clássico
        }

        try
        {
            var configuration = context.Configuration;
            var sourceMembers = ReflectionHelper.GetReadableMembers(sourceType);
            var destinationMembers = ReflectionHelper.GetWritableMembers(destinationType);
            var overrides = CollectOverrides(definition, configuration);

            var constructor = ResolveInlineConstructor(destinationType);

            if (constructor is null)
            {
                // sem construtor acessível — o caminho clássico reporta o erro de validação
                throw PlanFallbackException.Instance;
            }

            var constructorParameters = constructor.GetParameters();
            var ctorArguments = new Expression?[constructorParameters.Length];
            var boundFromSource = 0;

            // ----- argumentos de construtor vinculados por nome (records/imutáveis) — paridade com ResolveFactory -----
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                var parameter = constructorParameters[i];
                var sourceMember = FindSourceMember(sourceMembers, parameter.Name!);

                if (sourceMember is null)
                {
                    ctorArguments[i] = parameter.HasDefaultValue
                        ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                        : Expression.Constant(parameter.ParameterType.IsValueType
                            ? Activator.CreateInstance(parameter.ParameterType)
                            : null, parameter.ParameterType);
                    continue;
                }

                var access = MemberAccess(sourceParameter, sourceMember);
                ctorArguments[i] = ConvertCtorArgument(
                    access, sourceMember.GetMemberType(), parameter.ParameterType, configuration, context.Errors,
                    $"constructor parameter '{parameter.Name}' of '{destinationType}'");
                boundFromSource++;
            }

            var newInstance = constructorParameters.Length == 0
                ? Expression.New(constructor)
                : Expression.New(constructor, ctorArguments!);

            var creation = new InlineCreation(newInstance, destinationType, constructorParameters.Length == 0);

            // ----- loop de membros -----
            foreach (var destinationMember in destinationMembers)
            {
                var destinationMemberType = destinationMember.GetMemberType();
                var memberOverride = FindOverride(overrides, destinationMember.Name);

                // ignore puro → membro intencionalmente não mapeado
                if (memberOverride is not null && memberOverride.Ignore && memberOverride.IgnoreCondition is null)
                {
                    continue;
                }

                var hasExplicitMap = memberOverride is { SourceValue: not null };
                var conventionSourceMember = hasExplicitMap ? null : FindSourceMember(sourceMembers, destinationMember.Name);

                if (!hasExplicitMap && conventionSourceMember is null)
                {
                    if (requireDestinationMemberSource)
                    {
                        context.Errors.Add($"Destination member '{destinationMember.Name}' on '{destinationType}' has no matching " +
                                           "source member and no Map/Ignore override (RequireDestinationMemberSource).");
                    }

                    continue;
                }

                var sourceValueType = hasExplicitMap
                    ? memberOverride!.SourceValue!.GetType().GetGenericArguments()[1]
                    : conventionSourceMember!.GetMemberType();

                var errorsBeforeMember = context.Errors.Count;

                try
                {
                    // membros condicionais (IgnoreIf) não são expressáveis em MemberInit → caminho lento
                    if (memberOverride is not null && memberOverride.IgnoreCondition is not null)
                    {
                        throw PlanFallbackException.Instance;
                    }

                    Expression sourceAccess = hasExplicitMap
                        ? InvokeOverride(memberOverride!.SourceValue!, sourceParameter, sourceValueType)
                        : MemberAccess(sourceParameter, conventionSourceMember!);

                    var value = BuildMemberValue(sourceAccess, sourceValueType, destinationMemberType, context);

                    if (value is null)
                    {
                        continue; // incompatível — membro ignorado silenciosamente (paridade AutoMapper)
                    }

                    if (value.Type != destinationMemberType)
                    {
                        value = Expression.Convert(value, destinationMemberType);
                    }

                    creation.Assignments.Add(new InlineAssignment(destinationMember, value));
                    boundFromSource++;
                }
                catch (PlanFallbackException)
                {
                    // rollback de erros do attempt abortado (erros de conversão de ctor de pares aninhados)
                    if (context.Errors.Count > errorsBeforeMember)
                    {
                        context.Errors.RemoveRange(errorsBeforeMember, context.Errors.Count - errorsBeforeMember);
                    }

                    if (!topLevel)
                    {
                        throw; // pares aninhados propagam — o MEMBRO do plano corrente vai para o caminho lento
                    }

                    creation.FallbackMembers.Add(new InlineFallback(
                        destinationMember.Name!, destinationMember, conventionSourceMember, memberOverride,
                        sourceValueType, destinationMemberType));
                }
            }

            creation.BoundFromSource = boundFromSource;
            return creation;
        }
        finally
        {
            context.VisitedPairs.Remove(pair); // diamantes (mesmo par em ramos distintos) são permitidos
        }
    }

    // ------------------------------------------------------------------
    // Valor de um membro (decisão: direto / conversor / aninhado / coleção)
    // ------------------------------------------------------------------

    private static Expression? BuildMemberValue(
        Expression sourceAccess, Type sourceValueType, Type destinationMemberType, PlanContext context)
    {
        if (sourceValueType == destinationMemberType)
        {
            return sourceAccess;
        }

        // ----- coleção → coleção: verificar PRIMEIRO -----
        // (TryCreateCollectionConverter descarta o flag de contexto do elemento — o conversor de
        // coleção embute NestedConverter por elemento = dispatch por item, o gargalo v0.1)
        var sourceElementType = GetEnumerableElementType(sourceValueType);

        if (sourceElementType is not null)
        {
            var destinationTarget = GetInlineCollectionTarget(destinationMemberType);

            if (destinationTarget is not null)
            {
                return BuildCollectionValue(sourceAccess, sourceValueType, destinationMemberType,
                    sourceElementType, destinationTarget.Value.Kind, destinationTarget.Value.ItemType, context);
            }

            // destino dicionário/coleção não inlinável → conversor dedicado (paridade v0.1)
            var dedicatedConverter = ConverterFactory.TryCreate(
                sourceValueType, destinationMemberType, context.Configuration, out _);

            return dedicatedConverter is null
                ? null
                : Expression.Invoke(Expression.Constant(dedicatedConverter), sourceAccess);
        }

        // ----- conversores context-free (enums, strings, operadores, primitivos) — ordem TryCreate, paridade total -----
        var converter = ConverterFactory.TryCreate(
            sourceValueType, destinationMemberType, context.Configuration, out var requiresContext);

        if (!requiresContext)
        {
            return converter is null
                ? null // incompatível — membro ignorado silenciosamente (paridade AutoMapper)
                : Expression.Invoke(Expression.Constant(converter), sourceAccess);
        }

        // ----- requiresContext: objeto aninhado → INLINING recursivo (MemberInit) -----
        if (IsMappableClass(Nullable.GetUnderlyingType(sourceValueType) ?? sourceValueType) &&
            IsMappableClass(Nullable.GetUnderlyingType(destinationMemberType) ?? destinationMemberType))
        {
            return BuildNestedValue(sourceAccess, sourceValueType, destinationMemberType, context);
        }

        throw PlanFallbackException.Instance;
    }

    // ------------------------------------------------------------------
    // Objeto aninhado → objeto (MemberInit recursivo)
    // ------------------------------------------------------------------

    private static Expression BuildNestedValue(
        Expression sourceAccess, Type sourceValueType, Type destinationMemberType, PlanContext context)
    {
        var configuration = context.Configuration;
        var nestedDefinition = configuration.FindDefinition(sourceValueType, destinationMemberType)
                               ?? new MapDefinition(sourceValueType, destinationMemberType);

        // callbacks/MaxDepth/ConstructUsing em mapas aninhados exigem execução com contexto → clássico
        if (NestedDefinitionRequiresClassicPath(nestedDefinition, configuration))
        {
            throw PlanFallbackException.Instance;
        }

        var creation = BuildCreation(
            nestedDefinition, sourceValueType, destinationMemberType, sourceAccess, context,
            requireDestinationMemberSource: false, topLevel: false);

        // par aninhado sem nenhum membro mapeável: o caminho clássico lança "Missing map" em runtime —
        // preservamos o comportamento caindo para o binding lento (resolver sintetiza o plano do par)
        if (creation.BoundFromSource == 0 &&
            ReflectionHelper.GetWritableMembers(destinationMemberType).Count > 0 &&
            CollectOverrides(nestedDefinition, configuration).Count == 0)
        {
            throw PlanFallbackException.Instance;
        }

        var body = ToCreationExpression(creation, destinationMemberType);

        if (!sourceAccess.Type.IsValueType || Nullable.GetUnderlyingType(sourceAccess.Type) is not null)
        {
            return NullGuard(sourceAccess, body, destinationMemberType);
        }

        return body;
    }

    // ------------------------------------------------------------------
    // Coleção → coleção (Select com projeção de elemento recursiva)
    // ------------------------------------------------------------------

    private static Expression BuildCollectionValue(
        Expression sourceAccess, Type sourceValueType, Type destinationMemberType,
        Type sourceElementType, InlineCollectionKind kind, Type destinationItemType, PlanContext context)
    {
        LambdaExpression? projection = null;

        if (sourceElementType != destinationItemType)
        {
            var elementConverter = ConverterFactory.TryCreate(
                sourceElementType, destinationItemType, context.Configuration, out var elementRequiresContext);

            if (elementConverter is not null && !elementRequiresContext)
            {
                // conversão simples por elemento (enum, string, primitivo, nullable, coleção context-free…)
                var itemParameter = Expression.Parameter(sourceElementType, "item");
                Expression body = Expression.Invoke(Expression.Constant(elementConverter), itemParameter);

                if (body.Type != destinationItemType)
                {
                    body = Expression.Convert(body, destinationItemType);
                }

                projection = Expression.Lambda(body, itemParameter);
            }
            else if (!IsEnumerableType(sourceElementType) && !IsEnumerableType(destinationItemType) &&
                     IsMappableClass(Nullable.GetUnderlyingType(sourceElementType) ?? sourceElementType) &&
                     IsMappableClass(Nullable.GetUnderlyingType(destinationItemType) ?? destinationItemType))
            {
                // elemento objeto→objeto: projeção recursiva INLINADA (o ganho do complex graph)
                var itemParameter = Expression.Parameter(sourceElementType, "item");
                var body = BuildNestedValue(itemParameter, sourceElementType, destinationItemType, context);
                projection = Expression.Lambda(body, itemParameter);
            }
            else
            {
                // elementos incompatíveis ou coleções/dicionários aninhados com elementos por classe:
                // caminho lento usa o conversor de coleção (paridade v0.1)
                throw PlanFallbackException.Instance;
            }
        }

        Expression materialized;

        if (projection is null)
        {
            // pass-through (mesma tipagem): ToList/ToArray usam o ctor de ICollection → já pré-dimensionam
            materialized = MaterializeCollection(kind, destinationItemType, sourceAccess);
        }
        else
        {
            // IMPORTANTE: compilar a projeção SEPARADAMENTE e injetá-la como constante.
            // Lambdas citadas (Quote) embutidas na árvore do plano têm invocação ~3× mais lenta
            // do que delegados pré-compilados passados como Expression.Constant (medido: 771 ns
            // vs 238 ns no grafo complexo de 10 itens).
            var compiledProjection = projection.Compile();

            switch (kind)
            {
                case InlineCollectionKind.ArrayKind:
                {
                    var method = MaterializeArrayMethod.MakeGenericMethod(sourceElementType, destinationItemType);
                    materialized = Expression.Call(method, sourceAccess, Expression.Constant(compiledProjection));
                    break;
                }

                case InlineCollectionKind.HashSetKind:
                {
                    var selectMethod = typeof(Enumerable).GetMethods()
                        .First(m => m.Name == nameof(Enumerable.Select) && m.GetParameters().Length == 2)
                        .MakeGenericMethod(sourceElementType, destinationItemType);
                    var selectCall = Expression.Call(selectMethod, sourceAccess, Expression.Constant(compiledProjection));
                    materialized = MaterializeCollection(kind, destinationItemType, selectCall);
                    break;
                }

                default:
                {
                    // materializador estático pré-dimensionado via ICollection.Count
                    // (Select + ToList cresce a lista dinamicamente — realocações e cópias desnecessárias)
                    var method = MaterializeListMethod.MakeGenericMethod(sourceElementType, destinationItemType);
                    materialized = Expression.Call(method, sourceAccess, Expression.Constant(compiledProjection));
                    break;
                }
            }
        }

        if (materialized.Type != destinationMemberType)
        {
            materialized = Expression.Convert(materialized, destinationMemberType);
        }

        if (!sourceValueType.IsValueType || Nullable.GetUnderlyingType(sourceValueType) is not null)
        {
            return NullGuard(sourceAccess, materialized, destinationMemberType);
        }

        return materialized;
    }

    private static Expression MaterializeCollection(InlineCollectionKind kind, Type itemType, Expression enumerable)
    {
        switch (kind)
        {
            case InlineCollectionKind.ArrayKind:
            {
                var toArray = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))!.MakeGenericMethod(itemType);
                return Expression.Call(toArray, enumerable);
            }

            case InlineCollectionKind.HashSetKind:
            {
                var hashSetType = typeof(HashSet<>).MakeGenericType(itemType);
                var constructor = hashSetType.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
                return Expression.New(constructor!, enumerable);
            }

            default:
            {
                // List<T> e interfaces (ICollection/IReadOnlyList/IReadOnlyCollection/IEnumerable) → ToList
                var toList = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(itemType);
                return Expression.Call(toList, enumerable);
            }
        }
    }

    /// <summary>Materializador com pré-dimensionamento (ICollection.Count) — evita o crescimento dinâmico do ToList sobre Select.</summary>
    private static List<TDestItem> MaterializeList<TSrcItem, TDestItem>(IEnumerable<TSrcItem> source, Func<TSrcItem, TDestItem> projection)
    {
        var count = source is ICollection<TSrcItem> collection ? collection.Count : 0;
        var result = new List<TDestItem>(count);

        foreach (var item in source)
        {
            result.Add(projection(item));
        }

        return result;
    }

    private static TDestItem[] MaterializeArray<TSrcItem, TDestItem>(IEnumerable<TSrcItem> source, Func<TSrcItem, TDestItem> projection)
    {
        var count = source is ICollection<TSrcItem> collection ? collection.Count : 0;
        var result = new TDestItem[count];
        var index = 0;

        foreach (var item in source)
        {
            result[index++] = projection(item);
        }

        return result;
    }

    // ------------------------------------------------------------------
    // Materialização da criação como expressão
    // ------------------------------------------------------------------

    private static Expression ToCreationExpression(InlineCreation creation, Type destinationType)
    {
        if (creation.Assignments.Count == 0)
        {
            return creation.NewInstance;
        }

        if (creation.HasParameterlessConstructor)
        {
            var bindings = new List<MemberBinding>(creation.Assignments.Count);

            foreach (var assignment in creation.Assignments)
            {
                bindings.Add(Expression.Bind(assignment.Member, assignment.Value));
            }

            return Expression.MemberInit(creation.NewInstance, bindings);
        }

        // construtor parametrizado + membros graváveis restantes → bloco com variável local
        var variable = Expression.Variable(destinationType, "created");
        var statements = new List<Expression>(creation.Assignments.Count + 2)
        {
            Expression.Assign(variable, creation.NewInstance),
        };

        foreach (var assignment in creation.Assignments)
        {
            statements.Add(Expression.Assign(MemberAccess(variable, assignment.Member), assignment.Value));
        }

        statements.Add(variable);
        return Expression.Block(new[] { variable }, statements);
    }

    /// <summary><c>source == null ? null : body</c> — guarda para objetos aninhados e coleções nuláveis.</summary>
    private static Expression NullGuard(Expression sourceAccess, Expression whenNotNull, Type resultType)
    {
        var test = Expression.NotEqual(sourceAccess, Expression.Constant(null, sourceAccess.Type));
        var convertedBody = whenNotNull.Type == resultType ? whenNotNull : Expression.Convert(whenNotNull, resultType);
        return Expression.Condition(test, convertedBody, Expression.Constant(null, resultType));
    }

    private static Expression InvokeOverride(Delegate sourceValue, Expression sourceParameter, Type resultType)
    {
        var invocation = Expression.Invoke(Expression.Constant(sourceValue), sourceParameter);

        if (invocation.Type != resultType)
        {
            return Expression.Convert(invocation, resultType);
        }

        return invocation;
    }

    private static ConstructorInfo? ResolveInlineConstructor(Type destinationType)
    {
        var parameterless = destinationType.GetConstructor(Type.EmptyTypes);

        if (parameterless is not null && parameterless.IsPublic)
        {
            return parameterless;
        }

        return ChooseParameterizedConstructor(destinationType);
    }
}
