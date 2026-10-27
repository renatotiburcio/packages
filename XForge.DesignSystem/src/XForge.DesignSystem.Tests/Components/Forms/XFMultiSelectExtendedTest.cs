using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.MultiSelect;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFMultiSelectExtendedTest : BunitContext
{
    private static readonly Option[] Options = new[]
    {
        new Option(1, "Review", false),
        new Option(2, "Blocked", false),
        new Option(3, "Internal", true),
        new Option(4, "Done", false),
        new Option(5, "WIP", false),
    };

    private static readonly GroupedOption[] GroupedOptions = new[]
    {
        new GroupedOption(1, "TypeScript", "Languages"),
        new GroupedOption(2, "C#", "Languages"),
        new GroupedOption(3, "Docker", "DevOps"),
        new GroupedOption(4, "Kubernetes", "DevOps"),
    };

    // --- SizeClass branches (lines 121-126) ---

    [Fact]
    public void Extended_SizeSm_AppliesSmallPadding()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("py-1.5", cut.Markup);
        Assert.Contains("pl-3", cut.Markup);
        Assert.Contains("pr-1.5", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_AppliesLargePadding()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("py-2.5", cut.Markup);
        Assert.Contains("pl-4", cut.Markup);
        Assert.Contains("pr-2", cut.Markup);
    }

    // --- SizeTextClass branches (lines 128-133) ---

    [Fact]
    public void Extended_SizeTextSm_AppliesSmallText()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void Extended_SizeTextLg_AppliesBaseText()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("text-base", cut.Markup);
    }

    // --- VariantClass branches (lines 135-141) ---

    [Fact]
    public void Extended_VariantFilled_AppliesFilledStyle()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Variant, XFFormVariant.Filled);
        });
        Assert.Contains("border-transparent", cut.Markup);
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_AppliesGhostStyle()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Variant, XFFormVariant.Ghost);
        });
        Assert.Contains("border-transparent", cut.Markup);
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // --- ToneClass branches (lines 143-161) ---

    [Fact]
    public void Extended_ToneSecondary_AppliesSecondaryFocus()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
        });
        Assert.Contains("focus-within:border-[var(--color-secondary)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSuccess_AppliesSuccessFocus()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Tone, XFSemanticColor.Success);
        });
        Assert.Contains("focus-within:border-[var(--color-success)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDanger_AppliesDangerFocus()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
        });
        Assert.Contains("focus-within:border-[var(--color-danger)]", cut.Markup);
    }

    // --- StateClass (line 163) ---

    [Fact]
    public void Extended_Disabled_AppliesDisabledState()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Disabled, true);
        });
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    // --- FullWidth false (line 103) ---

    [Fact]
    public void Extended_FullWidthFalse_AppliesInlineFlex()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.FullWidth, false);
        });
        Assert.Contains("inline-flex", cut.Markup);
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    // --- Required asterisk (lines 10-13) ---

    [Fact]
    public void Extended_Required_ShowsAsterisk()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Label, "Tags");
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Required, true);
        });
        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("*", cut.Markup);
    }

    // --- HelperText (lines 198-201) ---

    [Fact]
    public void Extended_HelperText_RendersHelperDiv()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.HelperText, "Pick some tags");
        });
        Assert.Contains("Pick some tags", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // --- Loading state (lines 65-70, 101-104) ---

    [Fact]
    public void Extended_Loading_ShowsSpinner()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Loading, true);
        });
        Assert.Contains("animate-spin", cut.Markup);
        Assert.Contains("Carregando...", cut.Markup);
    }

    // --- Clearable false (line 71) ---

    [Fact]
    public void Extended_ClearableFalse_HidesClearButton()
    {
        IReadOnlyList<int> values = new[] { 1 };
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, values);
            p.Add(x => x.Clearable, false);
        });
        Assert.DoesNotContain("Limpar", cut.Markup);
    }

    // --- Clear button shown when Clearable=true and items selected (line 71) ---

    [Fact]
    public void Extended_ClearableTrue_WithValues_ShowsClearButton()
    {
        IReadOnlyList<int> values = new[] { 1 };
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, values);
            p.Add(x => x.Clearable, true);
        });
        Assert.Contains("Limpar", cut.Markup);
    }

    // --- MaxVisibleChips overflow (lines 34-37) ---

    [Fact]
    public void Extended_MaxVisibleChips_Overflow_ShowsPlusCount()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, new[] { 1, 2, 3, 4 });
            p.Add(x => x.MaxVisibleChips, 2);
        });
        Assert.Contains("+2", cut.Markup);
    }

    // --- EmptyText (lines 105-108) ---

    [Fact]
    public void Extended_EmptyItems_ShowsEmptyText()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<Option>());
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        Assert.Contains("Nenhuma opcao.", cut.Markup);
    }

    // --- Custom EmptyText ---

    [Fact]
    public void Extended_CustomEmptyText_UsesCustomText()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<Option>());
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.EmptyText, "No items found");
        });
        cut.Find("input").Focus();
        Assert.Contains("No items found", cut.Markup);
    }

    // --- Filter.StartsWith (lines 347-352) ---

    [Fact]
    public void Extended_FilterStartsWith_FiltersCorrectly()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Filter, XFMultiSelectFilter.StartsWith);
        });
        cut.Find("input").Input("Rev");
        Assert.Contains("Review", cut.Markup);
        cut.Find("input").Input("Bl");
        Assert.Contains("Blocked", cut.Markup);
    }

    // --- ShowBulkActions false (line 93) ---

    [Fact]
    public void Extended_ShowBulkActionsFalse_HidesBulkButtons()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.ShowBulkActions, false);
        });
        cut.Find("input").Focus();
        Assert.DoesNotContain("Selecionar todos", cut.Markup);
    }

    // --- ReadOnly hides chip remove and clear (lines 26-27, 71) ---

    [Fact]
    public void Extended_ReadOnly_HidesChipRemoveAndClear()
    {
        IReadOnlyList<int> values = new[] { 1 };
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, values);
            p.Add(x => x.ReadOnly, true);
        });
        Assert.DoesNotContain("Remover Review", cut.Markup);
        Assert.DoesNotContain("Limpar", cut.Markup);
    }

    // --- ItemTemplate (lines 144-149) ---

    [Fact]
    public void Extended_ItemTemplate_RendersCustomItemContent()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.ItemTemplate, (Option o) => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-item");
                builder.AddContent(2, $"#{o.Id} {o.Name}");
                builder.CloseElement();
            });
        });
        cut.Find("input").Focus();
        Assert.Contains("#1 Review", cut.Markup);
        Assert.Contains("custom-item", cut.Markup);
    }

    // --- CheckClass selected/unselected (lines 354-361) ---

    [Fact]
    public void Extended_CheckClassSelected_AppliesSelectedStyle()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, new[] { 1 });
        });
        cut.Find("input").Focus();
        Assert.Contains("border-[var(--color-primary)]", cut.Markup);
    }

    // --- GetOptionClass highlighted (lines 363-372) ---

    [Fact]
    public void Extended_GetOptionClass_Highlighted_AppliesHighlightBg()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        cut.Find(".xf-multiselect").KeyDown("ArrowDown");
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]", cut.Markup);
    }

    // --- OpenOnFocus false (lines 206-213) ---

    [Fact]
    public void Extended_OpenOnFocusFalse_DoesNotOpenOnFocus()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.OpenOnFocus, false);
        });
        cut.Find("input").Focus();
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    // --- Chevron rotation when open (line 114) ---

    [Fact]
    public void Extended_ChevronRotated_WhenOpen()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        Assert.Contains("rotate-180", cut.Markup);
    }

    // --- Aria attributes ---

    [Fact]
    public void Extended_AriaExpanded_WhenOpen()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        var input = cut.Find("input[role='combobox']");
        Assert.NotNull(input.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Extended_AriaRequired_WhenRequired()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Required, true);
        });
        var input = cut.Find("input[role='combobox']");
        Assert.NotNull(input.GetAttribute("aria-required"));
    }

    [Fact]
    public void Extended_AriaReadonly_WhenReadOnly()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.ReadOnly, true);
        });
        var input = cut.Find("input[role='combobox']");
        Assert.NotNull(input.GetAttribute("aria-readonly"));
    }

    // --- DescribedBy links to helper (line 99-101) ---

    [Fact]
    public void Extended_DescribedBy_LinksToHelper()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.HelperText, "Help text");
        });
        var input = cut.Find("input[role='combobox']");
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
    }

    // --- Validation error with EditContext (lines 89-101) ---

    [Fact]
    public void Extended_ValidationError_ShowsErrorAndAriaInvalid()
    {
        var model = new MultiSelectModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.TagIds);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "At least one tag required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFMultiSelect<Option, int>>(3);
                childBuilder.AddAttribute(4, "Values", model.TagIds);
                childBuilder.AddAttribute(5, "ValuesChanged", EventCallback.Factory.Create<IReadOnlyList<int>>(this, value => model.TagIds = value));
                childBuilder.AddAttribute(6, "ValuesExpression", (System.Linq.Expressions.Expression<Func<IReadOnlyList<int>?>>)(() => model.TagIds));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("At least one tag required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    // --- ToneClass validation error overrides tone (lines 147-149) ---

    [Fact]
    public void Extended_ValidationError_OverridesToneWithDanger()
    {
        var model = new MultiSelectModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.TagIds);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Error");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFMultiSelect<Option, int>>(3);
                childBuilder.AddAttribute(4, "Values", model.TagIds);
                childBuilder.AddAttribute(5, "ValuesChanged", EventCallback.Factory.Create<IReadOnlyList<int>>(this, value => model.TagIds = value));
                childBuilder.AddAttribute(6, "ValuesExpression", (System.Linq.Expressions.Expression<Func<IReadOnlyList<int>?>>)(() => model.TagIds));
                childBuilder.AddAttribute(7, "Tone", XFSemanticColor.Success);
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    // --- MaxItems limits displayed items (lines 78-79) ---

    [Fact]
    public void Extended_MaxItems_LimitsFilteredItems()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.MaxItems, 2);
        });
        cut.Find("input").Focus();
        var options = cut.FindAll("[role='option']");
        Assert.Equal(2, options.Count);
    }

    // --- ToggleItem removes existing value (lines 262-265) ---

    [Fact]
    public void Extended_ToggleItem_RemovesExistingValue()
    {
        IReadOnlyList<int> values = new[] { 1, 2 };
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, values);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        cut.FindAll("button").First(b => b.TextContent.Contains("Review")).Click();
        Assert.DoesNotContain(1, values);
        Assert.Contains(2, values);
    }

    // --- SelectAll selects all enabled items (lines 287-296) ---

    [Fact]
    public void Extended_SelectAll_SelectsAllEnabledItems()
    {
        IReadOnlyList<int> values = Array.Empty<int>();
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.DisabledItemSelector, o => o.Locked);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        cut.FindAll("button").First(b => b.TextContent.Contains("Selecionar todos")).Click();
        Assert.Contains(1, values);
        Assert.Contains(2, values);
        Assert.Contains(4, values);
        Assert.Contains(5, values);
        Assert.DoesNotContain(3, values); // disabled
    }

    // --- Clear resets values (lines 298-303) ---

    [Fact]
    public void Extended_Clear_ResetsAllValues()
    {
        IReadOnlyList<int> values = new[] { 1, 2 };
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Values, values);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        cut.FindAll("button").First(b => b.TextContent.Contains("Limpar")).Click();
        Assert.Empty(values);
    }

    // --- Keyboard: Escape closes (lines 229-231) ---

    [Fact]
    public void Extended_Escape_ClosesDropdown()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        Assert.Contains("role=\"listbox\"", cut.Markup);
        cut.Find(".xf-multiselect").KeyDown("Escape");
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    // --- Keyboard: ArrowDown opens and highlights (lines 233-236) ---

    [Fact]
    public void Extended_ArrowDown_OpensAndHighlights()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.OpenOnFocus, false);
        });
        cut.Find(".xf-multiselect").KeyDown("ArrowDown");
        var input = cut.Find("input[role='combobox']");
        var activeId = input.GetAttribute("aria-activedescendant");
        Assert.NotNull(activeId);
    }

    // --- Keyboard: ArrowUp highlights previous (lines 238-241) ---

    [Fact]
    public void Extended_ArrowUp_HighlightsPrevious()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        cut.Find(".xf-multiselect").KeyDown("ArrowDown");
        cut.Find(".xf-multiselect").KeyDown("ArrowDown");
        cut.Find(".xf-multiselect").KeyDown("ArrowUp");
        var input = cut.Find("input[role='combobox']");
        var activeId = input.GetAttribute("aria-activedescendant");
        Assert.NotNull(activeId);
    }

    // --- Keyboard: Enter toggles highlighted item (lines 243-245) ---

    [Fact]
    public void Extended_Enter_TogglesHighlightedItem()
    {
        IReadOnlyList<int> values = Array.Empty<int>();
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        cut.Find(".xf-multiselect").KeyDown("Enter");
        Assert.Contains(1, values);
    }

    // --- Keyboard: Tab closes (lines 247-249) ---

    [Fact]
    public void Extended_Tab_ClosesDropdown()
    {
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
        });
        cut.Find("input").Focus();
        Assert.Contains("role=\"listbox\"", cut.Markup);
        cut.Find(".xf-multiselect").KeyDown("Tab");
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    // --- Disabled prevents toggle (lines 180-181, 218-219) ---

    [Fact]
    public void Extended_Disabled_PreventsToggle()
    {
        IReadOnlyList<int> values = Array.Empty<int>();
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.Disabled, true);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    // --- DisabledItemSelector prevents toggle (lines 254-257) ---

    [Fact]
    public void Extended_DisabledItem_PreventsToggle()
    {
        IReadOnlyList<int> values = Array.Empty<int>();
        var cut = Render<XFMultiSelect<Option, int>>(p =>
        {
            p.Add(x => x.Items, Options);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.DisabledItemSelector, o => o.Locked);
            p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, next => values = next));
        });
        cut.Find("input").Focus();
        cut.FindAll("button").First(b => b.TextContent.Contains("Internal")).Click();
        Assert.Empty(values);
    }

    // --- GroupHeaderClass (line 380) ---

    [Fact]
    public void Extended_GroupHeaderClass_AppliesHeaderStyle()
    {
        var cut = Render<XFMultiSelect<GroupedOption, int>>(p =>
        {
            p.Add(x => x.Items, GroupedOptions);
            p.Add(x => x.TextSelector, o => o.Name);
            p.Add(x => x.ValueSelector, o => o.Id);
            p.Add(x => x.GroupSelector, o => o.Group);
        });
        cut.Find("input").Focus();
        Assert.Contains("font-semibold", cut.Markup);
        Assert.Contains("uppercase", cut.Markup);
    }

    // --- Records ---

    private sealed record Option(int Id, string Name, bool Locked);
    private sealed record GroupedOption(int Id, string Name, string Group);

    private sealed class MultiSelectModel
    {
        [Required]
        public IReadOnlyList<int>? TagIds { get; set; }
    }
}
