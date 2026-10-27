namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Bold formatting toolbar button. Applies bold styling to the selected cells.
    /// </summary>
    public partial class XFSpreadsheetBold : XFSpreadsheetToolBase
    {
        public XFSpreadsheetBold()
        {
            Command = "bold";
            Title = "Bold";
            Icon = "B";
        }
    }
}
