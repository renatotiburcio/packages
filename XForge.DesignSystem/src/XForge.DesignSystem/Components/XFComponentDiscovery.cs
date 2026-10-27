using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Components;

public sealed class XFComponentDiscovery : IXFComponentDiscovery
{
    private readonly IReadOnlyList<XFComponentInfo> _all;

    public XFComponentDiscovery(IEnumerable<XFComponentInfo> components) => _all = components.ToList();

    public IEnumerable<XFComponentInfo> GetAll() => _all;

    public XFComponentInfo? GetByName(string name)
        => _all.FirstOrDefault(c => string.Equals(c.Name, name, System.StringComparison.OrdinalIgnoreCase));

    public IEnumerable<XFComponentInfo> GetByCategory(string category)
        => _all.Where(c => string.Equals(c.Category, category, System.StringComparison.OrdinalIgnoreCase));
}