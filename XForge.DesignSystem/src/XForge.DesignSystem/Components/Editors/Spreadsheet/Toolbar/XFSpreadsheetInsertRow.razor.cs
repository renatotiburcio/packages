namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Insert row toolbar button. Inserts a new row at the current position.
    /// </summary>
    public partial class XFSpreadsheetInsertRow : XFSpreadsheetToolBase
    {
        public XFSpreadsheetInsertRow()
        {
            Command = "insertRow";
            Title = "Insert Row";
            Icon = "+";
        }
    }
}
