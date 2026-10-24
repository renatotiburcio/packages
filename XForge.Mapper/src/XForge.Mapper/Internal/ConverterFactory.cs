using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>
/// Builds typed converters (Func of source-value to destination-value) between member value types:
/// nullables, enums (name-based), strings (parse/format), primitives, user-defined operators,
/// collections/dictionaries and nested objects (recursive plans).
/// </summary>
internal static class ConverterFactory
{
    /// <summary>
    /// Creates a converter delegate or returns <see langword="null"/> when the types are identical
    /// (identity) or incompatible (the member binding is then skipped by the plan builder).
    /// </summary>
    public static Delegate? TryCreate(Type sourceType, Type destinationType, MapperConfiguration configuration, out bool requiresContext)
    {
        requiresContext = false;

        if (sourceType == destinationType)
        {
            return null;
        }

        // ---- Nullables (unwrap one level and recurse) ----
        var sourceUnderlying = Nullable.GetUnderlyingType(sourceType);
        var destinationUnderlying = Nullable.GetUnderlyingType(destinationType);

        if (sourceUnderlying is not null || destinationUnderlying is not null)
        {
            var effectiveSource = sourceUnderlying ?? sourceType;
            var effectiveDestination = destinationUnderlying ?? destinationType;
            var inner = TryCreate(effectiveSource, effectiveDestination, configuration, out var innerRequiresContext);
            requiresContext = innerRequiresContext;
            return BuildNullableWrapper(sourceType, destinationType, effectiveSource, effectiveDestination, inner);
        }

        var isSourceString = sourceType == typeof(string);
        var isDestinationString = destinationType == typeof(string);

        // ---- Enum → enum / enum → number / number → enum ----
        if (sourceType.IsEnum && !isDestinationString)
        {
            return BuildGeneric(nameof(EnumToEnumConverter), new[] { sourceType, destinationType }, destinationType);
        }

        if (destinationType.IsEnum && !isSourceString)
        {
            var effectiveSourceForEnum = Nullable.GetUnderlyingType(sourceType) ?? sourceType;

            if (!IsConvertiblePrimitive(effectiveSourceForEnum))
            {
                return null; // only numeric/string sources can become enums (string handled above)
            }

            return BuildGeneric(nameof(NumberToEnumConverter), new[] { sourceType, destinationType }, destinationType);
        }

        // ---- string → primitive / enum / Guid / TimeSpan / char ----
        if (isSourceString)
        {
            return BuildGeneric(nameof(StringConverter), new[] { destinationType }, destinationType);
        }

        // ---- anything → string (primitives, enums, Guid, TimeSpan…) ----
        if (isDestinationString)
        {
            return BuildGeneric(nameof(ToStringConverter), new[] { sourceType }, sourceType);
        }

        // ---- char ↔ string ----
        if (sourceType == typeof(char) || destinationType == typeof(char))
        {
            return BuildGeneric(nameof(CharConverter), new[] { sourceType, destinationType });
        }

        // ---- user-defined implicit/explicit operators (guaranteed to exist) ----
        var conversionOperator = FindConversionOperator(sourceType, destinationType);

        if (conversionOperator is not null)
        {
            return BuildGeneric(nameof(OperatorConverter), new[] { sourceType, destinationType }, conversionOperator);
        }

        // ---- IConvertible primitives (numbers, bool, DateTime…) ----
        if (IsConvertiblePrimitive(sourceType) && IsConvertiblePrimitive(destinationType))
        {
            return BuildGeneric(nameof(ChangeTypeConverter), new[] { sourceType, destinationType });
        }

        // ---- collections / dictionaries (copies — never share instances) ----
        var collectionConverter = TryCreateCollectionConverter(sourceType, destinationType, configuration, out var collectionRequiresContext);

        if (collectionConverter is not null)
        {
            requiresContext = collectionRequiresContext;
            return collectionConverter;
        }

        // ---- assignment-compatible reference casts (interfaces, downcasts) — not for mappable classes ----
        if ((destinationType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(destinationType)) &&
            !(IsMappableClass(sourceType) && IsMappableClass(destinationType)))
        {
            return BuildGeneric(nameof(CastConverter), new[] { sourceType, destinationType });
        }

        // ---- nested object → object (recursive plan) ----
        if (IsMappableClass(sourceType) && IsMappableClass(destinationType))
        {
            requiresContext = true;
            return BuildGeneric(nameof(NestedConverter), new[] { sourceType, destinationType }, configuration);
        }

        return null;
    }

    // ------------------------------------------------------------------
    // Dispatch helpers
    // ------------------------------------------------------------------

