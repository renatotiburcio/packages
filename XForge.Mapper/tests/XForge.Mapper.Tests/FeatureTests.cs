using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mapper.Tests;

public class PolymorphismTests
{
    [Fact]
    public void Runtime_source_type_dispatches_to_derived_map()
    {
        var mapper = MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<BaseEntidade, BaseDto>();
            cfg.CreateMap<EntidadeDerivada, DerivadaDto>()
                .IncludeBase<BaseEntidade, BaseDto>();
        }).CreateMapper();

        BaseEntidade fonte = new EntidadeDerivada { IdBase = 1, Extra = "extra" };

        var dto = ((IMapper)mapper).Map<BaseDto>(fonte);

        var derivada = Assert.IsType<DerivadaDto>(dto);
        Assert.Equal(1, derivada.IdBase);
        Assert.Equal("extra", derivada.Extra);
    }

    [Fact]
    public void Include_registers_the_association_for_validation()
    {
        var configuration = MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<BaseEntidade, BaseDto>()
                .Include<EntidadeDerivada, DerivadaDto>();
            cfg.CreateMap<EntidadeDerivada, DerivadaDto>();
        });

        configuration.Validate(); // não lança
        Assert.True(true);
    }
}

public class CollectionTests
{
    private readonly IMapper _mapper = MapperConfiguration.Create(cfg =>
    {
        cfg.CreateMap<Pessoa, PessoaDto>();
        cfg.CreateMap<ListaFonte, ListaDestino>();
    }).CreateMapper();

    [Fact]
    public void List_maps_to_list()
    {
        var dto = _mapper.Map<ListaDestino>(new ListaFonte { Nomes = new List<string> { "a", "b" } });
        Assert.Equal(new[] { "a", "b" }, dto.Nomes);
    }

    [Fact]
    public void Array_maps_to_list()
    {
        var dto = _mapper.Map<ListaDestino>(new ListaFonte { Nomes = new[] { "x" } });
        Assert.Equal(new[] { "x" }, dto.Nomes);
    }

    [Fact]
    public void List_of_objects_maps_with_nested_plan()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<DonoLista, DonoListaDto>()).CreateMapper();

        var dto = mapper.Map<DonoListaDto>(new DonoLista
        {
            Pessoas = new List<Pessoa>
            {
                new() { Nome = "a" },
                new() { Nome = "b" },
            },
        });

        Assert.Equal(2, dto.Pessoas!.Count);
        Assert.Equal("b", dto.Pessoas[1].Nome);
    }

    [Fact]
    public void Dictionary_maps_with_key_and_value_converters()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<DicionarioFonte, DicionarioDestino>()).CreateMapper();

        var dto = mapper.Map<DicionarioDestino>(new DicionarioFonte
        {
            PorId = new Dictionary<int, string> { [1] = "um", [2] = "dois" },
        });

        Assert.Equal("dois", dto.PorId![2]);
    }

    [Fact]
    public void MapList_creates_a_new_list()
    {
        var dtos = _mapper.MapList<Pessoa, PessoaDto>(new[]
        {
            new Pessoa { Nome = "a" },
            new Pessoa { Nome = "b" },
        });

        Assert.Equal(2, dtos.Count);
        Assert.Equal("b", dtos[1].Nome);
    }

    public class ListaFonte
    {
        public IEnumerable<string> Nomes { get; set; } = Enumerable.Empty<string>();
    }

    public class ListaDestino
    {
        public List<string> Nomes { get; set; } = new();
    }

    public class DonoLista
    {
        public List<Pessoa> Pessoas { get; set; } = new();
    }

    public class DonoListaDto
    {
        public List<PessoaDto>? Pessoas { get; set; }
    }

    public class DicionarioFonte
    {
        public Dictionary<int, string> PorId { get; set; } = new();
    }

    public class DicionarioDestino
    {
        public Dictionary<int, string>? PorId { get; set; }
    }
}

public class EnumAndConversionTests
{
    private readonly IMapper _mapper = MapperConfiguration.Create(cfg =>
        cfg.CreateMap<Origem7, Destino7>()).CreateMapper();

    [Fact]
    public void Enum_maps_to_string_by_name()
    {
        var dto = _mapper.Map<Destino7>(new Origem7 { Tipo = TipoPessoa.Juridica, Numero = 30 });
        Assert.Equal("Juridica", dto.Tipo);
        Assert.Equal("30", dto.Numero);
    }

