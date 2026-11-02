using Sample.Domain;
using XForge.Mediator;

namespace Sample.Application.Clientes;

/// <summary>Handler da query de listagem — devolve os DTOs mapeados pelo generator.</summary>
public sealed class ObterClientesHandler : IRequestHandler<ObterClientesQuery, IReadOnlyList<ClienteDto>>
{
    private readonly IClienteRepository _repository;

    public ObterClientesHandler(IClienteRepository repository) => _repository = repository;

    public ValueTask<IReadOnlyList<ClienteDto>> Handle(ObterClientesQuery request, CancellationToken cancellationToken)
    {
        var mappings = new ClienteMappings();
        var clientes = _repository
            .List()
            .Select(mappings.Map)
            .ToList();

        return ValueTask.FromResult<IReadOnlyList<ClienteDto>>(clientes);
    }
}
