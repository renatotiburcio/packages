using Sample.Domain;
using XForge.Mediator;

namespace Sample.Application.Clientes;

/// <summary>Handler da query — devolve o DTO mapeado pelo generator em compile-time.</summary>
public sealed class ObterClienteHandler : IRequestHandler<ObterClienteQuery, ClienteDto?>
{
    private readonly IClienteRepository _repository;

    public ObterClienteHandler(IClienteRepository repository) => _repository = repository;

    public ValueTask<ClienteDto?> Handle(ObterClienteQuery request, CancellationToken cancellationToken)
    {
        var cliente = _repository.FindById(request.Id);

        if (cliente is null)
        {
            return ValueTask.FromResult<ClienteDto?>(null);
        }

        // Mapeamento tradicional: a projeçã~o gerada é usada pelo mapper de runtime quando
        // o tipo concreto não é conhecido em compile-time (dispatch polimórfico/roadmap).
        var mappings = new ClienteMappings();
        return ValueTask.FromResult(mappings.Map(cliente));
    }
}