    [Fact]
    public void Enum_maps_to_number()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Origem7, DestinoEnumNumero>()).CreateMapper();

        var dto = mapper.Map<DestinoEnumNumero>(new Origem7 { Tipo = TipoPessoa.Juridica });
        Assert.Equal(1, dto.Tipo);
    }

    [Fact]
    public void String_maps_to_enum()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<OrigemTexto, DestinoEnum>()).CreateMapper();

        var dto = mapper.Map<DestinoEnum>(new OrigemTexto { Tipo = "juridica" });
        Assert.Equal(TipoPessoa.Juridica, dto.Tipo);
    }

    [Fact]
    public void String_maps_to_number()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<OrigemTextoNumero, DestinoNumero>()).CreateMapper();

        var dto = mapper.Map<DestinoNumero>(new OrigemTextoNumero { Numero = "42" });
        Assert.Equal(42, dto.Numero);
    }

    public class OrigemTextoNumero
    {
        public string Numero { get; set; } = string.Empty;
    }

    public class Origem7
    {
        public TipoPessoa Tipo { get; set; }
        public int Numero { get; set; }
    }

    public class Destino7
    {
        public string Tipo { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }

    public class DestinoEnumNumero
    {
        public int Tipo { get; set; }
    }

    public class DestinoNumero
    {
        public int Numero { get; set; }
    }

    public class OrigemTexto
    {
        public string Tipo { get; set; } = string.Empty;
    }

    public class DestinoEnum
    {
        public TipoPessoa Tipo { get; set; }
    }

    [Fact]
    public void Nullable_conversions_work_both_ways()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Origem8, Destino8>()).CreateMapper();

        var comValor = mapper.Map<Destino8>(new Origem8 { IdadeObrigatoria = 25 });
        var semValor = mapper.Map<Destino8>(new Origem8 { IdadeObrigatoria = null });

        Assert.Equal(25, comValor.IdadeObrigatoria);
        Assert.Equal(0, semValor.IdadeObrigatoria);
        Assert.Equal(30, mapper.Map<Destino8>(new Origem8 { Idade = 30 }).Idade);
    }

    public class Origem8
    {
        public int? IdadeObrigatoria { get; set; }
        public int Idade { get; set; }
    }

    public class Destino8
    {
        public int IdadeObrigatoria { get; set; }
        public int? Idade { get; set; }
    }
}

public class CircularAndDepthTests
{
    [Fact]
    public void Circular_references_terminate_and_share_instances()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<ComCiclo, ComCicloDto>()).CreateMapper();

        var a = new ComCiclo { Nome = "a" };
        var b = new ComCiclo { Nome = "b", Outro = a };
        a.Outro = b;

        var dto = mapper.Map<ComCicloDto>(a);

        Assert.Equal("a", dto.Nome);
        Assert.Equal("b", dto.Outro!.Nome);
        Assert.Same(dto, dto.Outro.Outro); // instância compartilhada: o ciclo fecha no mesmo DTO
    }

    public class ComCicloDto
    {
        public string Nome { get; set; } = string.Empty;
        public ComCicloDto? Outro { get; set; }
    }

    [Fact]
    public void MaxDepth_guards_against_runaway_recursion()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<NoLimit, NoLimitDto>().MaxDepth(2)).CreateMapper();

        var raiz = new NoLimit { Nome = "0", Amigo = new NoLimit { Nome = "1", Amigo = new NoLimit { Nome = "2" } } };

        var dto = mapper.Map<NoLimitDto>(raiz);

        Assert.Equal("0", dto.Nome);
        Assert.Equal("1", dto.Amigo!.Nome);
        Assert.Null(dto.Amigo.Amigo); // profundidade 2 cortada
    }

    public class NoLimitDto
    {
        public string Nome { get; set; } = string.Empty;
        public NoLimitDto? Amigo { get; set; }
    }
}

public class MergeTests
{
    [Fact]
    public void Map_onto_existing_instance_preserves_unmapped_members()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Pessoa, PessoaDto>()).CreateMapper();

        var existing = new PessoaDto { Ignorado = "mantido", Nome = "antigo" };
        mapper.Map(new Pessoa { Nome = "novo", Id = 3 }, existing);

        Assert.Equal("novo", existing.Nome);
        Assert.Equal(3, existing.Id);
        Assert.Equal("mantido", existing.Ignorado); // sem binding → preservado
    }

    [Fact]
    public void Map_onto_existing_runs_callbacks()
    {
        var chamou = false;

        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte9, Destino9>()
                .AfterMap((s, d) => chamou = true))
            .CreateMapper();

        var destino = new Destino9();
        mapper.Map(new Fonte9 { Valor = "v" }, destino);

        Assert.True(chamou);
        Assert.Equal("v", destino.Valor);
    }

    public class Fonte9
    {
        public string Valor { get; set; } = string.Empty;
    }

    public class Destino9
    {
        public string Valor { get; set; } = string.Empty;
    }
}

