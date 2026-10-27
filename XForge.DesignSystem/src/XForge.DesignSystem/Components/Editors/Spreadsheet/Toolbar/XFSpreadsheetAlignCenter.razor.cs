namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Align center toolbar button. Aligns the content of the selected cells to the center.
    /// </summary>
    public partial class XFSpreadsheetAlignCenter : XFSpreadsheetToolBase
    {
        public XFSpreadsheetAlignCenter()
        {
            Command = "justifyCenter";
            Title = "Align Center";
            Icon = "\u2b0c";
        }
    }
}
