using Sample.Domain;

namespace Sample.Infrastructure;

/// <summary>Implementação em memória do repositório (o sample evita dependência de banco).</summary>
public sealed class InMemoryClienteRepository : IClienteRepository
{
    private readonly List<Cliente> _clientes = new();
    private int _nextId = 1;
    private readonly object _gate = new();

    public void Add(Cliente cliente)
    {
        lock (_gate)
        {
            cliente.Id = _nextId++;
            _clientes.Add(cliente);
        }
    }

    public Cliente? FindById(int id)
    {
        lock (_gate)
        {
            return _clientes.FirstOrDefault(cliente => cliente.Id == id);
        }
    }

    public IReadOnlyList<Cliente> List()
    {
        lock (_gate)
        {
            return _clientes.ToArray();
        }
    }
}
