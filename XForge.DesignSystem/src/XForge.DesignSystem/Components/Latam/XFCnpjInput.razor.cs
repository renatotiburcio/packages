using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFCnpjInput : XForgeComponentBase
    {
        private string _raw = "";

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private string Formatted => Format(_raw);

        private void OnInput(ChangeEventArgs e)
        {
            var v = e.Value?.ToString() ?? "";
            var digits = new string(v.Where(char.IsDigit).ToArray());
            _raw = digits.Substring(0, Math.Min(14, digits.Length));
            Value = _raw;
            ValueChanged.InvokeAsync(_raw);
        }

        private static string Format(string s)
        {
            s = new string(s.Where(char.IsDigit).ToArray());
            if (s.Length <= 2) return s;
            if (s.Length <= 5) return $"{s.Substring(0, 2)}.{s.Substring(2)}";
            if (s.Length <= 8) return $"{s.Substring(0, 2)}.{s.Substring(2, 3)}.{s.Substring(5)}";
            if (s.Length <= 12) return $"{s.Substring(0, 2)}.{s.Substring(2, 3)}.{s.Substring(5, 3)}/{s.Substring(8)}";
            return $"{s.Substring(0, 2)}.{s.Substring(2, 3)}.{s.Substring(5, 3)}/{s.Substring(8, 4)}-{s.Substring(12)}";
        }

        public static bool IsValid(string s)
        {
            s = new string(s.Where(char.IsDigit).ToArray());
            if (s.Length != 14) return false;
            if (s.All(c => c == s[0])) return false;
            int[] m1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] m2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 12; i++) sum += int.Parse(s[i].ToString()) * m1[i];
            int r1 = sum % 11;
            if (r1 < 2) r1 = 0; else r1 = 11 - r1;
            if (int.Parse(s[12].ToString()) != r1) return false;
            sum = 0;
            for (int i = 0; i < 13; i++) sum += int.Parse(s[i].ToString()) * m2[i];
            int r2 = sum % 11;
            if (r2 < 2) r2 = 0; else r2 = 11 - r2;
            return int.Parse(s[13].ToString()) == r2;
        }
    }
}
