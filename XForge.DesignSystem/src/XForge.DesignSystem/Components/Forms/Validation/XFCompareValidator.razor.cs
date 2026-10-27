using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Compares two field values using a specified operator.
    /// </summary>
    public partial class XFCompareValidator : XFValidatorBase
    {
        /// <summary>The value to compare against.</summary>
        [Parameter] public object? Value { get; set; }

        /// <summary>The comparison operator.</summary>
        [Parameter] public XFCompareOperator Operator { get; set; } = XFCompareOperator.Equal;

        protected override bool Validate(object? value)
        {
            if (value is null && Value is null)
                return Operator == XFCompareOperator.Equal;

            if (value is null || Value is null)
                return Operator == XFCompareOperator.NotEqual;

            if (value is IComparable comparable && Value is IComparable target)
            {
                var cmp = comparable.CompareTo(Convert.ChangeType(target, value.GetType()));
                return Operator switch
                {
                    XFCompareOperator.Equal => cmp == 0,
                    XFCompareOperator.NotEqual => cmp != 0,
                    XFCompareOperator.GreaterThan => cmp > 0,
                    XFCompareOperator.GreaterThanOrEqual => cmp >= 0,
                    XFCompareOperator.LessThan => cmp < 0,
                    XFCompareOperator.LessThanOrEqual => cmp <= 0,
                    _ => true
                };
            }

            return Operator switch
            {
                XFCompareOperator.Equal => Equals(value, Value),
                XFCompareOperator.NotEqual => !Equals(value, Value),
                _ => true
            };
        }
    }

    public enum XFCompareOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
}
