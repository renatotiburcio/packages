using Riok.Mapperly.Abstractions;

namespace XForge.Mapper.Benchmarks;

// =====================================================================
// Modelos compartilhados
// =====================================================================

public class PessoaB
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sobrenome { get; set; }
    public int Idade { get; set; }
    public string? Email { get; set; }
    public bool Ativo { get; set; }
}

public class PessoaDtoB
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sobrenome { get; set; }
    public int Idade { get; set; }
    public string? Email { get; set; }
    public bool Ativo { get; set; }
}

public class ClienteB
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class ClienteDtoB
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class ItemPedidoB
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class ItemPedidoDtoB
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class PedidoB
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public DateTime Data { get; set; }
    public ClienteB? Cliente { get; set; }
    public List<ItemPedidoB> Itens { get; set; } = new();
}

public class PedidoDtoB
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public DateTime Data { get; set; }
    public ClienteDtoB? Cliente { get; set; }
    public List<ItemPedidoDtoB> Itens { get; set; } = new();
}

// Computed member (FullName)
public class PessoaNomeB
{
    public string PrimeiroNome { get; set; } = string.Empty;
    public string UltimoNome { get; set; } = string.Empty;
    public int Id { get; set; }
}

public class PessoaNomeDtoB
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
}

// =====================================================================
// XForge.Mediator — n/a. XForge.Mapper: configurações
// =====================================================================

public static class MapperSetup
{
    public static MapperConfiguration CreateXForgeConfig(Action<IMapperConfigurationExpression>? extra = null)
    {
        return MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<ClienteB, ClienteDtoB>();
            cfg.CreateMap<ItemPedidoB, ItemPedidoDtoB>();
            cfg.CreateMap<PedidoB, PedidoDtoB>();
            cfg.CreateMap<PessoaB, PessoaDtoB>();
            extra?.Invoke(cfg);
        });
    }

    public static PedidoB CreatePedido() => new()
    {
        Id = 1,
        Numero = 1001,
        Data = new DateTime(2026, 1, 1),
        Cliente = new ClienteB { Id = 5, Nome = "acme", Email = "a@b.c" },
        Itens = Enumerable.Range(0, 10).Select(i => new ItemPedidoB
        {
            Produto = "prod-" + i,
            Quantidade = i + 1,
            PrecoUnitario = 10m + i,
        }).ToList(),
    };

    public static List<PessoaB> CreatePessoas(int count) => Enumerable.Range(0, count).Select(i => new PessoaB
    {
        Id = i,
        Nome = "nome-" + i,
        Sobrenome = "sob-" + i,
        Idade = 20 + i,
        Email = "e" + i + "@x.com",
        Ativo = i % 2 == 0,
    }).ToList();
}

// =====================================================================
// Mapperly (source generator)
// =====================================================================

[Mapper]
public partial class GeneratedMapper
{
    public partial PessoaDtoB Map(PessoaB source);
    public partial void Map(PessoaB source, PessoaDtoB destination);
    public partial List<PessoaDtoB> Map(List<PessoaB> source);
    public partial void Map(PedidoB source, PedidoDtoB destination);
    public partial void Map(PessoaNomeB source, PessoaNomeDtoB destination);
    public partial ClienteDtoB Map(ClienteB source);
    public partial ItemPedidoDtoB Map(ItemPedidoB source);
    public partial List<ItemPedidoDtoB> Map(List<ItemPedidoB> source);
}
