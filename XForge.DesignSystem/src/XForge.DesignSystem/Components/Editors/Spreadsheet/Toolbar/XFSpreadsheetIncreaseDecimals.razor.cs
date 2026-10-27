namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Increase decimals toolbar button. Increases the number of decimal places shown in the selected cells.
    /// </summary>
    public partial class XFSpreadsheetIncreaseDecimals : XFSpreadsheetToolBase
    {
        public XFSpreadsheetIncreaseDecimals()
        {
            Command = "increaseDecimals";
            Title = "Increase Decimals";
            Icon = "+.0";
        }
    }
}
