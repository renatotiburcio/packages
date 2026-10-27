using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Image
{
    public partial class XFImage : XForgeComponentBase
    {
        protected override string BaseClass => "xf-image";

        [Parameter] public string? Src { get; set; }
        [Parameter] public string? FallbackSrc { get; set; }
        [Parameter] public string? Alt { get; set; }
        [Parameter] public bool Decorative { get; set; }
        [Parameter] public bool Fluid { get; set; } = true;
        [Parameter] public bool Lazy { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string Decoding { get; set; } = "async";
        [Parameter] public XFImageAspectRatio AspectRatio { get; set; } = XFImageAspectRatio.Auto;
        [Parameter] public XFImageFit Fit { get; set; } = XFImageFit.Cover;
        [Parameter] public XFImageRadius Radius { get; set; } = XFImageRadius.Lg;
        [Parameter] public RenderFragment? FallbackContent { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public string DefaultAltText { get; set; } = "Imagem";
        [Parameter] public string UnavailableLabel { get; set; } = "Imagem indisponivel";
        [Parameter] public string FallbackText { get; set; } = "Imagem";

        private bool primaryFailed;
        private bool fallbackFailed;

        private bool IsInteractive => OnClick.HasDelegate && !Disabled;

        private bool ShouldRenderImage => !string.IsNullOrWhiteSpace(CurrentSource);

        private string? CurrentSource => !primaryFailed && !string.IsNullOrWhiteSpace(Src)
            ? Src
            : !fallbackFailed && !string.IsNullOrWhiteSpace(FallbackSrc)
                ? FallbackSrc
                : null;

        private string AltValue => Decorative ? string.Empty : Alt ?? DefaultAltText;

        private string? ImageRole => Decorative ? "presentation" : IsInteractive ? "button" : null;

        private string? AriaLabelValue => Decorative ? null : Alt ?? UnavailableLabel;

        private string? TabIndexValue => IsInteractive ? "0" : null;

        private string LoadingValue => Lazy ? "lazy" : "eager";

        protected override void OnParametersSet()
        {
            primaryFailed = false;
            fallbackFailed = false;
        }

        protected override string GetComponentClass()
        {
            var width = Fluid ? "w-full" : "inline-block";
            return GetClass($"{BaseClass} relative overflow-hidden bg-surface-50 text-text-muted ring-1 ring-border-light {width} {AspectRatioClass} {RadiusClass}");
        }

        private string ImageClass => $"block h-full {ImageWidthClass} {FitClass} {InteractiveClass}";

        private string FallbackClass => $"flex h-full min-h-24 w-full items-center justify-center bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] text-primary {InteractiveClass}";

        private string ImageWidthClass => Fluid ? "w-full" : "";

        private string InteractiveClass => IsInteractive
            ? "cursor-pointer transition duration-200 ease-out hover:brightness-[0.98] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_28%,transparent)] focus-visible:ring-offset-2 focus-visible:ring-offset-bg-surface"
            : "";

        private string AspectRatioClass => AspectRatio switch
        {
            XFImageAspectRatio.Square => "aspect-square",
            XFImageAspectRatio.Video => "aspect-video",
            XFImageAspectRatio.Wide => "aspect-[21/9]",
            XFImageAspectRatio.Portrait => "aspect-[3/4]",
            _ => ""
        };

        private string FitClass => Fit switch
        {
            XFImageFit.Contain => "object-contain",
            XFImageFit.Fill => "object-fill",
            XFImageFit.None => "object-none",
            _ => "object-cover"
        };

        private string RadiusClass => Radius switch
        {
            XFImageRadius.None => "rounded-none",
            XFImageRadius.Sm => "rounded-md",
            XFImageRadius.Md => "rounded-lg",
            XFImageRadius.Xl => "rounded-2xl",
            XFImageRadius.Full => "rounded-full",
            _ => "rounded-xl"
        };

        private Task HandleImageError()
        {
            if (!primaryFailed && !string.IsNullOrWhiteSpace(Src))
            {
                primaryFailed = true;
            }
            else
            {
                fallbackFailed = true;
            }

            return Task.CompletedTask;
        }

        private Task HandleClickAsync(MouseEventArgs args)
        {
            return IsInteractive ? OnClick.InvokeAsync(args) : Task.CompletedTask;
        }

        private Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (!IsInteractive)
            {
                return Task.CompletedTask;
            }

            return args.Key is "Enter" or " "
                ? OnClick.InvokeAsync(new MouseEventArgs())
                : Task.CompletedTask;
        }
    }
}
