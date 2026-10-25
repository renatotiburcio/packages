global using Xunit;

namespace XForge.Validation.Tests;

public class PessoaV
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sobrenome { get; set; }
    public int Idade { get; set; }
    public string? Email { get; set; }
    public string? Cpf { get; set; }
    public TipoCliente Tipo { get; set; }
    public decimal Saldo { get; set; }
    public string? Cidade { get; set; }
    public string? Senha { get; set; }
    public string? ConfirmacaoSenha { get; set; }
    public List<string>? Tags { get; set; }
    public List<PedidoV>? Pedidos { get; set; }
    public EnderecoV? Endereco { get; set; }
}

public class EnderecoV
{
    public string Cidade { get; set; } = string.Empty;
    public string? Uf { get; set; }
}

public class PedidoV
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Total { get; set; }
}

public enum TipoCliente
{
    PessoaFisica = 1,
    PessoaJuridica = 2,
}
