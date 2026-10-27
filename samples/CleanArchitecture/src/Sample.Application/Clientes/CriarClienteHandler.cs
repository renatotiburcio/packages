using Sample.Domain;
using XForge.Mediator;

namespace Sample.Application.Clientes;

/// <summary>
/// Handler do comando. Descoberto em compile-time pelo XForge.Mediator.SourceGenerator —
/// registro e dispatch sem reflection.
/// </summary>
public sealed class CriarClienteHandler : IRequestHandler<CriarClienteCommand>
{
    private readonly IClienteRepository _repository;

    public CriarClienteHandler(IClienteRepository repository) => _repository = repository;

    public ValueTask<Unit> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        _repository.Add(new Cliente { Nome = request.Nome, Email = request.Email });
        return ValueTask.FromResult(Unit.Value);
    }
}
