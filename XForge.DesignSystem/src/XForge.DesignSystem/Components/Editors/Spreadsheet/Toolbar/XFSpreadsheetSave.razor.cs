namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Save toolbar button. Saves the current spreadsheet document.
    /// </summary>
    public partial class XFSpreadsheetSave : XFSpreadsheetToolBase
    {
        public XFSpreadsheetSave()
        {
            Command = "save";
            Title = "Save";
            Icon = "\U0001f4be";
        }
    }
}
