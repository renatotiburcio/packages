namespace Sample.Domain;

/// <summary>Contrato de persistência (a implementação vive na Infrastructure).</summary>
public interface IClienteRepository
{
    void Add(Cliente cliente);

    Cliente? FindById(int id);

    IReadOnlyList<Cliente> List();
}
