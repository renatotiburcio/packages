using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSpreadsheetToolbarComponentTest : XForgeTestContext
{
    public XFSpreadsheetToolbarComponentTest() { Services.AddSingleton<IXFLocalizer, TestLocalizer>(); }

    // Test toolbar container
    [Fact] public void Toolbar_RendersContainer() { var cut = Render<XFSpreadsheetToolbar>(); Assert.Contains("xf-spreadsheet-toolbar", cut.Markup); }
    [Fact] public void Toolbar_RendersChildContent() { var cut = Render<XFSpreadsheetToolbar>(p => p.AddChildContent("<button>Test</button>")); Assert.Contains("Test", cut.Markup); }

    // Test key toolbar buttons
    [Fact] public void Bold_RendersButton() { var cut = Render<XFSpreadsheetBold>(); Assert.Contains("Bold", cut.Markup); }
    [Fact] public void Italic_RendersButton() { var cut = Render<XFSpreadsheetItalic>(); Assert.Contains("Italic", cut.Markup); }
    [Fact] public void Underline_RendersButton() { var cut = Render<XFSpreadsheetUnderline>(); Assert.Contains("Underline", cut.Markup); }
    [Fact] public void Undo_RendersButton() { var cut = Render<XFSpreadsheetUndo>(); Assert.Contains("Undo", cut.Markup); }
    [Fact] public void Redo_RendersButton() { var cut = Render<XFSpreadsheetRedo>(); Assert.Contains("Redo", cut.Markup); }
    [Fact] public void Save_RendersButton() { var cut = Render<XFSpreadsheetSave>(); Assert.Contains("Save", cut.Markup); }
    [Fact] public void AlignLeft_RendersButton() { var cut = Render<XFSpreadsheetAlignLeft>(); Assert.Contains("Align Left", cut.Markup); }
    [Fact] public void AlignCenter_RendersButton() { var cut = Render<XFSpreadsheetAlignCenter>(); Assert.Contains("Align Center", cut.Markup); }
    [Fact] public void AlignRight_RendersButton() { var cut = Render<XFSpreadsheetAlignRight>(); Assert.Contains("Align Right", cut.Markup); }
    [Fact] public void InsertRow_RendersButton() { var cut = Render<XFSpreadsheetInsertRow>(); Assert.Contains("Insert Row", cut.Markup); }
    [Fact] public void InsertColumn_RendersButton() { var cut = Render<XFSpreadsheetInsertColumn>(); Assert.Contains("Insert Column", cut.Markup); }
    [Fact] public void AutoFilter_RendersButton() { var cut = Render<XFSpreadsheetAutoFilter>(); Assert.Contains("Auto Filter", cut.Markup); }
    [Fact] public void Sort_RendersButton() { var cut = Render<XFSpreadsheetSort>(); Assert.Contains("Sort", cut.Markup); }
    [Fact] public void IncreaseDecimals_RendersButton() { var cut = Render<XFSpreadsheetIncreaseDecimals>(); Assert.Contains("Increase Decimals", cut.Markup); }
    [Fact] public void DecreaseDecimals_RendersButton() { var cut = Render<XFSpreadsheetDecreaseDecimals>(); Assert.Contains("Decrease Decimals", cut.Markup); }
    // Test disabled state
    [Fact] public void Bold_Disabled_ShowsDisabled() { var cut = Render<XFSpreadsheetBold>(p => { p.Add(p => p.Disabled, true); }); Assert.Contains("disabled", cut.Markup); }
}
