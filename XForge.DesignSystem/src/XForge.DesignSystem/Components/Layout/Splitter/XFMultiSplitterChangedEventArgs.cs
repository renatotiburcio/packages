using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Layout.Splitter;

public sealed class XFMultiSplitterChangedEventArgs : EventArgs
{
    public XFMultiSplitterChangedEventArgs(IReadOnlyList<double> sizes, int handleIndex, XFOrientation orientation)
    {
        Sizes = sizes;
        HandleIndex = handleIndex;
        Orientation = orientation;
    }

    public IReadOnlyList<double> Sizes { get; }

    public int HandleIndex { get; }

    public XFOrientation Orientation { get; }
}
