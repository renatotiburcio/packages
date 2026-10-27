using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Code-behind for <see cref="XFFluentIcon{TIcon}"/>. Resolves the icon from
/// <see cref="XFIconCatalog"/> using the generic TIcon type name as the lookup key.
/// </summary>
public partial class XFFluentIcon<TIcon> : ComponentBase where TIcon : class
{
}

/// <summary>Reserved icon color tokens. CustomColor is only valid when Color=Custom.</summary>
public enum XFIconColor
{
    Accent, Lightweight, Success, Warning, Error, Important, Custom
}