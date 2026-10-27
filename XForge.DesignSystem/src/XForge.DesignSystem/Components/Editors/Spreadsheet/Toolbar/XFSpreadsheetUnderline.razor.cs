namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Underline formatting toolbar button. Applies underline styling to the selected cells.
    /// </summary>
    public partial class XFSpreadsheetUnderline : XFSpreadsheetToolBase
    {
        public XFSpreadsheetUnderline()
        {
            Command = "underline";
            Title = "Underline";
            Icon = "U";
        }
    }
}
