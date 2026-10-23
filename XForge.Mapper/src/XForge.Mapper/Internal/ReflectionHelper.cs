using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>Reflection helpers for member discovery and typed delegate creation.</summary>
internal static class ReflectionHelper
{
    /// <summary>Gets the value type of a property or field.</summary>
    public static Type GetMemberType(this MemberInfo member)
    {
        return member switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            _ => throw new ArgumentException($"Member '{member.Name}' is not a property or field.", nameof(member)),
        };
    }

    /// <summary>Gets public instance readable properties + fields of a type.</summary>
    public static List<MemberInfo> GetReadableMembers(Type type)
    {
        var members = new List<MemberInfo>();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        foreach (var property in type.GetProperties(flags))
        {
            if (property.CanRead && property.GetIndexParameters().Length == 0)
            {
                members.Add(property);
            }
        }

        members.AddRange(type.GetFields(flags).Where(f => f.IsPublic));
        return members;
    }

    /// <summary>Gets public instance writable properties + fields of a type (init-only properties excluded).</summary>
    public static List<MemberInfo> GetWritableMembers(Type type)
    {
        var members = new List<MemberInfo>();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        foreach (var property in type.GetProperties(flags))
        {
            if (property.CanWrite && property.GetIndexParameters().Length == 0 && !IsInitOnly(property))
            {
                members.Add(property);
            }
        }

        members.AddRange(type.GetFields(flags).Where(f => f.IsPublic && !f.IsInitOnly));
        return members;
    }

    /// <summary>Detects init-only setters (records / <c>init</c> accessors) via the IsExternalInit modreq.</summary>
    public static bool IsInitOnly(PropertyInfo property)
    {
        var setter = property.SetMethod;

        if (setter is null)
        {
            return false;
        }

        return setter.ReturnParameter!.GetRequiredCustomModifiers()
            .Any(modifier => modifier.FullName == "System.Runtime.CompilerServices.IsExternalInit");
    }

    /// <summary>Creates a typed getter delegate for a property or field (compiled once per member).</summary>
    public static Func<TSource, TValue> CreateGetter<TSource, TValue>(MemberInfo member)
    {
        var parameter = Expression.Parameter(typeof(TSource), "source");
        Expression access = member switch
        {
            PropertyInfo property => Expression.Property(parameter, property),
            FieldInfo field => Expression.Field(parameter, field),
            _ => throw new ArgumentException($"Member '{member.Name}' is not a property or field.", nameof(member)),
        };

        if (access.Type != typeof(TValue))
        {
            access = Expression.Convert(access, typeof(TValue));
        }

        return Expression.Lambda<Func<TSource, TValue>>(access, parameter).Compile();
    }

    /// <summary>Creates a typed setter delegate for a writable property or field (compiled once per member).</summary>
    public static Action<TDestination, TValue> CreateSetter<TDestination, TValue>(MemberInfo member)
    {
        var instance = Expression.Parameter(typeof(TDestination), "destination");
        var value = Expression.Parameter(typeof(TValue), "value");

        Expression memberAccess = member switch
        {
            PropertyInfo property => Expression.Property(instance, property),
            FieldInfo field => Expression.Field(instance, field),
            _ => throw new ArgumentException($"Member '{member.Name}' is not a property or field.", nameof(member)),
        };

        Expression convertedValue = value.Type == memberAccess.Type
            ? value
            : (Expression)Expression.Convert(value, memberAccess.Type);

        var body = Expression.Assign(memberAccess, convertedValue);
        return Expression.Lambda<Action<TDestination, TValue>>(body, instance, value).Compile();
    }
}
