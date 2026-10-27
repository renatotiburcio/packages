using System.Reflection;

namespace XForge.DesignSystem.Docs.QualityTests.Infrastructure;

/// <summary>
/// Discovers all public Blazor components in XForge.DesignSystem by scanning
/// the compiled assembly for types inheriting ComponentBase with [Parameter] properties.
/// </summary>
public static class ComponentApiCatalog
{
    private static readonly Lazy<IReadOnlyList<ComponentDescriptor>> _components = new(DiscoverComponents);

    public static IReadOnlyList<ComponentDescriptor> All => _components.Value;

    public static ComponentDescriptor? FindByName(string name)
    {
        var exact = All.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (exact != null) return exact;

        var stripped = StripGenericArity(name);
        return All.FirstOrDefault(c => StripGenericArity(c.Name).Equals(stripped, StringComparison.OrdinalIgnoreCase));
    }

    private static string StripGenericArity(string name)
    {
        var idx = name.IndexOf('`');
        return idx >= 0 ? name[..idx] : name;
    }

    private static List<ComponentDescriptor> DiscoverComponents()
    {
        var assembly = Assembly.GetAssembly(typeof(XForge.DesignSystem.Core.Base.XForgeComponentBase))!;
        var componentBaseType = typeof(Microsoft.AspNetCore.Components.ComponentBase);
        var results = new List<ComponentDescriptor>();

        foreach (var type in assembly.GetExportedTypes()
            .Where(t => componentBaseType.IsAssignableFrom(t) && t.Name.StartsWith("XF"))
            .OrderBy(t => t.Name))
        {
            results.Add(new ComponentDescriptor
            {
                Name = type.Name,
                FullName = type.FullName!,
                Namespace = type.Namespace!,
                Category = InferCategory(type),
                Parameters = ExtractParameters(type),
                CascadingParameters = ExtractCascadingParameters(type),
                EventCallbacks = ExtractEventCallbacks(type),
                RenderFragments = ExtractRenderFragments(type),
                PublicMethods = ExtractPublicMethods(type),
                RelatedEnums = ExtractRelatedEnums(type, assembly),
                RelatedTypes = ExtractRelatedTypes(type, assembly),
                HasIdisposable = typeof(IDisposable).IsAssignableFrom(type),
                HasIAsyncDisposable = typeof(IAsyncDisposable).IsAssignableFrom(type),
            });
        }
        return results;
    }

