using System.Linq.Expressions;
using XForge.Mapper;
using XForge.Mapper.Internal;
using XForge.Mapper.Projections.Internal;

namespace XForge.Mapper.Projections;

/// <summary>
/// Projeção <c>ProjectTo</c> para <see cref="IQueryable{T}"/> (EF Core / LINQ): monta a expressão
/// <c>Select</c> a partir dos maps fluentes registrados no XForge.Mapper.
/// </summary>
public static class QueryableProjectionExtensions
{
    /// <summary>
    /// Projeta a consulta para <typeparamref name="TDestination"/> usando os maps registrados
    /// (convenção + fluent). A expressão é traduzida pelo provider (EF Core) — os membros são
    /// resolvidos no banco, não em memória.
    /// </summary>
    public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source,
        IMapper mapper) where TDestination : notnull
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        return ProjectCore<TSource, TDestination>(source, mapper.Configuration);
    }

    /// <summary>Projeção usando diretamente a <see cref="MapperConfiguration"/>.</summary>
    public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source,
        MapperConfiguration configuration) where TDestination : notnull
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return ProjectCore<TSource, TDestination>(source, configuration);
    }

    private static IQueryable<TDestination> ProjectCore<TSource, TDestination>(IQueryable<TSource> source,
        MapperConfiguration configuration) where TDestination : notnull
    {
        var projection = ProjectionCache.GetOrAdd(typeof(TSource), typeof(TDestination), configuration);
        var typedProjection = (Expression<Func<TSource, TDestination>>)projection;
        return source.Select(typedProjection);
    }

    /// <summary>
    /// Projeta a consulta para <typeparamref name="TDestination"/> usando os maps registrados
    /// (convenção + fluent). Forma ergonômica com um único parâmetro de tipo — o tipo da fonte
    /// é resolvido em runtime (dispatch polimórfico na hierarquia de maps).
    /// </summary>
    public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
        IMapper mapper) where TDestination : notnull
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        var projection = ProjectionCache.GetOrAdd(source.ElementType, typeof(TDestination), mapper.Configuration);

        var queryExpression = Expression.Call(
            typeof(Queryable), nameof(Queryable.Select),
            new[] { source.ElementType, typeof(TDestination) },
            source.Expression, Expression.Quote(projection));

        return source.Provider.CreateQuery<TDestination>(queryExpression);
    }

    /// <summary>Projeção usando diretamente a <see cref="MapperConfiguration"/> (fonte não genérica).</summary>
    public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
        MapperConfiguration configuration) where TDestination : notnull
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var projection = ProjectionCache.GetOrAdd(source.ElementType, typeof(TDestination), configuration);

        var queryExpression = Expression.Call(
            typeof(Queryable), nameof(Queryable.Select),
            new[] { source.ElementType, typeof(TDestination) },
            source.Expression, Expression.Quote(projection));

        return source.Provider.CreateQuery<TDestination>(queryExpression);
    }
}
