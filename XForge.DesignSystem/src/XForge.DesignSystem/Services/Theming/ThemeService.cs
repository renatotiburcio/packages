using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace XForge.DesignSystem.Services.Theming;
public enum XTheme { Light, Dark, Auto, HighContrast }
public record XThemeColors(string Primary, string Secondary, string Background, string Foreground, string Accent);
public interface IXThemeService
{
    XTheme Current { get; }
    event Action<XTheme>? OnThemeChanged;
    Task SetThemeAsync(XTheme theme);
    XThemeColors GetColors(XTheme theme);
    IReadOnlyList<XTheme> AvailableThemes { get; }
}
public abstract class ThemeServiceBase : IXThemeService
{
    public abstract XTheme Current { get; }
    public event Action<XTheme>? OnThemeChanged;
    public abstract Task SetThemeAsync(XTheme theme);
    public virtual XThemeColors GetColors(XTheme theme) => theme switch { XTheme.Light => new XThemeColors("#0066cc","#6c757d","#ffffff","#212529","#0d6efd"), XTheme.Dark => new XThemeColors("#3ea6ff","#adb5bd","#121212","#f8f9fa","#0a84ff"), XTheme.HighContrast => new XThemeColors("#ffff00","#ffffff","#000000","#ffffff","#ffff00"), _ => new XThemeColors("#0066cc","#6c757d","#ffffff","#212529","#0d6efd") };
    public virtual IReadOnlyList<XTheme> AvailableThemes => new[] { XTheme.Light, XTheme.Dark, XTheme.Auto, XTheme.HighContrast };
    protected void RaiseChanged(XTheme t) => OnThemeChanged?.Invoke(t);
}
public class StaticThemeService : ThemeServiceBase
{
    private XTheme _current = XTheme.Light;
    public override XTheme Current => _current;
    public override Task SetThemeAsync(XTheme theme) { var prev = _current; _current = theme; if (prev != theme) RaiseChanged(theme); return Task.CompletedTask; }
}
public class CascadingThemeService : ThemeServiceBase
{
    private readonly HashSet<ThemeServiceBase> _children = new();
    private XTheme _current = XTheme.Light;
    public override XTheme Current => _current;
    public override Task SetThemeAsync(XTheme theme) { _current = theme; RaiseChanged(theme); foreach (var c in _children) c.SetThemeAsync(theme); return Task.CompletedTask; }
    public void RegisterChild(ThemeServiceBase child) { _children.Add(child); child.OnThemeChanged += _ => { }; }
}