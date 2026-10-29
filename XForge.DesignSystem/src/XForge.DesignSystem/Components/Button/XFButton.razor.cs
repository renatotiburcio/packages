using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Windows.Input;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Button
{
    public partial class XFButton : XForgeComponentBase, IDisposable
    {
        protected override string BaseClass => "xf-button";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFButtonVariant Variant { get; set; } = XFButtonVariant.Solid;
        [Parameter] public XFButtonColor Color { get; set; } = XFButtonColor.Primary;
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
        [Parameter] public XFButtonType Type { get; set; } = XFButtonType.Button;
        [Parameter] public XFButtonShape Shape { get; set; } = XFButtonShape.Auto;
        [Parameter] public string? Text { get; set; }
        [Parameter] public string? LoadingText { get; set; }
        [Parameter] public RenderFragment? Icon { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public RenderFragment? LoadingContent { get; set; }
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public bool Active { get; set; }
        [Parameter] public bool Pressed { get; set; }
        [Parameter] public bool UseFormState { get; set; }
        [Parameter] public bool DisableWhenFormInvalid { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public bool Autofocus { get; set; }
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public string? Form { get; set; }
        [Parameter] public string? FormAction { get; set; }
        [Parameter] public string? FormEnctype { get; set; }
        [Parameter] public string? FormMethod { get; set; }
        [Parameter] public bool? FormNoValidate { get; set; }
        [Parameter] public string? FormTarget { get; set; }
        [Parameter] public ICommand? Command { get; set; }
        [Parameter] public object? CommandParameter { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        // assinatura de render: botões são o elemento mais frequente em UI — evita re-render
        // redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, Variant, Color, Size, Type, Shape, Text, LoadingText, Disabled, Loading, Icon, FullWidth);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [CascadingParameter] private XFEditFormContext? FormContext { get; set; }

        private bool _clicking;
        private ICommand? _boundCommand;
        private bool IsFormSubmitting => UseFormState && FormContext?.Submitting == true;
        private bool IsFormActionBlocked => DisableWhenFormInvalid && FormContext?.ActionsBlocked == true;
        private bool EffectiveLoading => Loading || IsFormSubmitting;
        private bool IsDisabled => Disabled || EffectiveLoading || IsFormActionBlocked || CommandCanExecute == false;
        private bool? CommandCanExecute => Command?.CanExecute(CommandParameter);
        private string ButtonTypeValue => Type.ToString().ToLowerInvariant();
        private string? AriaPressed => Active || Pressed ? "true" : null;
        private string? EffectiveAriaLabel => !string.IsNullOrWhiteSpace(AriaLabel)
            ? AriaLabel
            : IsIconOnly ? Title : null;
        private string IconClass => Size is XFExtendedSize.Xs or XFExtendedSize.Sm
            ? "flex h-4 w-4 shrink-0 items-center justify-center"
            : "flex h-5 w-5 shrink-0 items-center justify-center";
        private string LoadingIndicatorClass => Size is XFExtendedSize.Xs or XFExtendedSize.Sm
            ? "flex h-4 w-4 shrink-0 items-center justify-center"
            : "flex h-5 w-5 shrink-0 items-center justify-center";

        protected override void OnParametersSet()
        {
            if (Icon is not null && StartIcon is null)
            {
                StartIcon = Icon;
            }

            if (!string.IsNullOrWhiteSpace(FormTarget) && FormTarget is not ("_self" or "_blank" or "_parent" or "_top"))
            {
                throw new ArgumentException("FormTarget must be one of: _self, _blank, _parent, _top.");
            }

            if (!ReferenceEquals(_boundCommand, Command))
            {
                if (_boundCommand is not null)
                {
                    _boundCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
                }

                _boundCommand = Command;

                if (_boundCommand is not null)
                {
                    _boundCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
                }
            }
        }

        protected override string GetComponentClass()
        {
            var sizeClass = Size switch
            {
                XFExtendedSize.Xs => IsIconLike ? "h-7 w-7 text-xs" : "h-7 px-2.5 text-xs",
                XFExtendedSize.Sm => IsIconLike ? "h-8 w-8 text-sm" : "h-8 px-3 text-sm",
                XFExtendedSize.Lg => IsIconLike ? "h-11 w-11 text-base" : "h-11 px-5 text-base",
                XFExtendedSize.Xl => IsIconLike ? "h-12 w-12 text-lg" : "h-12 px-6 text-lg",
                _ => IsIconLike ? "h-10 w-10 text-sm" : "h-10 px-4 text-sm"
            };

            var variantClass = (Color, Variant) switch
            {
                (_, XFButtonVariant.Outline) => $"{ToneTextClass} bg-bg-surface {ToneBorderClass} shadow-[0_1px_1px_rgb(15_23_42/.04)] hover:bg-current/5 hover:ring-2 hover:ring-current/10 active:bg-current/10 active:ring-2 active:ring-current/20",
                (_, XFButtonVariant.Ghost) => $"{ToneTextClass} border-transparent bg-transparent hover:bg-current/7 hover:ring-1 hover:ring-current/10 active:bg-current/12",
                (_, XFButtonVariant.Soft) => $"{ToneSoftClass} border-transparent shadow-[inset_0_0_0_1px_rgb(255_255_255/.18)] hover:ring-2 hover:ring-current/12 active:ring-2 active:ring-current/20",
                (_, XFButtonVariant.Subtle) => $"{ToneTextClass} border-border-light bg-surface-50 shadow-[0_1px_2px_rgb(15_23_42/.04)] hover:bg-surface-100 hover:ring-2 hover:ring-surface-300/60 active:bg-surface-200",
                (_, XFButtonVariant.Link) => $"{ToneTextClass} h-auto border-transparent bg-transparent px-0 underline-offset-4 hover:underline active:opacity-80 shadow-none",
                _ => $"{ToneSolidClass} {ToneBorderClass} shadow-[0_1px_2px_rgb(15_23_42/.12),inset_0_1px_0_rgb(255_255_255/.18)] hover:shadow-[0_7px_14px_rgb(15_23_42/.14),inset_0_1px_0_rgb(255_255_255/.24)] hover:ring-2 hover:ring-current/20 active:shadow-[inset_0_2px_4px_rgb(15_23_42/.18)] active:ring-2 active:ring-current/25"
            };

            var stateClass = IsDisabled ? " opacity-60 cursor-not-allowed pointer-events-none" : "";
            var widthClass = FullWidth ? " w-full" : "";
            var pressedClass = Active || Pressed ? " ring-2 ring-border-focus ring-offset-2" : "";
            var shapeClass = ShapeClass;
            var baseClass = "inline-flex items-center justify-center box-border border outline-none font-semibold text-center no-underline cursor-pointer transition-all duration-200 ease-out gap-2 select-none focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 active:translate-y-px";

            return GetClass($"{BaseClass} {baseClass}{stateClass}{widthClass}{pressedClass} {shapeClass} {sizeClass} {variantClass}");
        }

        protected async Task HandleClickAsync(MouseEventArgs e)
        {
            if (IsDisabled || _clicking)
                return;

            try
            {
                _clicking = true;

                if (OnClick.HasDelegate)
                    await OnClick.InvokeAsync(e);

                if (CommandCanExecute != false)
                    Command?.Execute(CommandParameter);
            }
            finally
            {
                _clicking = false;
            }
        }

        private bool IsIconOnly =>
            StartIcon is not null &&
            EndIcon is null &&
            ChildContent is null &&
            string.IsNullOrWhiteSpace(Text) &&
            string.IsNullOrWhiteSpace(LoadingText);

        private bool IsIconLike => IsIconOnly || Shape is XFButtonShape.Square or XFButtonShape.Circle;

        private string ShapeClass => Shape switch
        {
            XFButtonShape.Circle => "rounded-full",
            XFButtonShape.Square => "rounded-lg",
            XFButtonShape.Rounded => "rounded-md",
            _ when IsIconOnly => "rounded-lg",
            _ => "rounded-md"
        };

        private string ToneTextClass => Color switch
        {
            XFButtonColor.Secondary => "text-text-secondary",
            XFButtonColor.Success => "text-success",
            XFButtonColor.Warning => "text-warning",
            XFButtonColor.Danger => "text-danger",
            XFButtonColor.Neutral => "text-text-primary",
            XFButtonColor.Info => "text-info",
            _ => "text-primary"
        };

        private string ToneBorderClass => Color switch
        {
            XFButtonColor.Secondary or XFButtonColor.Neutral => "border-border",
            XFButtonColor.Success => "border-success",
            XFButtonColor.Warning => "border-warning",
            XFButtonColor.Danger => "border-danger",
            XFButtonColor.Info => "border-info",
            _ => "border-primary"
        };

        private string ToneSolidClass => Color switch
        {
            XFButtonColor.Secondary => "bg-secondary text-secondary-contrast hover:bg-secondary-hover active:bg-secondary-active",
            XFButtonColor.Success => "bg-success text-white hover:bg-[color-mix(in_oklab,var(--color-success)_88%,black)] active:bg-[color-mix(in_oklab,var(--color-success)_78%,black)]",
            XFButtonColor.Warning => "bg-warning text-surface-950 hover:bg-[color-mix(in_oklab,var(--color-warning)_88%,black)] active:bg-[color-mix(in_oklab,var(--color-warning)_78%,black)]",
            XFButtonColor.Danger => "bg-danger text-white hover:bg-[color-mix(in_oklab,var(--color-danger)_88%,black)] active:bg-[color-mix(in_oklab,var(--color-danger)_78%,black)]",
            XFButtonColor.Neutral => "bg-surface-950 text-white hover:bg-surface-800 active:bg-surface-700",
            XFButtonColor.Info => "bg-info text-white hover:bg-[color-mix(in_oklab,var(--color-info)_88%,black)] active:bg-[color-mix(in_oklab,var(--color-info)_78%,black)]",
            _ => "bg-primary text-primary-contrast hover:bg-primary-hover active:bg-primary-active"
        };

        private string ToneSoftClass => Color switch
        {
            XFButtonColor.Secondary => "bg-surface-100 text-text-secondary",
            XFButtonColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success",
            XFButtonColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_10%,transparent)] text-warning",
            XFButtonColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger",
            XFButtonColor.Neutral => "bg-surface-100 text-text-primary",
            XFButtonColor.Info => "bg-[color-mix(in_oklab,var(--color-info)_10%,transparent)] text-info",
            _ => "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary"
        };

        private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (_boundCommand is not null)
            {
                _boundCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            }
        }
    }
}
