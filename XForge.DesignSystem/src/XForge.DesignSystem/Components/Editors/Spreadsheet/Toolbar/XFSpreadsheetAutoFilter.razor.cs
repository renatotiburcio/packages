namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Auto filter toolbar button. Toggles auto-filter on the selected data range.
    /// </summary>
    public partial class XFSpreadsheetAutoFilter : XFSpreadsheetToolBase
    {
        public XFSpreadsheetAutoFilter()
        {
            Command = "autoFilter";
            Title = "Auto Filter";
            Icon = "\U0001f53d";
        }
    }
}
