namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Undo toolbar button. Reverts the last action performed in the spreadsheet.
    /// </summary>
    public partial class XFSpreadsheetUndo : XFSpreadsheetToolBase
    {
        public XFSpreadsheetUndo()
        {
            Command = "undo";
            Title = "Undo";
            Icon = "\u21a9";
        }
    }
}
