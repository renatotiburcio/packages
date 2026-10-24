global using Xunit;

namespace XForge.Mapper.Projections.Tests;

public class PessoaP
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sobrenome { get; set; }
    public int Idade { get; set; }
    public EnderecoP? Endereco { get; set; }
    public bool Ativo { get; set; }
}

public class EnderecoP
{
    public string Cidade { get; set; } = string.Empty;
    public string? Uf { get; set; }
}

public class PessoaDtoP
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string? Cidade { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Ignorado { get; set; }
    public EnderecoDtoP? Endereco { get; set; }
}

public class EnderecoDtoP
{
    public string Cidade { get; set; } = string.Empty;
}

public class ItemPedidoP
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class ItemPedidoDtoP
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class PedidoP
{
    public int Id { get; set; }
    public List<ItemPedidoP> Itens { get; set; } = new();
}

public class PedidoDtoP
{
    public int Id { get; set; }
    public List<ItemPedidoDtoP> Itens { get; set; } = new();
}

public record PedidoRecord(int Id, decimal Total);
