using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFVehiclePlateInput : XForgeComponentBase
    {
        private string _raw = "";

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private string Formatted => Format(_raw);

        private void OnInput(ChangeEventArgs e)
        {
            var v = e.Value?.ToString() ?? "";
            _raw = v.ToUpper().Replace("-", "");
            if (_raw.Length > 7) _raw = _raw.Substring(0, 7);
            Value = _raw;
            ValueChanged.InvokeAsync(_raw);
        }

        private static string Format(string s)
        {
            if (s.Length <= 3) return s;
            return $"{s.Substring(0, 3)}-{s.Substring(3)}";
        }

        public static bool IsValid(string s)
        {
            var p = s.Replace("-", "").ToUpper();
            if (p.Length != 7) return false;
            return Regex.IsMatch(p, "^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$");
        }
    }
}
