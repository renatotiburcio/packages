using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
namespace XForge.DesignSystem.Components.Localization;
public interface IComponentLocalizer { string this[string key] { get; } }
public class ComponentLocalizer<T> : IComponentLocalizer where T : class
{
    private readonly ResourceManager _rm;
    private readonly CultureInfo _culture;
    public ComponentLocalizer(CultureInfo? culture = null) { _rm = new ResourceManager(typeof(T).FullName!, typeof(T).Assembly); _culture = culture ?? CultureInfo.CurrentUICulture; }
    public string this[string key] { get { try { return _rm.GetString(key, _culture) ?? key; } catch (MissingManifestResourceException) { return key; } } }
    public string GetString(string key, params object[] args) { var s = this[key]; return args.Length == 0 ? s : string.Format(_culture, s, args); }
    public IEnumerable<CultureInfo> SupportedCultures => CultureInfo.GetCultures(CultureTypes.SpecificCultures);
}