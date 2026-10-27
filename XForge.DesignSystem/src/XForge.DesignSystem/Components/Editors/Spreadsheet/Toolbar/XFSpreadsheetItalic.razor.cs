namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Italic formatting toolbar button. Applies italic styling to the selected cells.
    /// </summary>
    public partial class XFSpreadsheetItalic : XFSpreadsheetToolBase
    {
        public XFSpreadsheetItalic()
        {
            Command = "italic";
            Title = "Italic";
            Icon = "I";
        }
    }
}