    private static Delegate BuildGeneric(string methodName, Type[] genericArguments, params object[] arguments)
    {
        var method = typeof(ConverterFactory).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;
        var closed = method.MakeGenericMethod(genericArguments);
        return (Delegate)closed.Invoke(null, arguments)!;
    }

    private static MethodInfo? FindConversionOperator(Type sourceType, Type destinationType)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

        foreach (var type in new[] { sourceType, destinationType })
        {
            foreach (var method in type.GetMethods(flags))
            {
                if (method.Name is not ("op_Implicit" or "op_Explicit") ||
                    method.ReturnType != destinationType ||
                    method.GetParameters().Length != 1)
                {
                    continue;
                }

                var parameterType = method.GetParameters()[0].ParameterType;
                var effectiveParameter = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
                var effectiveSource = Nullable.GetUnderlyingType(sourceType) ?? sourceType;

                if (effectiveParameter == effectiveSource)
                {
                    return method;
                }
            }
        }

        return null;
    }

    private static bool IsConvertiblePrimitive(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return effective.IsPrimitive || effective == typeof(decimal) || effective == typeof(DateTime) || effective == typeof(DateTimeOffset);
    }

    private static bool IsMappableClass(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return !effective.IsPrimitive && effective != typeof(decimal) && effective != typeof(string) &&
               effective != typeof(DateTime) && effective != typeof(DateTimeOffset) && effective != typeof(Guid) &&
               effective != typeof(TimeSpan) && effective != typeof(object) && !effective.IsEnum;
    }

    // ------------------------------------------------------------------
    // Nullable wrappers (built via expressions — identity cases compile without boxing)
    // ------------------------------------------------------------------

    private static Delegate BuildNullableWrapper(Type sourceType, Type destinationType,
        Type effectiveSource, Type effectiveDestination, Delegate? inner)
    {
        var sourceIsNullable = Nullable.GetUnderlyingType(sourceType) is not null;
        var destinationIsNullable = Nullable.GetUnderlyingType(destinationType) is not null;

        var parameter = Expression.Parameter(sourceType, "value");

        Expression coreResult;

        if (inner is not null)
        {
            Expression innerArgument = parameter;

            if (sourceIsNullable)
            {
                innerArgument = Expression.Property(parameter, sourceType.GetProperty("Value")!);
            }

            if (innerArgument.Type != effectiveSource)
            {
                innerArgument = Expression.Convert(innerArgument, effectiveSource);
            }

            coreResult = Expression.Invoke(Expression.Constant(inner), innerArgument);
        }
        else
        {
            // identity: effectiveSource == effectiveDestination
            coreResult = sourceIsNullable
                ? Expression.Property(parameter, sourceType.GetProperty("Value")!)
                : (Expression)parameter;
        }

        Expression body;

        if (sourceIsNullable)
        {
            var hasValue = Expression.Property(parameter, sourceType.GetProperty("HasValue")!);
            var whenNull = Expression.Constant(
                effectiveDestination.IsValueType ? Activator.CreateInstance(effectiveDestination) : null,
                destinationType);
            body = Expression.Condition(hasValue, Expression.Convert(coreResult, destinationType), whenNull);
        }
        else
        {
            body = Expression.Convert(coreResult, destinationType);
        }

        var lambdaType = typeof(Func<,>).MakeGenericType(sourceType, destinationType);
        return Expression.Lambda(lambdaType, body, parameter).Compile();
    }

    // ------------------------------------------------------------------
    // Enums / numbers / strings
    // ------------------------------------------------------------------

    private static Func<TSrc, TDest> EnumToEnumConverter<TSrc, TDest>(Type destinationType)
    {
        return src => src is null ? default! : (TDest)ConvertEnumToObject((Enum)(object)src, destinationType)!;
    }

    private static Func<TSrc, TDest> NumberToEnumConverter<TSrc, TDest>(Type destinationType)
    {
        var destinationEnum = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
        var underlyingType = Enum.GetUnderlyingType(destinationEnum);

        return src => src is null
            ? default!
            : (TDest)Enum.ToObject(destinationEnum, Convert.ChangeType(src, underlyingType, CultureInfo.InvariantCulture))!;
    }

    private static object? ConvertEnumToObject(Enum value, Type destinationType)
    {
        var destinationUnderlying = Nullable.GetUnderlyingType(destinationType);
        var destinationEnum = destinationUnderlying ?? destinationType;

        if (destinationEnum.IsEnum)
        {
            try
            {
                return Enum.Parse(destinationEnum, value.ToString(), ignoreCase: true);
            }
            catch (ArgumentException)
            {
                var underlyingType = Enum.GetUnderlyingType(destinationEnum);
                var numeric = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                return Enum.ToObject(destinationEnum, numeric);
            }
        }

        return Convert.ChangeType(value, destinationEnum, CultureInfo.InvariantCulture);
    }

    private static Func<string?, TDest> StringConverter<TDest>(Type destinationType)
    {
        var effectiveDestination = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

        if (!IsParseable(effectiveDestination))
        {
            return static text => default!; // incompatible → skipped by caller
        }

        return text => string.IsNullOrEmpty(text)
            ? default!
            : (TDest)ParseValue(text!, effectiveDestination)!;
    }

    private static bool IsParseable(Type type)
    {
        return type.IsEnum || type == typeof(string) || type == typeof(Guid) || type == typeof(TimeSpan) ||
               type == typeof(char) || IsConvertiblePrimitive(type);
    }

    private static object? ParseValue(string text, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return text;
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, text, ignoreCase: true);
        }

        if (targetType == typeof(Guid))
        {
            return Guid.Parse(text);
        }

        if (targetType == typeof(TimeSpan))
        {
            return TimeSpan.Parse(text, CultureInfo.InvariantCulture);
        }

        if (targetType == typeof(char))
        {
            return text.Length == 1 ? text[0] : throw new FormatException($"Cannot convert '{text}' to char.");
        }

        if (targetType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(text, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture);
    }

    private static Func<TSrc, string> ToStringConverter<TSrc>(Type sourceType)
    {
        // only convertible/format sources produce meaningful strings; incompatible sources → skipped by caller
        if (!(IsConvertiblePrimitive(sourceType) || sourceType.IsEnum || sourceType == typeof(Guid) || sourceType == typeof(TimeSpan)))
        {
            return static src => default!; // incompatible → skipped by caller
        }

        return src => src switch
        {
            null => string.Empty,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => src.ToString() ?? string.Empty,
        };
    }

    private static Func<TSrc, TDest> CharConverter<TSrc, TDest>()
    {
        if (typeof(TSrc) == typeof(char))
        {
            return src => src is null ? default! : (TDest)(object)src.ToString()!;
        }

        return src => src switch
        {
            null => default!,
            string text when text.Length == 1 => (TDest)(object)text[0],
            string => throw new FormatException("Cannot convert string with more than one character to char."),
            _ => default!,
        };
    }

    private static Func<TSrc, TDest> ChangeTypeConverter<TSrc, TDest>()
    {
        var effectiveDestination = Nullable.GetUnderlyingType(typeof(TDest)) ?? typeof(TDest);

        return src => src is null
            ? default!
            : (TDest)Convert.ChangeType(src, effectiveDestination, CultureInfo.InvariantCulture)!;
    }

    private static Func<TSrc, TDest> OperatorConverter<TSrc, TDest>(MethodInfo operatorMethod)
    {
        return v => (TDest)operatorMethod.Invoke(null, new[] { (object?)v })!;
    }

    private static Func<TSrc, TDest> CastConverter<TSrc, TDest>()
    {
        return static v => v is null ? default! : (TDest)(object)v;
    }

    // ------------------------------------------------------------------
    // Collections / dictionaries
    // ------------------------------------------------------------------

    private enum CollectionKind
    {
        List,
        Array,
        HashSet,
        Enumerable,
        Dictionary,
    }

    private static Delegate? TryCreateCollectionConverter(Type sourceType, Type destinationType,
        MapperConfiguration configuration, out bool requiresContext)
    {
        requiresContext = false;

        var sourceItem = GetEnumerableElementType(sourceType);
        var destinationTarget = GetCollectionTarget(destinationType);

        if (sourceItem is null || destinationTarget is null)
        {
            return null;
        }

        if (destinationTarget.Kind == CollectionKind.Dictionary)
        {
            var sourceKey = GetDictionaryValueArgument(sourceType, 0);
            var sourceValue = GetDictionaryValueArgument(sourceType, 1);

            if (sourceKey is null || sourceValue is null)
            {
                return null;
            }

            var keyConverter = TryCreate(sourceKey, destinationTarget.KeyType!, configuration, out var keyContext);
            var valueConverter = TryCreate(sourceValue, destinationTarget.ItemType, configuration, out var valueContext);
            requiresContext = keyContext || valueContext;

            var method = typeof(ConverterFactory)
                .GetMethod(nameof(DictionaryConverter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(sourceType, destinationType, sourceKey, sourceValue, destinationTarget.KeyType!, destinationTarget.ItemType);

            return (Delegate)method.Invoke(null, new[] { keyConverter, valueConverter })!;
        }

        var elementConverter = TryCreate(sourceItem, destinationTarget.ItemType, configuration, out var elementContext);
        _ = elementContext; // elements map context-less (circular references THROUGH collections are a documented v0.1 limitation)

        // IMPORTANT: the collection converter is the final transform — do NOT wrap it in a nested-plan
        // transform (that would synthesize a convention plan for the collection types themselves).
        var kind = destinationTarget.Kind;
        var converterMethod = (kind switch
        {
            CollectionKind.Array => typeof(ConverterFactory).GetMethod(nameof(ArrayConverter), BindingFlags.NonPublic | BindingFlags.Static),
            CollectionKind.HashSet => typeof(ConverterFactory).GetMethod(nameof(HashSetConverter), BindingFlags.NonPublic | BindingFlags.Static),
            _ => typeof(ConverterFactory).GetMethod(nameof(ListConverter), BindingFlags.NonPublic | BindingFlags.Static),
        })!;

        var closed = converterMethod.MakeGenericMethod(sourceType, destinationType, sourceItem, destinationTarget.ItemType);
        return (Delegate)closed.Invoke(null, new[] { (object?)elementConverter })!;
    }

    private static Type? GetEnumerableElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetArrayRank() == 1 ? type.GetElementType() : null;
        }

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(Dictionary<,>))
            {
                return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
            }

            if (definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return typeof(object); // non-generic collections map to object elements
        }

        return null;
    }

    private static CollectionTarget? GetCollectionTarget(Type destinationType)
    {
        if (destinationType.IsArray)
        {
            return destinationType.GetArrayRank() == 1
                ? new CollectionTarget(CollectionKind.Array, destinationType.GetElementType()!)
                : null;
        }

        if (destinationType.IsGenericType)
        {
            var definition = destinationType.GetGenericTypeDefinition();
            var arguments = destinationType.GetGenericArguments();

            if (definition == typeof(Dictionary<,>))
            {
                return new CollectionTarget(CollectionKind.Dictionary, arguments[1], arguments[0]);
            }

            if (definition == typeof(List<>))
            {
                return new CollectionTarget(CollectionKind.List, arguments[0]);
            }

            if (definition == typeof(HashSet<>))
            {
                return new CollectionTarget(CollectionKind.HashSet, arguments[0]);
            }
        }

        foreach (var interfaceType in destinationType.GetInterfaces())
        {
            if (!interfaceType.IsGenericType)
            {
                continue;
            }

            var definition = interfaceType.GetGenericTypeDefinition();

            if (definition == typeof(IDictionary<,>))
            {
                var arguments = interfaceType.GetGenericArguments();
                return new CollectionTarget(CollectionKind.Dictionary, arguments[1], arguments[0]);
            }

            if (definition == typeof(IReadOnlyList<>) || definition == typeof(IReadOnlyCollection<>) ||
                definition == typeof(ICollection<>) || definition == typeof(IEnumerable<>))
            {
                return new CollectionTarget(CollectionKind.Enumerable, interfaceType.GetGenericArguments()[0]);
            }
        }

        return null;
    }

    private static Type? GetDictionaryValueArgument(Type type, int index)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            return type.GetGenericArguments()[index];
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                return interfaceType.GetGenericArguments()[index];
            }
        }

        return null;
    }

    private sealed class CollectionTarget
    {
        public CollectionTarget(CollectionKind kind, Type itemType, Type? keyType = null)
        {
            Kind = kind;
            ItemType = itemType;
            KeyType = keyType;
        }

        public CollectionKind Kind { get; }
        public Type ItemType { get; }
        public Type? KeyType { get; }
    }

    private static Func<TSrcValue, TDestValue> ListConverter<TSrcValue, TDestValue, TSrcItem, TDestItem>(Delegate? elementConverter)
    {
        var element = (Func<TSrcItem, TDestItem>?)elementConverter;

        return src =>
        {
            if (src is null)
            {
                return default!;
            }

            var result = new List<TDestItem>();

            if (element is null)
            {
                // identity: TSrcItem == TDestItem at runtime — same closed generic, no boxing
                foreach (var item in (IEnumerable<TDestItem>)src)
                {
                    result.Add(item);
                }
            }
            else
            {
                foreach (var item in (IEnumerable<TSrcItem>)src)
                {
                    result.Add(element(item));
                }
            }

            return (TDestValue)(object)result;
        };
    }

    private static Func<TSrcValue, TDestValue> ArrayConverter<TSrcValue, TDestValue, TSrcItem, TDestItem>(Delegate? elementConverter)
    {
        var element = (Func<TSrcItem, TDestItem>?)elementConverter;

        return src =>
        {
            if (src is null)
            {
                return default!;
            }

            var result = new List<TDestItem>();

            if (element is null)
            {
                foreach (var item in (IEnumerable<TDestItem>)src)
                {
                    result.Add(item);
                }
            }
            else
            {
                foreach (var item in (IEnumerable<TSrcItem>)src)
                {
                    result.Add(element(item));
                }
            }

            return (TDestValue)(object)result.ToArray();
        };
    }

    private static Func<TSrcValue, TDestValue> HashSetConverter<TSrcValue, TDestValue, TSrcItem, TDestItem>(Delegate? elementConverter)
    {
        var element = (Func<TSrcItem, TDestItem>?)elementConverter;

        return src =>
        {
            if (src is null)
            {
                return default!;
            }

            var result = new HashSet<TDestItem>();

            if (element is null)
            {
                foreach (var item in (IEnumerable<TDestItem>)src)
                {
                    result.Add(item);
                }
            }
            else
            {
                foreach (var item in (IEnumerable<TSrcItem>)src)
                {
                    result.Add(element(item));
                }
            }

            return (TDestValue)(object)result;
        };
    }

    private static Func<TSrcValue, TDestValue> DictionaryConverter<TSrcValue, TDestValue, TKeySource, TValueSource, TKey, TValue>(
        Delegate? keyConverter, Delegate? valueConverter)
        where TKey : notnull
    {
        var convertKey = (Func<TKeySource, TKey>?)keyConverter;
        var convertValue = (Func<TValueSource, TValue>?)valueConverter;

        return src =>
        {
            if (src is null)
            {
                return default!;
            }

            var result = new Dictionary<TKey, TValue>();

            if (convertKey is null && convertValue is null)
            {
                // identity: TKeySource == TKey and TValueSource == TValue at runtime
                foreach (var pair in (IEnumerable<KeyValuePair<TKey, TValue>>)src)
                {
                    result.Add(pair.Key, pair.Value);
                }
            }
            else
            {
                foreach (var pair in (IEnumerable<KeyValuePair<TKeySource, TValueSource>>)src)
                {
                    // identity side: TKeySource == TKey / TValueSource == TValue at runtime (cast via object)
                    var key = convertKey is null ? (TKey)(object)pair.Key! : convertKey(pair.Key);
                    var value = convertValue is null ? (TValue)(object)pair.Value! : convertValue(pair.Value);
                    result.Add(key, value);
                }
            }

            return (TDestValue)(object)result;
        };
    }

    // ------------------------------------------------------------------
    // Nested objects
    // ------------------------------------------------------------------

    private static Func<TSrcValue, TDestValue> NestedConverter<TSrcValue, TDestValue>(MapperConfiguration configuration)
    {
        IMappingPlan? cachedPlan = null;

        return value =>
        {
            if (value is null)
            {
                return default!;
            }

            cachedPlan ??= configuration.GetOrSynthesizePlan(typeof(TSrcValue), typeof(TDestValue));
            return (TDestValue)cachedPlan.MapObject(value, null, null)!;
        };
    }

    /// <summary>
    /// Context-aware nested mapping used directly by member bindings: consults the instance cache
    /// (circular references, cached before population), enforces MaxDepth and passes the shared
    /// context to the nested plan.
    /// </summary>
    public static Func<TSource, TSourceValue, MappingContext?, TDestValue> CreateNestedTransform<TSource, TSourceValue, TDestValue>(
        MapperConfiguration configuration)
    {
        IMappingPlan? cachedPlan = null;

        return (source, value, context) =>
        {
            if (value is null)
            {
                return default!;
            }

            cachedPlan ??= configuration.GetOrSynthesizePlan(typeof(TSourceValue), typeof(TDestValue));
            var plan = cachedPlan;

            if (context is null)
            {
                return (TDestValue)plan.MapObject(value, null, null)!;
            }

            if (context.TryGetCreated(typeof(TSourceValue), typeof(TDestValue), value, out var existing))
            {
                return (TDestValue)existing!;
            }

            context.Depth++;

            if (context.Depth >= context.MaxDepth)
            {
                context.Depth--;
                return default!; // MaxDepth: nested levels beyond the limit stay at their default value
            }

            try
            {
                // cache the destination BEFORE populating it (circular references terminate correctly)
                var destinationInstance = plan.CreateDestinationObject(value, context);
                context.CacheCreated(typeof(TSourceValue), typeof(TDestValue), value, destinationInstance);
                plan.PopulateObject(value, destinationInstance, context);
                return (TDestValue)destinationInstance;
            }
            finally
            {
                context.Depth--;
            }
        };
    }
}
