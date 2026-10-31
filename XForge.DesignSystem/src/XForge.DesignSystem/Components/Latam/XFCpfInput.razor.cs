using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFCpfInput : XForgeComponentBase
    {
        private string _raw = "";

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private string Formatted => Format(_raw);

        private void OnInput(ChangeEventArgs e)
        {
            var v = e.Value?.ToString() ?? "";
            _raw = new string(v.Where(char.IsDigit).ToArray());
            if (_raw.Length > 11) _raw = _raw.Substring(0, 11);
            Value = _raw;
            ValueChanged.InvokeAsync(_raw);
        }

        private static string Format(string s)
        {
            s = new string(s.Where(char.IsDigit).ToArray());
            if (s.Length <= 3) return s;
            if (s.Length <= 6) return $"{s.Substring(0, 3)}.{s.Substring(3)}";
            if (s.Length <= 9) return $"{s.Substring(0, 3)}.{s.Substring(3, 3)}.{s.Substring(6)}";
            return $"{s.Substring(0, 3)}.{s.Substring(3, 3)}.{s.Substring(6, 3)}-{s.Substring(9)}";
        }

        public static bool IsValid(string s)
        {
            s = new string(s.Where(char.IsDigit).ToArray());
            if (s.Length != 11) return false;
            if (s.All(c => c == s[0])) return false;
            int sum = 0;
            for (int i = 0; i < 9; i++) sum += int.Parse(s[i].ToString()) * (10 - i);
            int r1 = sum % 11;
            if (r1 < 2) r1 = 0; else r1 = 11 - r1;
            if (int.Parse(s[9].ToString()) != r1) return false;
            sum = 0;
            for (int i = 0; i < 10; i++) sum += int.Parse(s[i].ToString()) * (11 - i);
            int r2 = sum % 11;
            if (r2 < 2) r2 = 0; else r2 = 11 - r2;
            return int.Parse(s[10].ToString()) == r2;
        }
    }
}
