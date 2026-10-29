using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Skeleton
{
    public partial class XFSkeleton : XForgeComponentBase
    {
        protected override string BaseClass => "xf-skeleton";

        [Parameter] public XFSkeletonShape Shape { get; set; } = XFSkeletonShape.Text;
        [Parameter] public XFSkeletonAnimation Animation { get; set; } = XFSkeletonAnimation.Pulse;
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Decorative { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string DefaultAriaLabel { get; set; } = "Carregando conteudo";
        [Parameter] public string? WidthClass { get; set; }
        [Parameter] public string? HeightClass { get; set; }
        [Parameter] public string? RoundedClass { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Shape, Animation, Size, Visible, Decorative, AriaLabel, DefaultAriaLabel, WidthClass, HeightClass, RoundedClass);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string? Role => Decorative ? null : "status";

        private string? AriaHidden => Decorative ? "true" : null;

        private string? ResolvedAriaLabel => Decorative ? null : AriaLabel ?? DefaultAriaLabel;

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {BaseShapeClass} {SizeClass} {AnimationClass} {CustomSizeClass}");
        }

        private string BaseShapeClass => Shape switch
        {
            XFSkeletonShape.Circle => "relative inline-block shrink-0 overflow-hidden rounded-full bg-surface-200",
            XFSkeletonShape.Avatar => "relative inline-block shrink-0 overflow-hidden rounded-full bg-surface-200",
            XFSkeletonShape.Rectangle => "relative block overflow-hidden rounded-lg bg-surface-200",
            XFSkeletonShape.Custom => "relative block overflow-hidden bg-surface-200",
            _ => "relative block overflow-hidden rounded-full bg-surface-200"
        };

        private string SizeClass => Shape switch
        {
            XFSkeletonShape.Circle or XFSkeletonShape.Avatar => Size switch
            {
                XFExtendedSize.Xs => "h-6 w-6",
                XFExtendedSize.Sm => "h-8 w-8",
                XFExtendedSize.Lg => "h-14 w-14",
                XFExtendedSize.Xl => "h-20 w-20",
                _ => "h-10 w-10"
            },
            XFSkeletonShape.Rectangle => Size switch
            {
                XFExtendedSize.Xs => "h-10 w-full",
                XFExtendedSize.Sm => "h-16 w-full",
                XFExtendedSize.Lg => "h-32 w-full",
                XFExtendedSize.Xl => "h-48 w-full",
                _ => "h-24 w-full"
            },
            XFSkeletonShape.Custom => "",
            _ => Size switch
            {
                XFExtendedSize.Xs => "h-2 w-full",
                XFExtendedSize.Sm => "h-2.5 w-full",
                XFExtendedSize.Lg => "h-4 w-full",
                XFExtendedSize.Xl => "h-5 w-full",
                _ => "h-3 w-full"
            }
        };

        private string AnimationClass => Animation switch
        {
            XFSkeletonAnimation.Shimmer => "before:absolute before:inset-0 before:-translate-x-full before:bg-[linear-gradient(90deg,transparent,rgb(255_255_255/.42),transparent)] motion-safe:before:animate-pulse motion-reduce:before:hidden",
            XFSkeletonAnimation.Pulse => "motion-safe:animate-pulse motion-reduce:animate-none",
            _ => ""
        };

        private string CustomSizeClass
        {
            get
            {
                var classes = new List<string>();

                if (!string.IsNullOrWhiteSpace(WidthClass))
                {
                    classes.Add(WidthClass);
                }

                if (!string.IsNullOrWhiteSpace(HeightClass))
                {
                    classes.Add(HeightClass);
                }

                if (!string.IsNullOrWhiteSpace(RoundedClass))
                {
                    classes.Add(RoundedClass);
                }

                return string.Join(" ", classes);
            }
        }
    }
}
