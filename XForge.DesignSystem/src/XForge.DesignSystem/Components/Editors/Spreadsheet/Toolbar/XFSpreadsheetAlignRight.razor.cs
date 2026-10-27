namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Align right toolbar button. Aligns the content of the selected cells to the right.
    /// </summary>
    public partial class XFSpreadsheetAlignRight : XFSpreadsheetToolBase
    {
        public XFSpreadsheetAlignRight()
        {
            Command = "justifyRight";
            Title = "Align Right";
            Icon = "\u27a1";
        }
    }
}
