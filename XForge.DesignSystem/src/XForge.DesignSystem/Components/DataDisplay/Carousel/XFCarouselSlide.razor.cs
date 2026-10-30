using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Carousel
{
    public partial class XFCarouselSlide : XForgeComponentBase, IDisposable
    {
        [CascadingParameter] private XFCarousel? ParentCarousel { get; set; }

        [Parameter] public string? SlideId { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', SlideId, Label, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private bool registered;
        private int Index => ParentCarousel?.GetSlideIndex(this) ?? -1;
        private bool IsActive => ParentCarousel?.CurrentIndex == Index;

        private string SlideClass
        {
            get
            {
                var transition = ParentCarouselTransition;

                return transition switch
                {
                    XFCarouselTransition.Fade => Join("absolute inset-0 transition-opacity duration-500 ease-out", IsActive ? "z-10 opacity-100" : "z-0 opacity-0 pointer-events-none", Class),
                    XFCarouselTransition.None => Join(IsActive ? "block" : "hidden", Class),
                    _ => Join("min-w-full shrink-0", Class)
                };
            }
        }

        private XFCarouselTransition ParentCarouselTransition
        {
            get
            {
                return ParentCarousel?.Transition ?? XFCarouselTransition.Slide;
            }
        }

        protected override void OnInitialized()
        {
            ParentCarousel?.Register(this);
            registered = ParentCarousel is not null;
        }

        public void Dispose()
        {
            if (registered)
            {
                ParentCarousel?.Unregister(this);
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
