using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.EditForm
{
    public partial class XFEditForm : XForgeComponentBase, IDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private EditContext? internalEditContext;
        private bool internalSubmitting;
        private bool validationReviewOpen;
        private bool unsavedChangesOpen;
        private XFEditFormCloseIntent pendingCloseIntent = XFEditFormCloseIntent.Close;
        private EditContext? previousSubscribedEditContext;
        private object? previousModel;
        private IDisposable? navigationGuardRegistration;
        private TaskCompletionSource<bool>? pendingNavigationConfirmation;

        [Parameter] public object? Model { get; set; }
        [Parameter] public EditContext? EditContext { get; set; }
        [Parameter] public RenderFragment<XFEditFormContext>? ChildContent { get; set; }
        [Parameter] public RenderFragment<XFEditFormContext>? Header { get; set; }
        [Parameter] public RenderFragment<XFEditFormContext>? Actions { get; set; }
        [Parameter] public RenderFragment<XFEditFormContext>? Footer { get; set; }
        [Parameter] public RenderFragment? Validator { get; set; }
        [Parameter] public EventCallback<EditContext> OnSubmit { get; set; }
        [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }
        [Parameter] public EventCallback<EditContext> OnInvalidSubmit { get; set; }
        [Parameter] public EventCallback<EditContext> OnDiscard { get; set; }
        [Parameter] public EventCallback<EditContext> OnSaveAndClose { get; set; }
        [Parameter] public EventCallback<XFEditFormCloseIntent> CloseApproved { get; set; }
        [Parameter] public string? FormName { get; set; }
        [Parameter] public bool Enhance { get; set; }
        [Parameter] public XFEditFormMethod? Method { get; set; }
        [Parameter] public string? FormAction { get; set; }
        [Parameter] public XFEditFormLayout Layout { get; set; } = XFEditFormLayout.Vertical;
        [Parameter] public XFDensity Density { get; set; } = XFDensity.Comfortable;
        [Parameter] public XFEditFormValidationMode ValidationMode { get; set; } = XFEditFormValidationMode.DataAnnotations;
        [Parameter] public XFEditFormValidationReview ValidationReview { get; set; } = XFEditFormValidationReview.None;
        [Parameter] public bool ShowValidationSummary { get; set; }
        [Parameter] public bool BlockActionsWhenInvalid { get; set; }
        [Parameter] public bool TrackChanges { get; set; } = true;
        [Parameter] public bool ConfirmUnsavedChanges { get; set; }
        [Parameter] public bool GuardNavigationWhenModified { get; set; } = true;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Submitting { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string ValidationReviewTitle { get; set; } = default!;
        [Parameter] public string ValidationReviewDescription { get; set; } = default!;
        [Parameter] public string ValidationReviewButtonText { get; set; } = default!;
        [Parameter] public string ValidationReviewLead { get; set; } = default!;
        [Parameter] public string ValidationReviewHint { get; set; } = default!;
        [Parameter] public string ValidationReviewCloseText { get; set; } = default!;
        [Parameter] public string ValidationReviewSingleCountFormat { get; set; } = default!;
        [Parameter] public string ValidationReviewMultipleCountFormat { get; set; } = default!;
        [Parameter] public string UnsavedChangesTitle { get; set; } = default!;
        [Parameter] public string UnsavedChangesDescription { get; set; } = default!;
        [Parameter] public string UnsavedChangesLead { get; set; } = default!;
        [Parameter] public string UnsavedChangesHint { get; set; } = default!;
        [Parameter] public string UnsavedChangesContinueText { get; set; } = default!;
        [Parameter] public string UnsavedChangesDiscardText { get; set; } = default!;
        [Parameter] public string UnsavedChangesSaveText { get; set; } = default!;

        [Inject] private IServiceProvider Services { get; set; } = default!;

        private EditContext ResolvedEditContext { get; set; } = default!;
        private bool IsSubmitting => Submitting || internalSubmitting;
        private bool IsModified => TrackChanges && ResolvedEditContext?.IsModified() == true;
        private IReadOnlyList<string> ValidationMessages => ResolvedEditContext?.GetValidationMessages().Where(message => !string.IsNullOrWhiteSpace(message)).Distinct().ToArray()
            ?? Array.Empty<string>();
        private bool HasValidationMessages => ValidationMessages.Count > 0;
        private bool ShouldBlockActions => BlockActionsWhenInvalid && HasValidationMessages;
        private bool ShouldShowValidationReview => ValidationReview != XFEditFormValidationReview.None && HasValidationMessages;
        private IReadOnlyDictionary<string, object>? FormAttributes => BuildFormAttributes();

        private XFEditFormContext CreateContext(EditContext editContext) => new(
            editContext,
            Disabled,
            ReadOnly,
            IsSubmitting,
            HasValidationMessages,
            ShouldBlockActions,
            TrackChanges,
            ConfirmUnsavedChanges,
            IsModified,
            Layout,
            Density,
            RequestCloseAsync,
            MarkSavedAsync,
            ValidateCurrentAsync);

        private string RootClass => Join(
            "xf-edit-form",
            LayoutClass,
            DensityClass,
            Disabled || IsSubmitting ? "opacity-60" : null,
            ReadOnly ? "data-[readonly=true]:cursor-default" : null,
            Class);

        private string HeaderClass => Density == XFDensity.Compact
            ? "grid gap-1 border-b border-border-light pb-3"
            : "grid gap-2 border-b border-border-light pb-4";

        private string FieldsetClass => Join(
            "min-w-0 border-0 p-0",
            Layout switch
            {
                XFEditFormLayout.Grid => "grid gap-4 md:grid-cols-2",
                XFEditFormLayout.Inline => "flex flex-wrap items-end gap-3",
                _ => "grid gap-4"
            });

        private string ActionsClass => Density == XFDensity.Compact
            ? Join("flex flex-wrap items-center justify-end gap-2 border-t border-border-light pt-3", ShouldBlockActions ? "pointer-events-none opacity-55 grayscale" : null)
            : Join("flex flex-wrap items-center justify-end gap-3 border-t border-border-light pt-4", ShouldBlockActions ? "pointer-events-none opacity-55 grayscale" : null);

        private string FooterClass => "border-t border-border-light pt-3 text-sm text-text-secondary";
        private string ValidationSummaryClass => Density == XFDensity.Compact ? "my-3" : "my-4";
        private string ValidationReviewClass => Join(
            "rounded-xl border p-4 shadow-[0_12px_36px_rgb(15_23_42/.06)]",
            ValidationReview == XFEditFormValidationReview.BannerAndButton
                ? "border-[color-mix(in_oklab,var(--color-danger)_20%,var(--color-border))] bg-[color-mix(in_oklab,var(--color-danger)_4%,var(--color-bg-surface))]"
                : "border-border-light bg-surface-50");
        private string ValidationReviewButtonClass => "inline-flex h-9 items-center justify-center gap-2 rounded-lg border border-[color-mix(in_oklab,var(--color-danger)_25%,var(--color-border))] bg-bg-surface px-3 text-sm font-semibold text-danger shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-danger)_8%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)]";
        private string ValidationDialogButtonClass => "inline-flex h-9 items-center justify-center rounded-lg bg-primary px-4 text-sm font-semibold text-white shadow-[0_1px_2px_rgb(15_23_42/.08)] transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_86%,black)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        private string UnsavedDialogPrimaryButtonClass => "inline-flex h-9 items-center justify-center rounded-lg bg-primary px-4 text-sm font-semibold text-white shadow-[0_1px_2px_rgb(15_23_42/.08)] transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_86%,black)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        private string UnsavedDialogSecondaryButtonClass => "inline-flex h-9 items-center justify-center rounded-lg border border-border bg-bg-surface px-4 text-sm font-semibold text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150 hover:bg-surface-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-border-focus";
        private string UnsavedDialogDangerButtonClass => "inline-flex h-9 items-center justify-center rounded-lg border border-[color-mix(in_oklab,var(--color-danger)_26%,var(--color-border))] bg-bg-surface px-4 text-sm font-semibold text-danger shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-danger)_8%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)]";

        private string LayoutClass => Layout switch
        {
            XFEditFormLayout.Grid => "grid gap-5",
            XFEditFormLayout.Inline => "grid gap-4",
            _ => "grid gap-5"
        };

        private string DensityClass => Density == XFDensity.Compact ? "text-sm" : "";

        private IReadOnlyDictionary<string, object>? BuildFormAttributes()
        {
            if (AdditionalAttributes is null && Method is null && string.IsNullOrWhiteSpace(FormAction))
            {
                return null;
            }

            var attributes = AdditionalAttributes is null
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object>(AdditionalAttributes, StringComparer.OrdinalIgnoreCase);

            if (Method is not null)
            {
                attributes["method"] = Method.Value.ToString().ToLowerInvariant();
            }

            if (!string.IsNullOrWhiteSpace(FormAction))
            {
                attributes["action"] = FormAction;
            }

            return attributes;
        }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ValidationReviewTitle)) ValidationReviewTitle = L("editform.validation_title", ValidationReviewTitle);
            if (string.IsNullOrWhiteSpace(ValidationReviewDescription)) ValidationReviewDescription = L("editform.validation_desc", ValidationReviewDescription);
            if (string.IsNullOrWhiteSpace(ValidationReviewButtonText)) ValidationReviewButtonText = L("editform.validation_button", ValidationReviewButtonText);
            if (string.IsNullOrWhiteSpace(ValidationReviewLead)) ValidationReviewLead = L("editform.validation_lead", ValidationReviewLead);
            if (string.IsNullOrWhiteSpace(ValidationReviewHint)) ValidationReviewHint = L("editform.validation_hint", ValidationReviewHint);
            if (string.IsNullOrWhiteSpace(ValidationReviewCloseText)) ValidationReviewCloseText = L("editform.validation_close", ValidationReviewCloseText);
            if (string.IsNullOrWhiteSpace(ValidationReviewSingleCountFormat)) ValidationReviewSingleCountFormat = L("editform.validation_single", ValidationReviewSingleCountFormat);
            if (string.IsNullOrWhiteSpace(ValidationReviewMultipleCountFormat)) ValidationReviewMultipleCountFormat = L("editform.validation_multiple", ValidationReviewMultipleCountFormat);
            if (string.IsNullOrWhiteSpace(UnsavedChangesTitle)) UnsavedChangesTitle = L("editform.unsaved_title", UnsavedChangesTitle);
            if (string.IsNullOrWhiteSpace(UnsavedChangesDescription)) UnsavedChangesDescription = L("editform.unsaved_desc", UnsavedChangesDescription);
            if (string.IsNullOrWhiteSpace(UnsavedChangesLead)) UnsavedChangesLead = L("editform.unsaved_lead", UnsavedChangesLead);
            if (string.IsNullOrWhiteSpace(UnsavedChangesHint)) UnsavedChangesHint = L("editform.unsaved_hint", UnsavedChangesHint);
            if (string.IsNullOrWhiteSpace(UnsavedChangesContinueText)) UnsavedChangesContinueText = L("editform.unsaved_continue", UnsavedChangesContinueText);
            if (string.IsNullOrWhiteSpace(UnsavedChangesDiscardText)) UnsavedChangesDiscardText = L("editform.unsaved_discard", UnsavedChangesDiscardText);
            if (string.IsNullOrWhiteSpace(UnsavedChangesSaveText)) UnsavedChangesSaveText = L("editform.save_close", UnsavedChangesSaveText);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Model is not null && EditContext is not null)
            {
                throw new InvalidOperationException($"{nameof(XFEditForm)} requires either a {nameof(Model)} or an {nameof(EditContext)}, not both.");
            }

            if (Model is null && EditContext is null)
            {
                throw new InvalidOperationException($"{nameof(XFEditForm)} requires either a {nameof(Model)} or an {nameof(EditContext)}.");
            }

            if (OnSubmit.HasDelegate && (OnValidSubmit.HasDelegate || OnInvalidSubmit.HasDelegate))
            {
                throw new InvalidOperationException($"{nameof(XFEditForm)} follows Blazor EditForm semantics: use either {nameof(OnSubmit)} or {nameof(OnValidSubmit)}/{nameof(OnInvalidSubmit)}.");
            }

            if (EditContext is not null)
            {
                ResolvedEditContext = EditContext;
            }
            else if (!ReferenceEquals(Model, previousModel))
            {
                internalEditContext = new EditContext(Model!);
                previousModel = Model;
                ResolvedEditContext = internalEditContext;
            }
            else
            {
                ResolvedEditContext = internalEditContext!;
            }

            SubscribeToEditContext(ResolvedEditContext);
            UpdateNavigationGuardRegistration();
        }

        private async Task HandleSubmitAsync(EditContext editContext)
        {
            if (OnSubmit.HasDelegate)
            {
                await InvokeWithSubmitStateAsync(() => OnSubmit.InvokeAsync(editContext));
            }
        }

        private async Task HandleValidSubmitAsync(EditContext editContext)
        {
            if (OnValidSubmit.HasDelegate)
            {
                await InvokeWithSubmitStateAsync(() => OnValidSubmit.InvokeAsync(editContext));
            }
        }

        private async Task HandleInvalidSubmitAsync(EditContext editContext)
        {
            if (OnInvalidSubmit.HasDelegate)
            {
                await InvokeWithSubmitStateAsync(() => OnInvalidSubmit.InvokeAsync(editContext));
            }
        }

        private RenderFragment RenderValidationReview() => builder =>
        {
            var sequence = 0;
            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", ValidationReviewClass);
            builder.AddAttribute(sequence++, "role", "status");
            builder.AddAttribute(sequence++, "aria-live", "polite");

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "flex flex-wrap items-center justify-between gap-3");

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "flex min-w-0 items-start gap-3");

            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", "mt-0.5 inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger");
            builder.AddAttribute(sequence++, "aria-hidden", "true");
            builder.AddContent(sequence++, XFSvgIcons.MarkupWithSize("circle-info", 18));
            builder.CloseElement();

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "grid gap-1");

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "text-sm font-semibold text-text-primary");
            builder.AddContent(sequence++, FormatValidationReviewCount());
            builder.CloseElement();

            if (ValidationReview == XFEditFormValidationReview.BannerAndButton)
            {
                builder.OpenElement(sequence++, "div");
                builder.AddAttribute(sequence++, "class", "text-sm leading-6 text-text-secondary");
                builder.AddContent(sequence++, ValidationReviewDescription);
                builder.CloseElement();
            }

            builder.CloseElement();
            builder.CloseElement();

            builder.OpenElement(sequence++, "button");
            builder.AddAttribute(sequence++, "type", "button");
            builder.AddAttribute(sequence++, "class", ValidationReviewButtonClass);
            builder.AddAttribute(sequence++, "onclick", EventCallback.Factory.Create(this, OpenValidationReviewAsync));

            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", "inline-flex h-4 w-4");
            builder.AddAttribute(sequence++, "aria-hidden", "true");
            builder.AddContent(sequence++, XFSvgIcons.MarkupWithSize("circle-info", 16));
            builder.CloseElement();

            builder.AddContent(sequence++, ValidationReviewButtonText);
            builder.CloseElement();

            builder.CloseElement();
            builder.CloseElement();
        };

        private Task OpenValidationReviewAsync()
        {
            validationReviewOpen = true;
            return Task.CompletedTask;
        }

        private string FormatValidationReviewCount()
        {
            var format = ValidationMessages.Count == 1
                ? ValidationReviewSingleCountFormat
                : ValidationReviewMultipleCountFormat;

            return string.Format(CultureInfo.CurrentCulture, format, ValidationMessages.Count);
        }

        private Task CloseValidationReviewAsync()
        {
            validationReviewOpen = false;
            return Task.CompletedTask;
        }

        private Task HandleValidationReviewOpenChangedAsync(bool open)
        {
            validationReviewOpen = open;
            return Task.CompletedTask;
        }

        private Task HandleUnsavedChangesOpenChangedAsync(bool open)
        {
            unsavedChangesOpen = open;
            return Task.CompletedTask;
        }

        private async Task RequestCloseAsync(XFEditFormCloseIntent intent = XFEditFormCloseIntent.Close)
        {
            if (Disabled || IsSubmitting)
            {
                return;
            }

            pendingCloseIntent = intent;

            if (ConfirmUnsavedChanges && IsModified)
            {
                unsavedChangesOpen = true;
                await InvokeAsync(StateHasChanged);
                return;
            }

            await ApproveCloseAsync(intent);
        }

        private Task ContinueEditingAsync()
        {
            unsavedChangesOpen = false;
            pendingNavigationConfirmation?.TrySetResult(false);
            pendingNavigationConfirmation = null;
            return Task.CompletedTask;
        }

        private async Task DiscardChangesAsync()
        {
            unsavedChangesOpen = false;
            await MarkSavedAsync();

            if (OnDiscard.HasDelegate)
            {
                await OnDiscard.InvokeAsync(ResolvedEditContext);
            }

            await ApproveCloseAsync(pendingCloseIntent);
            pendingNavigationConfirmation?.TrySetResult(true);
            pendingNavigationConfirmation = null;
        }

        private async Task SaveAndCloseAsync()
        {
            unsavedChangesOpen = false;

            var isValid = await ValidateCurrentAsync();
            if (!isValid)
            {
                if (ValidationReview != XFEditFormValidationReview.None && HasValidationMessages)
                {
                    validationReviewOpen = true;
                }

                if (OnInvalidSubmit.HasDelegate)
                {
                    await OnInvalidSubmit.InvokeAsync(ResolvedEditContext);
                }

                await InvokeAsync(StateHasChanged);
                return;
            }

            await InvokeWithSubmitStateAsync(async () =>
            {
                if (OnSaveAndClose.HasDelegate)
                {
                    await OnSaveAndClose.InvokeAsync(ResolvedEditContext);
                }
                else if (OnValidSubmit.HasDelegate)
                {
                    await OnValidSubmit.InvokeAsync(ResolvedEditContext);
                }
            });

            await MarkSavedAsync();
            await ApproveCloseAsync(pendingCloseIntent);
            pendingNavigationConfirmation?.TrySetResult(true);
            pendingNavigationConfirmation = null;
        }

        private Task<bool> ConfirmNavigationAsync(string? targetUri)
        {
            if (!ConfirmUnsavedChanges || !IsModified)
            {
                return Task.FromResult(true);
            }

            pendingCloseIntent = XFEditFormCloseIntent.NavigateAway;
            pendingNavigationConfirmation = new TaskCompletionSource<bool>();
            unsavedChangesOpen = true;
            _ = InvokeAsync(StateHasChanged);
            return pendingNavigationConfirmation.Task;
        }

        private Task<bool> ValidateCurrentAsync()
        {
            var isValid = ResolvedEditContext.Validate();
            return Task.FromResult(isValid);
        }

        private Task MarkSavedAsync()
        {
            ResolvedEditContext.MarkAsUnmodified();
            return InvokeAsync(StateHasChanged);
        }

        private async Task ApproveCloseAsync(XFEditFormCloseIntent intent)
        {
            if (CloseApproved.HasDelegate)
            {
                await CloseApproved.InvokeAsync(intent);
            }
        }

        private async Task InvokeWithSubmitStateAsync(Func<Task> callback)
        {
            internalSubmitting = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                await callback();
            }
            finally
            {
                internalSubmitting = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void SubscribeToEditContext(EditContext editContext)
        {
            if (ReferenceEquals(previousSubscribedEditContext, editContext))
            {
                return;
            }

            if (previousSubscribedEditContext is not null)
            {
                previousSubscribedEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                previousSubscribedEditContext.OnFieldChanged -= OnFieldChanged;
            }

            previousSubscribedEditContext = editContext;
            previousSubscribedEditContext.OnValidationStateChanged += OnValidationStateChanged;
            previousSubscribedEditContext.OnFieldChanged += OnFieldChanged;
        }

        private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs args)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        private void OnFieldChanged(object? sender, FieldChangedEventArgs args)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        private void UpdateNavigationGuardRegistration()
        {
            if (!GuardNavigationWhenModified || !ConfirmUnsavedChanges)
            {
                navigationGuardRegistration?.Dispose();
                navigationGuardRegistration = null;
                return;
            }

            if (navigationGuardRegistration is not null)
            {
                return;
            }

            var navigationGuardService = Services.GetService(typeof(XFNavigationGuardService)) as XFNavigationGuardService;
            navigationGuardRegistration = navigationGuardService?.Register(() => ConfirmUnsavedChanges && IsModified, ConfirmNavigationAsync);
        }

        public void Dispose()
        {
            if (previousSubscribedEditContext is not null)
            {
                previousSubscribedEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                previousSubscribedEditContext.OnFieldChanged -= OnFieldChanged;
            }

            navigationGuardRegistration?.Dispose();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
