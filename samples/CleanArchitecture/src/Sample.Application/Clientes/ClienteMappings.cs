using Sample.Domain;
using XForge.Mapper.Generators;

namespace Sample.Application.Clientes;

/// <summary>
/// Mapeamentos do cliente — o corpo dos métodos parciais é gerado em compile-time pelo
/// XForge.Mapper.SourceGenerator (convenções idênticas ao motor de runtime).
/// </summary>
[XForgeMap]
public partial class ClienteMappings
{
    public partial ClienteDto Map(Cliente source);
}
