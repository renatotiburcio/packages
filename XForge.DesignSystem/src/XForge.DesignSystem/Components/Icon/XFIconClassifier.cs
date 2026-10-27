using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Components.Icon;

public sealed class XFIconClassifier : IXFIconClassifier
{
    public XFIconCategory Classify(XFIconInfo info)
    {
        var haystack = (info.Name + " " + string.Join(' ', info.Keywords)).ToLowerInvariant();
        foreach (var (cat, kws) in XFIconCategoryRules.Rules)
            if (kws.Any(k => haystack.Contains(k))) return cat;
        return XFIconCategory.Other;
    }

    public IEnumerable<XFIconInfo> ByCategory(XFIconCategory cat) => System.Array.Empty<XFIconInfo>();
}