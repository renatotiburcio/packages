using System.Collections.Generic;

namespace XForge.DesignSystem.Components;

public interface IXFComponentDiscovery
{
    IEnumerable<XFComponentInfo> GetAll();
    XFComponentInfo? GetByName(string name);
    IEnumerable<XFComponentInfo> GetByCategory(string category);
}