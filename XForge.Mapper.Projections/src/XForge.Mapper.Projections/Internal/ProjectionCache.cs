using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using XForge.Mapper;
using XForge.Mapper.Internal;

namespace XForge.Mapper.Projections.Internal;

/// <summary>Cache de lambdas de projeção por configuração e par de tipos.</summary>
internal static class ProjectionCache
{
    private static readonly ConditionalWeakTable<
        MapperConfiguration, ConcurrentDictionary<(Type, Type), LambdaExpression>> Cache = new();

    public static LambdaExpression GetOrAdd(Type sourceType, Type destinationType, MapperConfiguration configuration)
    {
        var map = Cache.GetValue(configuration, static _ => new ConcurrentDictionary<(Type, Type), LambdaExpression>());
        return map.GetOrAdd((sourceType, destinationType), key => ProjectionExpressionBuilder.Build(key.Item1, key.Item2, configuration));
    }
}
