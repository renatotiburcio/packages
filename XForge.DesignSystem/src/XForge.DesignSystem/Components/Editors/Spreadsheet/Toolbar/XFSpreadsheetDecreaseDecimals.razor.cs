namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Decrease decimals toolbar button. Decreases the number of decimal places shown in the selected cells.
    /// </summary>
    public partial class XFSpreadsheetDecreaseDecimals : XFSpreadsheetToolBase
    {
        public XFSpreadsheetDecreaseDecimals()
        {
            Command = "decreaseDecimals";
            Title = "Decrease Decimals";
            Icon = "-.0";
        }
    }
}
