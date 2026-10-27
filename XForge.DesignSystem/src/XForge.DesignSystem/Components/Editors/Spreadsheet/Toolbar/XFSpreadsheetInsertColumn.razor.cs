namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Insert column toolbar button. Inserts a new column at the current position.
    /// </summary>
    public partial class XFSpreadsheetInsertColumn : XFSpreadsheetToolBase
    {
        public XFSpreadsheetInsertColumn()
        {
            Command = "insertColumn";
            Title = "Insert Column";
            Icon = "+";
        }
    }
}
