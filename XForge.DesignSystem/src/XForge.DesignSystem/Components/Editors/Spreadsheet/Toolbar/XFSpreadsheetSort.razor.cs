namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Sort toolbar button. Sorts the selected data range in ascending or descending order.
    /// </summary>
    public partial class XFSpreadsheetSort : XFSpreadsheetToolBase
    {
        public XFSpreadsheetSort()
        {
            Command = "sort";
            Title = "Sort";
            Icon = "\u2195";
        }
    }
}
