using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Layout.Splitter
{
    public sealed record XFSplitterChangedEventArgs(double PrimarySize, double SecondarySize, XFOrientation Orientation);
}
