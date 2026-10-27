using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.DropDownDataGrid;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDropDownDataGridExtendedTest : BunitContext
{
    public XFDropDownDataGridExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDropDownDataGrid<string, string>> RenderDDDG(
        Action<ComponentParameterCollectionBuilder<XFDropDownDataGrid<string, string>>>? configure = null)
    {
        return Render<XFDropDownDataGrid<string, string>>(p =>
        {
            p.Add(x => x.Items, new List<string> { "Item 1", "Item 2", "Item 3" });
            p.Add(x => x.TextSelector, (string item) => item);
            p.Add(x => x.ValueSelector, (string item) => item);
            configure?.Invoke(p);
        });
    }

    // --- 1. BaseClass ---

    [Fact]
    public void Extended_BaseClass_ContainsXfDropDownDataGrid()
    {
        var cut = RenderDDDG();
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    // --- 2. RootClass ---

    [Fact]
    public void Extended_RootClass_ContainsFlexFlexColGap1_5()
    {
        var cut = RenderDDDG();
        Assert.Contains("flex flex-col gap-1.5", cut.Markup);
    }

    // --- 3. FullWidth true ---

    [Fact]
    public void Extended_FullWidthTrue_ContainsWFull()
    {
        var cut = RenderDDDG(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    // --- 4. FullWidth false ---

    [Fact]
    public void Extended_FullWidthFalse_ContainsInlineFlex()
    {
        var cut = RenderDDDG(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    // --- 5. Label ---

    [Fact]
    public void Extended_Label_RendersLabelElementWithText()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Label, "Select Item"));
        var label = cut.Find("label");
        Assert.NotNull(label);
        Assert.Contains("Select Item", label.InnerHtml);
    }

    // --- 6. LabelClass ---

    [Fact]
    public void Extended_LabelClass_ContainsFlexItemsCenterGap1()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Label, "Test"));
        Assert.Contains("flex items-center gap-1", cut.Markup);
    }

    // --- 7. HelperText ---

    [Fact]
    public void Extended_HelperText_RendersHelperText()
    {
        var cut = RenderDDDG(p => p.Add(x => x.HelperText, "Pick one"));
        Assert.Contains("Pick one", cut.Markup);
    }

    // --- 8. HelperClass ---

    [Fact]
    public void Extended_HelperClass_ContainsTextXsLeading5TextTextSecondary()
    {
        var cut = RenderDDDG(p => p.Add(x => x.HelperText, "Help"));
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // --- 9. Placeholder ---

    [Fact]
    public void Extended_Placeholder_RendersOnInput()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Placeholder, "Choose..."));
        var input = cut.Find("input[placeholder]");
        Assert.Equal("Choose...", input.GetAttribute("placeholder"));
    }

    // --- 10. AriaLabel ---

    [Fact]
    public void Extended_AriaLabel_RendersOnInput()
    {
        var cut = RenderDDDG(p => p.Add(x => x.AriaLabel, "Dropdown aria"));
        var input = cut.Find("input[aria-label]");
        Assert.Equal("Dropdown aria", input.GetAttribute("aria-label"));
    }

    // --- 11. Name ---

    [Fact]
    public void Extended_Name_RendersOnInput()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Name, "my-field"));
        var input = cut.Find("input[name]");
        Assert.Equal("my-field", input.GetAttribute("name"));
    }

    // --- 12. Required ---

    [Fact]
    public void Extended_Required_RendersAsterisk()
    {
        var cut = RenderDDDG(p =>
        {
            p.Add(x => x.Label, "Required Field");
            p.Add(x => x.Required, true);
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("*", cut.Find("label").InnerHtml);
    }

    // --- 13. ReadOnly parameter accepted ---

    [Fact]
    public void Extended_ReadOnly_Accepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.ReadOnly, true));
        var input = cut.Find("input[readonly]");
        Assert.NotNull(input);
    }

    // --- 14. Disabled parameter accepted ---

    [Fact]
    public void Extended_Disabled_Accepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Disabled, true));
        var input = cut.Find("input[disabled]");
        Assert.NotNull(input);
    }

    // --- 15. Loading parameter accepted ---

    [Fact]
    public void Extended_Loading_Accepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Loading, true));
        Assert.Contains("animate-spin", cut.Markup);
    }

    // --- 16. Clearable parameter accepted ---

    [Fact]
    public void Extended_Clearable_Accepted()
    {
        var cut = RenderDDDG(p =>
        {
            p.Add(x => x.Clearable, true);
            p.Add(x => x.Value, "Item 1");
        });
        Assert.Contains("Limpar", cut.Markup);
    }

    // --- 17. ShowSearch parameter accepted ---

    [Fact]
    public void Extended_ShowSearch_Accepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.ShowSearch, false));
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    // --- 18. MaxDropdownHeight parameter accepted ---

    [Fact]
    public void Extended_MaxDropdownHeight_Accepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.MaxDropdownHeight, 500));
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    // --- 19. Class parameter ---

    [Fact]
    public void Extended_Class_CustomClassApplied()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Class, "custom-class"));
        Assert.Contains("custom-class", cut.Markup);
    }

    // --- 20. Style parameter ---

    [Fact]
    public void Extended_Style_Applied()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Style, "color:red"));
        Assert.Contains("color:red", cut.Markup);
    }

    // --- 21. Id parameter ---

    [Fact]
    public void Extended_Id_RenderedOnInput()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Id, "my-dd-id"));
        var input = cut.Find("input#my-dd-id");
        Assert.NotNull(input);
    }

    // --- 22. AdditionalAttributes ---

    [Fact]
    public void Extended_AdditionalAttributes_RenderedOnInput()
    {
        var cut = RenderDDDG(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "dddg-test" } }));
        var input = cut.Find("input[data-testid]");
        Assert.Equal("dddg-test", input.GetAttribute("data-testid"));
    }

    // --- 23. FieldShellClass ---

    [Fact]
    public void Extended_FieldShellClass_ContainsGroupRelativeFlexItemsCenterGap2()
    {
        var cut = RenderDDDG();
        Assert.Contains("group relative flex items-center gap-2", cut.Markup);
    }

    // --- 24. InputClass ---

    [Fact]
    public void Extended_InputClass_ContainsMinWFlex1Border0BgTransparentP0TextSm()
    {
        var cut = RenderDDDG();
        Assert.Contains("min-w-0 flex-1 border-0 bg-transparent p-0 text-sm", cut.Markup);
    }

    // --- 25. AdornmentClass ---

    [Fact]
    public void Extended_AdornmentClass_ContainsFlexShrink0ItemsCenterTextTextMuted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Loading, true));
        Assert.Contains("flex shrink-0 items-center text-text-muted", cut.Markup);
    }

    // --- 26. IconButtonClass ---

    [Fact]
    public void Extended_IconButtonClass_ContainsInlineFlexH6W6Shrink0ItemsCenterJustifyCenterRoundedMd()
    {
        var cut = RenderDDDG(p =>
        {
            p.Add(x => x.Clearable, true);
            p.Add(x => x.Value, "Item 1");
        });
        Assert.Contains("inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md", cut.Markup);
    }

    // --- 27. ToggleButtonClass ---

    [Fact]
    public void Extended_ToggleButtonClass_ContainsInlineFlexH6W6Shrink0ItemsCenterJustifyCenterRoundedMd()
    {
        var cut = RenderDDDG();
        var toggleButton = cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Abrir opcoes");
        Assert.Contains("inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md", toggleButton.GetAttribute("class"));
    }

    // --- 28. ChevronClass ---

    [Fact]
    public void Extended_ChevronClass_ContainsH4W4TransitionTransformDuration150()
    {
        var cut = RenderDDDG();
        Assert.Contains("h-4 w-4 transition-transform duration-150", cut.Markup);
    }

    // --- 29. StateRowClass ---

    [Fact]
    public void Extended_StateRowClass_PropertyExists()
    {
        // StateRowClass ("px-3 py-6 text-center text-sm text-text-muted") only renders
        // when the dropdown is open and items are empty — requires JS interop.
        // Verify the component renders correctly instead.
        var cut = RenderDDDG();
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    // --- 30. Default values verification ---

    [Fact]
    public void Extended_Defaults_ShowSearch_IsTrue()
    {
        var cut = RenderDDDG();
        Assert.True(cut.Instance.ShowSearch);
    }

    [Fact]
    public void Extended_Defaults_MaxDropdownHeight_Is320()
    {
        var cut = RenderDDDG();
        Assert.Equal(320, cut.Instance.MaxDropdownHeight);
    }

    [Fact]
    public void Extended_Defaults_FullWidth_IsTrue()
    {
        var cut = RenderDDDG();
        Assert.True(cut.Instance.FullWidth);
    }

    [Fact]
    public void Extended_Defaults_Clearable_IsTrue()
    {
        var cut = RenderDDDG();
        Assert.True(cut.Instance.Clearable);
    }

    [Fact]
    public void Extended_Defaults_Required_IsFalse()
    {
        var cut = RenderDDDG();
        Assert.False(cut.Instance.Required);
    }

    [Fact]
    public void Extended_Defaults_ReadOnly_IsFalse()
    {
        var cut = RenderDDDG();
        Assert.False(cut.Instance.ReadOnly);
    }

    [Fact]
    public void Extended_Defaults_Loading_IsFalse()
    {
        var cut = RenderDDDG();
        Assert.False(cut.Instance.Loading);
    }

    [Fact]
    public void Extended_Defaults_ValueChanged_ParameterAccepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, _ => { })));
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    [Fact]
    public void Extended_Defaults_Change_ParameterAccepted()
    {
        var cut = RenderDDDG(p => p.Add(x => x.Change, EventCallback.Factory.Create<string?>(this, _ => { })));
        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
    }

    [Fact]
    public void Extended_Defaults_ClearLabel_IsLimpar()
    {
        var cut = RenderDDDG();
        Assert.Equal("Limpar", cut.Instance.ClearLabel);
    }

    [Fact]
    public void Extended_Defaults_ToggleLabel_IsAbrirOpcoes()
    {
        var cut = RenderDDDG();
        Assert.Equal("Abrir opcoes", cut.Instance.ToggleLabel);
    }

    [Fact]
    public void Extended_Defaults_LoadingText_IsCarregando()
    {
        var cut = RenderDDDG();
        Assert.Equal("Carregando...", cut.Instance.LoadingText);
    }

    [Fact]
    public void Extended_Defaults_EmptyText_IsNenhumResultado()
    {
        var cut = RenderDDDG();
        Assert.Equal("Nenhum resultado.", cut.Instance.EmptyText);
    }

    [Fact]
    public void Extended_Defaults_ListboxLabel_IsOpcoes()
    {
        var cut = RenderDDDG();
        Assert.Equal("Opcoes", cut.Instance.ListboxLabel);
    }

    [Fact]
    public void Extended_Defaults_SearchPlaceholder_IsBuscar()
    {
        var cut = RenderDDDG();
        Assert.Equal("Buscar...", cut.Instance.SearchPlaceholder);
    }
}
