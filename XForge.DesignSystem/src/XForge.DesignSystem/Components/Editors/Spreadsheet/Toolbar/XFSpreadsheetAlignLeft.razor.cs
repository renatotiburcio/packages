namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Align left toolbar button. Aligns the content of the selected cells to the left.
    /// </summary>
    public partial class XFSpreadsheetAlignLeft : XFSpreadsheetToolBase
    {
        public XFSpreadsheetAlignLeft()
        {
            Command = "justifyLeft";
            Title = "Align Left";
            Icon = "\u2b05";
        }
    }
}
