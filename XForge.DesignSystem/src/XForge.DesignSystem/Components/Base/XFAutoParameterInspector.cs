using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Base;

/// <summary>
/// Detects [Parameter] auto-properties at runtime to enable metadata introspection.
/// Inspired by MudBlazor T012: avoid manual getter/setter boilerplate.
/// </summary>
public static class XFAutoParameterInspector
{
    public static IReadOnlyList<string> GetParameterNames([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type componentType)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));
        var props = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return props
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p => p.Name)
            .ToList();
    }

    public static IReadOnlyList<PropertyInfo> GetParameters([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type componentType)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));
        var props = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return props.Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null).ToList();
    }
}