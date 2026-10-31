using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFCepInput : XForgeComponentBase
    {
        private string _raw = "";
        private CepResult? Result;

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public EventCallback<CepResult> OnCepFound { get; set; }

        private string Formatted => Format(_raw);

        private void OnInput(ChangeEventArgs e)
        {
            var v = e.Value?.ToString() ?? "";
            var digits = new string(v.Where(char.IsDigit).ToArray());
            _raw = digits.Substring(0, Math.Min(8, digits.Length));
            Value = _raw;
            ValueChanged.InvokeAsync(_raw);
        }

        private static string Format(string s)
        {
            if (s.Length <= 5) return s;
            return $"{s.Substring(0, 5)}-{s.Substring(5)}";
        }

        private async Task Lookup()
        {
            Result = new CepResult("Rua Exemplo", "Centro", "Sao Paulo", "SP", _raw);
            await OnCepFound.InvokeAsync(Result);
        }
    }
}
