using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Docs.Shared.Components.Overview;

public sealed record XForgeCategoryOverviewItem(string Title, string Description, string Icon);

public sealed record XForgeCategoryOverviewComponentRow(
    string Component,
    string Purpose,
    string MainUse,
    string EnterpriseUse,
    string Accessibility,
    string Complexity);

public sealed record XForgeCategoryOverviewNameDescription(string Name, string Description);

public sealed record XForgeCategoryOverviewExample(
    string Title,
    string Description,
    RenderFragment Preview,
    string Code);
