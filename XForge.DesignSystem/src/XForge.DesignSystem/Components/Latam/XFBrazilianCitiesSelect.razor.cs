using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFBrazilianCitiesSelect : XForgeComponentBase
    {
        [Parameter] public string? Uf { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private IReadOnlyList<string> Cities => Uf switch
        {
            "SP" => new List<string> { "Sao Paulo", "Guarulhos", "Campinas", "Sao Bernardo do Campo", "Santo Andre", "Osasco", "Ribeirao Preto", "Sorocaba" },
            "RJ" => new List<string> { "Rio de Janeiro", "Sao Goncalo", "Duque de Caxias", "Nova Iguacu", "Niteroi" },
            "MG" => new List<string> { "Belo Horizonte", "Uberlandia", "Contagem", "Juiz de Fora" },
            _ => new List<string>()
        };
    }
}
