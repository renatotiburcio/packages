using System.Globalization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Layout
{
public enum XFAlignItems
    {
        Start,
        Center,
        End,
        Stretch,
        Baseline,
        Normal
    }

    public enum XFJustifyContent
    {
        Start,
        End,
        Center,
        SpaceBetween,
        SpaceAround,
        SpaceEvenly,
        Normal
    }

    public enum XFWrap
    {
        NoWrap,
        Wrap,
        WrapReverse
    }

    public enum XFGridFlow
    {
        Row,
        Column,
        Dense,
        RowDense,
        ColumnDense
    }

    public enum XFGridItems
    {
        Start,
        Center,
        End,
        Stretch,
        Normal
    }

    public readonly struct XFSizeToken
    {
        public string Value { get; }

        public XFSizeToken(string value)
        {
            Value = value;
        }

        public static implicit operator XFSizeToken(string value) => new XFSizeToken(value);
        public static implicit operator XFSizeToken(int value) => new XFSizeToken(value.ToString());
        public static implicit operator XFSizeToken(double value) => new XFSizeToken(value.ToString(CultureInfo.InvariantCulture));
    }
}
