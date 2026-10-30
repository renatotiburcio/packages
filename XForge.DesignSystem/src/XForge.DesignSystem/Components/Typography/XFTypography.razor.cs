using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Typography
{
    public partial class XFTypography : XForgeComponentBase
    {
        protected override string BaseClass => "xf-typography";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFTypographyVariant Variant { get; set; } = XFTypographyVariant.Body1;
        [Parameter] public XFTypographyColor Color { get; set; } = XFTypographyColor.Default;
        [Parameter] public XFTypographyWeight Weight { get; set; } = XFTypographyWeight.Inherit;
        [Parameter] public XFTypographyAlign Align { get; set; } = XFTypographyAlign.Start;
        [Parameter] public XFTypographyTransform Transform { get; set; } = XFTypographyTransform.None;
        [Parameter] public string? As { get; set; }
        [Parameter] public string? Text { get; set; }
        [Parameter] public bool NoWrap { get; set; }
        [Parameter] public bool Truncate { get; set; }
        [Parameter] public bool GutterBottom { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, Variant, Color, Weight, Align, Transform, As, Text, NoWrap, Truncate, GutterBottom);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected string GetTagName()
        {
            if (!string.IsNullOrWhiteSpace(As))
            {
                return As;
            }

            return Variant switch
            {
                XFTypographyVariant.Display => "h1",
                XFTypographyVariant.Hero => "h1",
                XFTypographyVariant.H1 => "h1",
                XFTypographyVariant.H2 => "h2",
                XFTypographyVariant.H3 => "h3",
                XFTypographyVariant.H4 => "h4",
                XFTypographyVariant.H5 => "h5",
                XFTypographyVariant.H6 => "h6",
                XFTypographyVariant.Subtitle1 => "h6",
                XFTypographyVariant.Subtitle2 => "h6",
                XFTypographyVariant.Caption => "span",
                XFTypographyVariant.Overline => "span",
                XFTypographyVariant.Code => "code",
                _ => "p"
            };
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {GetVariantClass()} {GetColorClass()} {GetWeightClass()} {GetAlignClass()} {GetTransformClass()} {GetFlowClass()}");
        }

        protected Dictionary<string, object> GetElementAttributes()
        {
            var attributes = new Dictionary<string, object>();

            if (AdditionalAttributes != null)
            {
                foreach (var attr in AdditionalAttributes)
                {
                    attributes[attr.Key] = attr.Value;
                }
            }

            attributes["class"] = GetComponentClass();
            attributes["id"] = Id;

            if (!string.IsNullOrEmpty(Style))
            {
                attributes["style"] = Style;
            }

            return attributes;
        }

        protected RenderFragment? RenderedContent => builder =>
        {
            if (!string.IsNullOrEmpty(Text))
            {
                builder.AddContent(0, Text);
            }
            else
            {
                builder.AddContent(1, ChildContent);
            }
        };

        private string GetVariantClass()
        {
            return Variant switch
            {
                XFTypographyVariant.Display => "text-4xl font-black leading-tight text-text-primary sm:text-5xl",
                XFTypographyVariant.Hero => "text-4xl font-bold leading-tight text-text-primary",
                XFTypographyVariant.H1 => "text-4xl font-bold leading-tight text-text-primary",
                XFTypographyVariant.H2 => "text-3xl font-bold leading-tight text-text-primary",
                XFTypographyVariant.H3 => "text-2xl font-semibold leading-tight text-text-primary",
                XFTypographyVariant.H4 => "text-xl font-semibold leading-tight text-text-primary",
                XFTypographyVariant.H5 => "text-lg font-semibold leading-snug text-text-primary",
                XFTypographyVariant.H6 => "text-base font-semibold leading-snug text-text-primary",
                XFTypographyVariant.Subtitle1 => "text-lg leading-relaxed text-text-secondary",
                XFTypographyVariant.Subtitle2 => "text-base leading-relaxed text-text-secondary",
                XFTypographyVariant.Body2 => "text-sm leading-relaxed text-text-secondary",
                XFTypographyVariant.Caption => "text-xs leading-normal text-text-muted",
                XFTypographyVariant.Overline => "text-xs font-semibold uppercase tracking-wider leading-normal text-text-muted",
                XFTypographyVariant.Code => "font-mono text-sm leading-relaxed text-text-primary",
                _ => "text-base leading-relaxed text-text-primary"
            };
        }

        private string GetColorClass()
        {
            return Color switch
            {
                XFTypographyColor.Secondary => "text-text-secondary",
                XFTypographyColor.Muted => "text-text-muted",
                XFTypographyColor.Subtle => "text-text-subtle",
                XFTypographyColor.Inverse => "text-text-inverse",
                XFTypographyColor.Primary => "text-primary",
                XFTypographyColor.Success => "text-success",
                XFTypographyColor.Warning => "text-warning",
                XFTypographyColor.Danger => "text-danger",
                _ => ""
            };
        }

        private string GetWeightClass()
        {
            return Weight switch
            {
                XFTypographyWeight.Normal => "font-normal",
                XFTypographyWeight.Medium => "font-medium",
                XFTypographyWeight.Bold => "font-bold",
                XFTypographyWeight.Semibold => "font-semibold",
                XFTypographyWeight.Black => "font-black",
                _ => ""
            };
        }

        private string GetAlignClass()
        {
            return Align switch
            {
                XFTypographyAlign.Center => "text-center",
                XFTypographyAlign.End => "text-end",
                XFTypographyAlign.Justify => "text-justify",
                _ => "text-start"
            };
        }

        private string GetTransformClass()
        {
            return Transform switch
            {
                XFTypographyTransform.Uppercase => "uppercase",
                XFTypographyTransform.Lowercase => "lowercase",
                XFTypographyTransform.Capitalize => "capitalize",
                _ => ""
            };
        }

        private string GetFlowClass()
        {
            var classes = new List<string>();

            if (NoWrap)
                classes.Add("whitespace-nowrap");

            if (Truncate)
                classes.Add("truncate");

            if (GutterBottom)
                classes.Add("mb-2");

            return string.Join(" ", classes);
        }
    }
}
