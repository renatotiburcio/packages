namespace XForge.Mediator;

/// <summary>
/// Excludes the decorated type from automatic registration when scanning assemblies with
/// <c>AddXForgeMediator</c> (handlers, behaviors, processors and exception actions/handlers).
/// Useful for test fixtures, duplicates or types registered manually elsewhere.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class XForgeMediatorIgnoreAttribute : Attribute
{
}
