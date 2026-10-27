namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Redo toolbar button. Reapplies the last undone action in the spreadsheet.
    /// </summary>
    public partial class XFSpreadsheetRedo : XFSpreadsheetToolBase
    {
        public XFSpreadsheetRedo()
        {
            Command = "redo";
            Title = "Redo";
            Icon = "\u21aa";
        }
    }
}