public class ReverseMapTests
{
    [Fact]
    public void TwoWays_maps_both_directions()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Pedido, PedidoDto>().TwoWays()).CreateMapper();

        var dto = mapper.Map<PedidoDto>(new Pedido(1, "acme", 10m));
        var volta = mapper.Map<Pedido>(dto);

        Assert.Equal("acme", dto.Cliente);
        Assert.Equal(1, volta.Numero);
        Assert.Equal(10m, volta.Total);
    }

    public record PedidoDto(int Numero, string Cliente, decimal Total);
}

public class ValidationTests
{
    [Fact]
    public void Validate_passes_with_valid_configuration()
    {
        var configuration = TestConfig.Create();
        configuration.Validate(); // não lança
    }

    [Fact]
    public void RequireDestinationMemberSource_reports_unmapped_members()
    {
        var configuration = MapperConfiguration.Create(cfg =>
        {
            cfg.RequireDestinationMemberSource = true;
            cfg.CreateMap<FonteV, DestinoV>();
        });

        var exception = Assert.Throws<MappingValidationException>(() => configuration.Validate());

        Assert.Contains(exception.Errors, e => e.Contains("SemMap"));
    }

    [Fact]
    public void Ignored_members_do_not_fail_validation()
    {
        var configuration = MapperConfiguration.Create(cfg =>
        {
            cfg.RequireDestinationMemberSource = true;
            cfg.CreateMap<FonteV, DestinoV>().Ignore(d => d.SemMap);
        });

        configuration.Validate(); // não lança
    }

    [Fact]
    public void Missing_constructor_fails_validation()
    {
        var configuration = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<object, SemConstrutor>());

        Assert.Throws<MappingValidationException>(() => configuration.Validate());
    }

    public class FonteV
    {
        public string Publico { get; set; } = string.Empty;
    }

    public class DestinoV
    {
        public string Publico { get; set; } = string.Empty;
        public string SemMap { get; set; } = string.Empty;
    }

    public class SemConstrutor
    {
        private SemConstrutor() { }
        public string Nome { get; } = string.Empty;
    }
}

public class DiAndFluentUsageTests
{
    [Fact]
    public void AddXForgeMapper_resolves_IMapper_as_singleton()
    {
        var provider = new ServiceCollection()
            .AddXForgeMapper(cfg => cfg.CreateMap<Pessoa, PessoaDto>())
            .BuildServiceProvider();

        var mapper1 = provider.GetRequiredService<IMapper>();
        var mapper2 = provider.GetRequiredService<IMapper>();

        Assert.Same(mapper1, mapper2);

        var dto = mapper1.Map<PessoaDto>(new Pessoa { Nome = "di" });
        Assert.Equal("di", dto.Nome);
    }

    [Fact]
    public void Fluent_MapTo_uses_the_injected_mapper()
    {
        var provider = new ServiceCollection()
            .AddXForgeMapper(cfg => cfg.CreateMap<Pessoa, PessoaDto>())
            .BuildServiceProvider();

        var mapper = provider.GetRequiredService<IMapper>();
        var dto = new Pessoa { Nome = "fluent" }.MapTo<PessoaDto>(mapper);

        Assert.Equal("fluent", dto.Nome);
    }

    [Fact]
    public void Fluent_MapOver_and_MapListTo_work()
    {
        var mapper = MapperConfiguration.Create(cfg => cfg.CreateMap<Pessoa, PessoaDto>()).CreateMapper();

        var destino = new PessoaDto();
        var resultado = new Pessoa { Nome = "merge" }.MapOver(destino, mapper);

        Assert.Same(destino, resultado);
        Assert.Equal("merge", resultado.Nome);

        var lista = new[] { new Pessoa { Nome = "1" }, new Pessoa { Nome = "2" } }.MapListTo<PessoaDto>(mapper);
        Assert.Equal(2, lista.Count);
    }

    [Fact]
    public void Static_default_mapper_handles_convention_mapping()
    {
        // sem configuração nenhuma: convenção + nested automático
        var dto = new Pessoa { Nome = "static", Endereco = new Endereco { Cidade = "sp" } }
            .MapTo<ConvencaoDto>();

        Assert.Equal("static", dto.Nome);
        Assert.Equal("sp", dto.Endereco!.Cidade);
    }

    public class ConvencaoDto
    {
        public string Nome { get; set; } = string.Empty;
        public EnderecoDto? Endereco { get; set; }
    }

    public class EnderecoDto
    {
        public string Cidade { get; set; } = string.Empty;
    }

    [Fact]
    public void Mapping_with_no_convention_matches_and_no_map_throws()
    {
        var mapper = MapperConfiguration.Create(_ => { }).CreateMapper();

        var exception = Assert.Throws<MappingException>(() => mapper.Map<CompletelyDifferent>(new Pessoa()));

        Assert.Contains("Missing map", exception.Message);
    }

    public class CompletelyDifferent
    {
        public string Xyz { get; set; } = string.Empty;
    }
}
