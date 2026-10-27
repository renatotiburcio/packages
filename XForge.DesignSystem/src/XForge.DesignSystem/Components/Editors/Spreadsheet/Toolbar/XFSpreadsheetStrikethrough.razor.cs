namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Strikethrough formatting toolbar button. Applies strikethrough styling to the selected cells.
    /// </summary>
    public partial class XFSpreadsheetStrikethrough : XFSpreadsheetToolBase
    {
        public XFSpreadsheetStrikethrough()
        {
            Command = "strikethrough";
            Title = "Strikethrough";
            Icon = "S";
        }
    }
}
