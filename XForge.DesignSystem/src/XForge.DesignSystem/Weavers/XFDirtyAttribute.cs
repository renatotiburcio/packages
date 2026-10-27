using System.Diagnostics.CodeAnalysis;
using System.Reflection;
// T049: Fody Weaver for [Parameter] auto-Dirty() (Blazorise pattern).
// P1, 16h, depends T045 (ShouldAutoGenerateId on base).
// Note: Fody is build-time. We provide a manual equivalent via [XFDirty] + a weaver interface that
// documents the intended Roslyn-level rewrite.
namespace XForge.DesignSystem.Weavers;

/// <summary>
/// Mark a [Parameter] property to be tracked for dirty state. Used in tandem with Fody/IXFParameterDirtyWeaver.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class XFDirtyAttribute : Attribute
{
    /// <summary>If true, also propagate OnDirty event when value changes.</summary>
    public bool FireEvent { get; set; } = true;
}

/// <summary>Marker for a component that has parameters tracked by the weaver.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class XFDirtyTrackedAttribute : Attribute { }

/// <summary>Roslyn weaver abstraction (would be implemented as ISourceGenerator / Fody ModuleWeaver in production).</summary>
public interface IXFParameterDirtyWeaver
{
    /// <summary>Name of this weaver (e.g. "XFParameterDirty").</summary>
    string Name { get; }
    /// <summary>Indicates whether the given type carries [XFDirty] parameters.</summary>
    bool ShouldWeave(Type type);
    /// <summary>Returns the set of property names that should be tracked.</summary>
    IReadOnlyCollection<string> GetTrackedProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type);
}

/// <summary>Default weaver implementation (reflection-based; production should rewrite to direct setters).</summary>
public sealed class XFParameterDirtyWeaver : IXFParameterDirtyWeaver
{
    public string Name => "XFParameterDirty";
    public bool ShouldWeave(Type type) => type.GetCustomAttributes(typeof(XFDirtyTrackedAttribute), inherit: true).Any();
    public IReadOnlyCollection<string> GetTrackedProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttributes(typeof(XFDirtyAttribute), inherit: true).Any())
            .Select(p => p.Name)
            .ToArray();
}