using System;

namespace XForge.DesignSystem.Components.Base;

/// <summary>
/// Categorizes a component for grouping in designer and docs.
/// Inspired by MudBlazor T013: every component declares a Category.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class XFCategoryAttribute : Attribute
{
    public XFCategoryAttribute(string category)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
    }

    public string Category { get; }

    public string? SubCategory { get; set; }
}

/// <summary>
/// Predefined component categories (MudBlazor-aligned).
/// </summary>
public static class XFCategories
{
    public const string Layout = "Layout";
    public const string Inputs = "Inputs";
    public const string DataDisplay = "Data Display";
    public const string Navigation = "Navigation";
    public const string Feedback = "Feedback";
    public const string Surfaces = "Surfaces";
    public const string Utilities = "Utilities";
    public const string AI = "AI";
    public const string Enterprise = "Enterprise";
}