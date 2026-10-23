global using Xunit;

namespace XForge.Mapper.Tests;

// ---------- modelos compartilhados ----------

public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sobrenome { get; set; }
    public int Idade { get; set; }
    public Endereco? Endereco { get; set; }
    public TipoPessoa Tipo { get; set; }
}

public class Endereco
{
    public string Cidade { get; set; } = string.Empty;
    public string? Uf { get; set; }
}

public class PessoaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string? Cidade { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Ignorado { get; set; } = string.Empty;
    public string? Extra { get; set; }
}

public enum TipoPessoa
{
    Fisica,
    Juridica,
}

public record Pedido(int Numero, string Cliente, decimal Total);

public class BaseEntidade
{
    public int IdBase { get; set; }
}

public class EntidadeDerivada : BaseEntidade
{
    public string Extra { get; set; } = string.Empty;
}

public class BaseDto
{
    public int IdBase { get; set; }
}

public class DerivadaDto : BaseDto
{
    public string Extra { get; set; } = string.Empty;
}

public class NoLimit
{
    public NoLimit? Amigo { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ComCiclo
{
    public string Nome { get; set; } = string.Empty;
    public ComCiclo? Outro { get; set; }
}

// ---------- fábricas de configuração ----------

public static class TestConfig
{
    public static MapperConfiguration Create(Action<IMapperConfigurationExpression>? extra = null)
    {
        return MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<Pessoa, PessoaDto>()
                .Map(d => d.Cidade, s => s.Endereco == null ? null : s.Endereco.Cidade)
                .Map(d => d.NomeCompleto, s => $"{s.Nome} {s.Sobrenome}".Trim())
                .Map(d => d.Tipo, s => s.Tipo.ToString())
                .Ignore(d => d.Ignorado)
                .Ignore(d => d.Extra);

            cfg.CreateMap<Endereco, PessoaDto>(); // não usado — só para variar

            extra?.Invoke(cfg);
        });
    }
}
