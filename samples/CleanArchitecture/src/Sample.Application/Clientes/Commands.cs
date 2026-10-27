using XForge.Mediator;

namespace Sample.Application.Clientes;

/// <summary>Comando: criar um cliente. Sem resposta (Unit).</summary>
public sealed record CriarClienteCommand(string Nome, string Email) : IRequest;

/// <summary>Query: buscar cliente por id.</summary>
public sealed record ObterClienteQuery(int Id) : IRequest<ClienteDto?>;

/// <summary>DTO de leitura — alvo do mapeamento gerado em compile-time.</summary>
public sealed class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
