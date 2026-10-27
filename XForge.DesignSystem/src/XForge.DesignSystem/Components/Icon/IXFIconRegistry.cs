using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

public interface IXFIconRegistry
{
    XFIconInfo? Get(string name, XFIconVariant variant, XFIconSize size);
    IEnumerable<XFIconInfo> Search(string keyword, int page = 0, int pageSize = 20);
    void Register(XFIconInfo info);
    int Count { get; }
}