    private static List<ParameterDescriptor> ExtractParameters(Type type)
    {
        var results = new List<ParameterDescriptor>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<Microsoft.AspNetCore.Components.ParameterAttribute>();
            if (attr is null) continue;
            results.Add(new ParameterDescriptor
            {
                Name = prop.Name,
                TypeName = FormatTypeName(prop.PropertyType),
                IsRequired = prop.GetCustomAttribute<Microsoft.AspNetCore.Components.EditorRequiredAttribute>() != null,
                HasUnmatchedValues = attr.CaptureUnmatchedValues,
                IsRenderFragment = prop.PropertyType.Name.Contains("RenderFragment"),
                IsEventCallback = prop.PropertyType.Name.Contains("EventCallback"),
            });
        }
        return results;
    }

    private static List<CascadingParameterDescriptor> ExtractCascadingParameters(Type type)
    {
        var results = new List<CascadingParameterDescriptor>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<Microsoft.AspNetCore.Components.CascadingParameterAttribute>();
            if (attr is null) continue;
            results.Add(new CascadingParameterDescriptor
            {
                Name = prop.Name,
                TypeName = FormatTypeName(prop.PropertyType),
                IsPublic = prop.GetGetMethod(true)?.IsPublic ?? false,
            });
        }
        return results;
    }

    private static List<EventCallbackDescriptor> ExtractEventCallbacks(Type type)
    {
        var results = new List<EventCallbackDescriptor>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.PropertyType.Name.Contains("EventCallback")) continue;
            if (prop.GetCustomAttribute<Microsoft.AspNetCore.Components.ParameterAttribute>() == null) continue;
            var typeArg = prop.PropertyType.IsGenericType
                ? FormatTypeName(prop.PropertyType.GetGenericArguments()[0])
                : null;
            results.Add(new EventCallbackDescriptor { Name = prop.Name, ValueType = typeArg });
        }
        return results;
    }

    private static List<RenderFragmentDescriptor> ExtractRenderFragments(Type type)
    {
        var results = new List<RenderFragmentDescriptor>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.PropertyType.Name.Contains("RenderFragment")) continue;
            if (prop.GetCustomAttribute<Microsoft.AspNetCore.Components.ParameterAttribute>() == null) continue;
            var isTyped = prop.PropertyType.IsGenericType;
            results.Add(new RenderFragmentDescriptor
            {
                Name = prop.Name,
                IsTyped = isTyped,
                ContextType = isTyped ? FormatTypeName(prop.PropertyType.GetGenericArguments()[0]) : null,
            });
        }
        return results;
    }

    private static List<PublicMethodDescriptor> ExtractPublicMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName && m.DeclaringType == type)
            .Select(m => new PublicMethodDescriptor
            {
                Name = m.Name,
                ReturnType = FormatTypeName(m.ReturnType),
                Parameters = m.GetParameters().Select(p => $"{FormatTypeName(p.ParameterType)} {p.Name}").ToArray(),
            }).ToList();
    }

    private static List<EnumDescriptor> ExtractRelatedEnums(Type type, Assembly assembly)
    {
        var results = new List<EnumDescriptor>();
        var paramTypes = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<Microsoft.AspNetCore.Components.ParameterAttribute>() != null)
            .Select(p => p.PropertyType).Distinct();
        foreach (var pt in paramTypes)
        {
            var actualType = Nullable.GetUnderlyingType(pt) ?? pt;
            if (!actualType.IsEnum) continue;
            if (results.Any(r => r.Name == actualType.Name)) continue;
            results.Add(new EnumDescriptor { Name = actualType.Name, Values = Enum.GetNames(actualType).ToList() });
        }
        return results;
    }

    private static List<string> ExtractRelatedTypes(Type type, Assembly assembly)
    {
        var results = new List<string>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetCustomAttribute<Microsoft.AspNetCore.Components.ParameterAttribute>() == null) continue;
            var pt = prop.PropertyType;
            if (pt.IsGenericType)
            {
                foreach (var ga in pt.GetGenericArguments())
                    if (ga.Assembly == assembly && !ga.IsEnum && !ga.IsPrimitive && ga.Namespace?.Contains("XForge") == true)
                        results.Add(ga.Name);
            }
            if (pt.Assembly == assembly && !pt.IsEnum && !pt.IsPrimitive && pt.Namespace?.Contains("XForge") == true)
                results.Add(pt.Name);
        }
        return results.Distinct().ToList();
    }

    private static string FormatTypeName(Type t)
    {
        if (t == typeof(string)) return "string";
        if (t == typeof(bool)) return "bool";
        if (t == typeof(int)) return "int";
        if (t == typeof(double)) return "double";
        if (t == typeof(float)) return "float";
        if (t == typeof(decimal)) return "decimal";
        if (t == typeof(object)) return "object";
        if (t == typeof(void)) return "void";
        var name = t.Name;
        if (t.IsGenericType)
        {
            var baseName = name.Contains('`') ? name[..name.IndexOf('`')] : name;
            var args = t.GetGenericArguments().Select(FormatTypeName);
            return $"{baseName}<{string.Join(", ", args)}>";
        }
        return name;
    }

    private static string InferCategory(Type type)
    {
        var ns = type.Namespace ?? "";
        if (ns.Contains(".AI.")) return "AI";
        if (ns.Contains(".Button")) return "Button";
        if (ns.Contains(".DataDisplay")) return "DataDisplay";
        if (ns.Contains(".Feedback")) return "Feedback";
        if (ns.Contains(".Forms")) return "Forms";
        if (ns.Contains(".Layout")) return "Layout";
        if (ns.Contains(".Navigation")) return "Navigation";
        if (ns.Contains(".Overlays")) return "Overlays";
        if (ns.Contains(".Typography")) return "Typography";
        if (ns.Contains(".Utilities")) return "Utilities";
        if (ns.Contains(".Shared")) return "Shared";
        if (ns.Contains(".CrossPlatform")) return "CrossPlatform";
        return "Other";
    }
}
public sealed record ComponentDescriptor
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public required string Namespace { get; init; }
    public required string Category { get; init; }
    public required List<ParameterDescriptor> Parameters { get; init; }
    public required List<CascadingParameterDescriptor> CascadingParameters { get; init; }
    public required List<EventCallbackDescriptor> EventCallbacks { get; init; }
    public required List<RenderFragmentDescriptor> RenderFragments { get; init; }
    public required List<PublicMethodDescriptor> PublicMethods { get; init; }
    public required List<EnumDescriptor> RelatedEnums { get; init; }
    public required List<string> RelatedTypes { get; init; }
    public required bool HasIdisposable { get; init; }
    public required bool HasIAsyncDisposable { get; init; }
}

public sealed record ParameterDescriptor
{
    public required string Name { get; init; }
    public required string TypeName { get; init; }
    public required bool IsRequired { get; init; }
    public required bool HasUnmatchedValues { get; init; }
    public required bool IsRenderFragment { get; init; }
    public required bool IsEventCallback { get; init; }
}

public sealed record CascadingParameterDescriptor
{
    public required string Name { get; init; }
    public required string TypeName { get; init; }
    public required bool IsPublic { get; init; }
}

public sealed record EventCallbackDescriptor
{
    public required string Name { get; init; }
    public string? ValueType { get; init; }
}

public sealed record RenderFragmentDescriptor
{
    public required string Name { get; init; }
    public required bool IsTyped { get; init; }
    public string? ContextType { get; init; }
}

public sealed record PublicMethodDescriptor
{
    public required string Name { get; init; }
    public required string ReturnType { get; init; }
    public required string[] Parameters { get; init; }
}

public sealed record EnumDescriptor
{
    public required string Name { get; init; }
    public required List<string> Values { get; init; }
}
