using XForge.Localization;

namespace XForge.DesignSystem.Tests;

public sealed class TestLocalizer : IXFLocalizer
{
    public string this[string key] => key;

    public string Format(string key, params object[] args)
    {
        var value = this[key];
        return args.Length == 0 ? value : string.Format(value, args);
    }
}
