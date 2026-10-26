using Xunit;

namespace XForge.Mapper.Tests;

// =====================================================================
// Inlining recursivo de planos (v0.2): grafos aninhados e coleções compilados
// em uma única expression tree por plano, com fallback clássico para ciclos.
// =====================================================================

public class InliningTests
{
    [Fact]
    public void Deeply_nested_graphs_are_inlined_and_mapped()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<PedidoV2, PedidoV2Dto>()).CreateMapper();

        var pedido = new PedidoV2
        {
            Id = 7,
            Cliente = new ClienteV2
            {
                Nome = "acme",
                Endereco = new EnderecoV2 { Cidade = "Recife", Uf = "PE" },
            },
            Itens = new List<ItemV2>
            {
                new() { Produto = "teclado", Quantidade = 2, Preco = 199.9m },
                new() { Produto = "mouse", Quantidade = 1, Preco = 89.5m },
            },
        };

        var dto = mapper.Map<PedidoV2Dto>(pedido);

        Assert.Equal(7, dto.Id);
        Assert.Equal("acme", dto.Cliente!.Nome);
        Assert.Equal("Recife", dto.Cliente.Endereco!.Cidade);
        Assert.Equal("PE", dto.Cliente.Endereco.Uf);
        Assert.Equal(2, dto.Itens.Count);
        Assert.Equal("teclado", dto.Itens[0].Produto);
        Assert.Equal(199.9m, dto.Itens[0].Preco);
        Assert.Equal("mouse", dto.Itens[1].Produto);
    }

    [Fact]
    public void Nested_nulls_propagate_as_null_in_inlined_graphs()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<PedidoV2, PedidoV2Dto>()).CreateMapper();

        var dto = mapper.Map<PedidoV2Dto>(new PedidoV2 { Id = 1, Cliente = null, Itens = null! });

        Assert.Null(dto.Cliente);
        Assert.Null(dto.Itens);
    }

    [Fact]
    public void Diamond_dependencies_map_without_false_cycle_detection()
    {
        // o mesmo par aninhado aparece em dois membros — diamante não é ciclo
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Fonte10, Fonte10Dto>()).CreateMapper();

        var endereco = new EnderecoV2 { Cidade = "Olinda", Uf = "PE" };
        var fonte = new Fonte10 { Residencial = endereco, Comercial = new EnderecoV2 { Cidade = "Cabo", Uf = "PE" } };

        var dto = mapper.Map<Fonte10Dto>(fonte);

        Assert.Equal("Olinda", dto.Residencial!.Cidade);
        Assert.Equal("Cabo", dto.Comercial!.Cidade);
        Assert.NotSame(dto.Residencial, dto.Comercial);
    }

    [Fact]
    public void IgnoreIf_member_coexists_with_inlined_members()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte11, Fonte11Dto>()
                .IgnoreIf(d => d.Apelido, s => s.Idade < 18)).CreateMapper();

        var menor = mapper.Map<Fonte11Dto>(new Fonte11 { Idade = 15, Apelido = "x", Email = "e@x.com" });
        Assert.Equal(string.Empty, menor.Apelido); // condicional → não atribuído
        Assert.Equal("e@x.com", menor.Email);      // membro inline normal

        var maior = mapper.Map<Fonte11Dto>(new Fonte11 { Idade = 30, Apelido = "x", Email = "e@x.com" });
        Assert.Equal("x", maior.Apelido);
    }

    [Fact]
    public void Merge_onto_existing_with_inlined_nested_and_collections()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<PedidoV2, PedidoV2Dto>()).CreateMapper();

        var destino = new PedidoV2Dto { Id = 0, Observacao = "mantida" };
        mapper.Map(new PedidoV2
        {
            Id = 9,
            Cliente = new ClienteV2 { Nome = "acme" },
            Itens = new List<ItemV2> { new() { Produto = "cabo", Quantidade = 3, Preco = 10m } },
        }, destino);

        Assert.Equal(9, destino.Id);
        Assert.Equal("mantida", destino.Observacao); // sem origem → preservado no merge
        Assert.Equal("acme", destino.Cliente!.Nome);
        Assert.Single(destino.Itens);
    }

    [Fact]
    public void Explicit_nested_map_overrides_are_honored_when_inlined()
    {
        var mapper = MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<ClienteV2, ClienteV2Dto>().Map(d => d.Nome, s => s.Nome + " LTDA");
            cfg.CreateMap<PedidoV2, PedidoV2Dto>();
        }).CreateMapper();

        var dto = mapper.Map<PedidoV2Dto>(new PedidoV2
        {
            Id = 1,
            Cliente = new ClienteV2 { Nome = "acme" },
            Itens = new List<ItemV2>(),
        });

        Assert.Equal("acme LTDA", dto.Cliente!.Nome); // override do mapa aninhado aplicado no inlining
    }

    [Fact]
    public void Self_referencing_same_type_members_share_source_instances()
    {
        // tipo idêntico → atribuição direta da referência (sem clone e sem recursão — sem ciclo)
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<NoLimit, NoLimit>()).CreateMapper();

        var raiz = new NoLimit { Nome = "raiz" };
        raiz.Amigo = raiz;

        var dto = mapper.Map<NoLimit>(raiz);

        Assert.Equal("raiz", dto.Nome);
        Assert.Same(raiz, dto.Amigo); // mesmo tipo → referência compartilhada com a origem (paridade v0.1)
    }

    [Fact]
    public void Collection_elements_with_nested_children_are_fully_inlined()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Frota, FrotaDto>()).CreateMapper();

        var frota = new Frota
        {
            Veiculos = new List<Veiculo>
            {
                new() { Placa = "AAA-111", Motorista = new PessoaV2 { Nome = "ana" } },
                new() { Placa = "BBB-222", Motorista = new PessoaV2 { Nome = "bruno" } },
            },
        };

        var dto = mapper.Map<FrotaDto>(frota);

        Assert.Equal(2, dto.Veiculos.Count);
        Assert.Equal("AAA-111", dto.Veiculos[0].Placa);
        Assert.Equal("ana", dto.Veiculos[0].Motorista!.Nome);
        Assert.Equal("bruno", dto.Veiculos[1].Motorista!.Nome);
    }

    [Fact]
    public void Array_and_interface_collection_destinations_are_inlined()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Fonte12, Fonte12Dto>()).CreateMapper();

        var fonte = new Fonte12
        {
            Numeros = new List<int> { 1, 2, 3 },
            Nomes = new List<string> { "a", "b" },
        };

        var dto = mapper.Map<Fonte12Dto>(fonte);

        Assert.Equal(new[] { 1, 2, 3 }, dto.Numeros);
        Assert.Equal(2, dto.Nomes.Count);
        Assert.Equal("b", dto.Nomes[1]);
    }

    [Fact]
    public void HashSet_destination_is_inlined()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Fonte13, Fonte13Dto>()).CreateMapper();

        var dto = mapper.Map<Fonte13Dto>(new Fonte13 { Codigos = new List<int> { 5, 5, 6 } });

        Assert.Equal(2, dto.Codigos.Count); // HashSet deduplica
        Assert.Contains(5, dto.Codigos);
        Assert.Contains(6, dto.Codigos);
    }

    [Fact]
    public void BeforeMap_mutating_source_is_reflected_in_inlined_population()
    {
        // com BeforeMap o plano usa o caminho dividido (factory → Before → população),
        // preservando a semântica de callbacks que modificam a origem antes do mapeamento
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte14, Fonte14Dto>()
                .BeforeMap((s, d) => s.Valor = "modificado")).CreateMapper();

        var dto = mapper.Map<Fonte14Dto>(new Fonte14 { Valor = "original" });

        Assert.Equal("modificado", dto.Valor);
    }

    [Fact]
    public void Unmappable_nested_member_throws_missing_map_at_runtime()
    {
        // par aninhado sem nenhum membro compatível: inlining não se aplica; o resolver
        // clássico sintetiza o plano e lança "Missing map" em runtime (paridade v0.1)
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Fonte15, Fonte15Dto>()).CreateMapper();

        var exception = Assert.Throws<MappingException>(() =>
            mapper.Map<Fonte15Dto>(new Fonte15 { Filho = new FilhoIncompativel { CodigoXyz = 1 } }));

        Assert.Contains("Missing map", exception.Message);
    }

    [Fact]
    public void Record_source_with_nested_and_collections_is_inlined()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Contrato, ContratoDto>()).CreateMapper();

        var dto = mapper.Map<ContratoDto>(new Contrato(
            42,
            new ClienteV2 { Nome = "acme" },
            new List<ItemV2> { new() { Produto = "licença", Quantidade = 1, Preco = 500m } }));

        Assert.Equal(42, dto.Numero);
        Assert.Equal("acme", dto.Cliente!.Nome);
        Assert.Equal("licença", dto.Itens[0].Produto);
    }

    // ---------- modelos ----------

    public class PedidoV2
    {
        public int Id { get; set; }
        public ClienteV2? Cliente { get; set; }
        public List<ItemV2> Itens { get; set; } = new();
    }

    public class PedidoV2Dto
    {
        public int Id { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public ClienteV2Dto? Cliente { get; set; }
        public List<ItemV2Dto>? Itens { get; set; }
    }

    public class ClienteV2
    {
        public string Nome { get; set; } = string.Empty;
        public EnderecoV2? Endereco { get; set; }
    }

    public class ClienteV2Dto
    {
        public string Nome { get; set; } = string.Empty;
        public EnderecoV2Dto? Endereco { get; set; }
    }

    public class EnderecoV2
    {
        public string Cidade { get; set; } = string.Empty;
        public string? Uf { get; set; }
    }

    public class EnderecoV2Dto
    {
        public string Cidade { get; set; } = string.Empty;
        public string? Uf { get; set; }
    }

    public class ItemV2
    {
        public string Produto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }

    public class ItemV2Dto
    {
        public string Produto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }

    public class Fonte10
    {
        public EnderecoV2? Residencial { get; set; }
        public EnderecoV2? Comercial { get; set; }
    }

    public class Fonte10Dto
    {
        public EnderecoV2Dto? Residencial { get; set; }
        public EnderecoV2Dto? Comercial { get; set; }
    }

    public class Fonte11
    {
        public int Idade { get; set; }
        public string Apelido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Fonte11Dto
    {
        public int Idade { get; set; }
        public string Apelido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Frota
    {
        public List<Veiculo> Veiculos { get; set; } = new();
    }

    public class FrotaDto
    {
        public List<VeiculoDto> Veiculos { get; set; } = new();
    }

    public class Veiculo
    {
        public string Placa { get; set; } = string.Empty;
        public PessoaV2? Motorista { get; set; }
    }

    public class VeiculoDto
    {
        public string Placa { get; set; } = string.Empty;
        public PessoaV2Dto? Motorista { get; set; }
    }

    public class PessoaV2
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class PessoaV2Dto
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class Fonte12
    {
        public List<int> Numeros { get; set; } = new();
        public List<string> Nomes { get; set; } = new();
    }

    public class Fonte12Dto
    {
        public int[] Numeros { get; set; } = Array.Empty<int>();
        public IReadOnlyList<string> Nomes { get; set; } = new List<string>();
    }

    public class Fonte13
    {
        public List<int> Codigos { get; set; } = new();
    }

    public class Fonte13Dto
    {
        public HashSet<int> Codigos { get; set; } = new();
    }

    public class Fonte14
    {
        public string Valor { get; set; } = string.Empty;
    }

    public class Fonte14Dto
    {
        public string Valor { get; set; } = string.Empty;
    }

    public class Fonte15
    {
        public FilhoIncompativel? Filho { get; set; }
    }

    public class Fonte15Dto
    {
        public FilhoIncompativelDto? Filho { get; set; }
    }

    public class FilhoIncompativel
    {
        public int CodigoXyz { get; set; }
    }

    public class FilhoIncompativelDto
    {
        public string DescricaoWxy { get; set; } = string.Empty;
    }

    public record Contrato(int Numero, ClienteV2? Cliente, List<ItemV2> Itens);

    public class ContratoDto
    {
        public int Numero { get; set; }
        public ClienteV2Dto? Cliente { get; set; }
        public List<ItemV2Dto> Itens { get; set; } = new();
    }
}
