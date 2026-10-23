using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mapper.Tests;

public class ConventionTests
{
    [Fact]
    public void Members_with_matching_names_map_automatically()
    {
        var mapper = TestConfig.Create().CreateMapper();
        var dto = mapper.Map<PessoaDto>(new Pessoa { Id = 7, Nome = "Renato", Idade = 30 });

        Assert.Equal(7, dto.Id);
        Assert.Equal("Renato", dto.Nome);
        Assert.Equal(30, dto.Idade);
    }

    [Fact]
    public void Mapping_is_case_insensitive()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<OrigemCase, DestinoCase>()).CreateMapper();

        var dto = mapper.Map<DestinoCase>(new OrigemCase { nomeminusculo = "ok" });

        Assert.Equal("ok", dto.NomeMinusculo);
    }

    public class OrigemCase
    {
        public string nomeminusculo { get; set; } = string.Empty;
    }

    public class DestinoCase
    {
        public string NomeMinusculo { get; set; } = string.Empty;
    }

    [Fact]
    public void Nested_objects_map_automatically_without_explicit_CreateMap()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Dono, DonoDto>()).CreateMapper();

        var dto = mapper.Map<DonoDto>(new Dono
        {
            Nome = "dono",
            Pet = new Pet { Apelido = "rex" },
        });

        Assert.Equal("rex", dto.Pet!.Apelido);
    }

    public class Dono
    {
        public string Nome { get; set; } = string.Empty;
        public Pet? Pet { get; set; }
    }

    public class DonoDto
    {
        public string Nome { get; set; } = string.Empty;
        public PetDto? Pet { get; set; }
    }

    public class Pet
    {
        public string Apelido { get; set; } = string.Empty;
    }

    public class PetDto
    {
        public string Apelido { get; set; } = string.Empty;
    }

    [Fact]
    public void Incompatible_member_types_are_skipped_silently()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<ComTiposIncompativeis, ComTiposIncompativeisDto>()).CreateMapper();

        var dto = mapper.Map<ComTiposIncompativeisDto>(new ComTiposIncompativeis { Matriz = new int[] { 1, 2 } });

        Assert.Null(dto.Matriz); // int[] → string: sem conversão, membro fica default
    }

    public class ComTiposIncompativeis
    {
        public int[] Matriz { get; set; } = System.Array.Empty<int>();
    }

    public class ComTiposIncompativeisDto
    {
        public string? Matriz { get; set; }
    }
}

public class FluentMapTests
{
    [Fact]
    public void Map_with_expressions_computes_values()
    {
        var mapper = TestConfig.Create().CreateMapper();

        var dto = mapper.Map<PessoaDto>(new Pessoa
        {
            Nome = "Renato",
            Sobrenome = "Tiburcio",
            Endereco = new Endereco { Cidade = "Sorocaba" },
        });

        Assert.Equal("Renato Tiburcio", dto.NomeCompleto);
        Assert.Equal("Sorocaba", dto.Cidade);
    }

    [Fact]
    public void Map_with_plain_function_overload()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte, Destino>()
                .MapWith(d => d.Codigo, s => $"ID-{s.Id}"))
            .CreateMapper();

        var dto = mapper.Map<Destino>(new Fonte { Id = 42 });

        Assert.Equal("ID-42", dto.Codigo);
    }

    public class Fonte
    {
        public int Id { get; set; }
    }

    public class Destino
    {
        public string Codigo { get; set; } = string.Empty;
    }
}

public class IgnoreTests
{
    [Fact]
    public void Ignored_members_keep_defaults()
    {
        var mapper = TestConfig.Create().CreateMapper();
        var dto = mapper.Map<PessoaDto>(new Pessoa { Nome = "x" });

        Assert.Equal(string.Empty, dto.Ignorado);
        Assert.Null(dto.Extra);
    }

    [Fact]
    public void Ignore_by_name_works()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte2, Destino2>().Ignore("Segredo"))
            .CreateMapper();

        var dto = mapper.Map<Destino2>(new Fonte2 { Segredo = "s", Publico = "p" });

        Assert.Equal(string.Empty, dto.Segredo);
        Assert.Equal("p", dto.Publico);
    }

    public class Fonte2
    {
        public string Segredo { get; set; } = string.Empty;
        public string Publico { get; set; } = string.Empty;
    }

    public class Destino2
    {
        public string Segredo { get; set; } = string.Empty;
        public string Publico { get; set; } = string.Empty;
    }

    [Fact]
    public void IgnoreIf_skips_conditionally()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte3, Destino3>()
                .IgnoreIf(d => d.Apelido, s => s.EsconderApelido))
            .CreateMapper();

        var escondido = mapper.Map<Destino3>(new Fonte3 { Apelido = "x", EsconderApelido = true });
        var visivel = mapper.Map<Destino3>(new Fonte3 { Apelido = "x", EsconderApelido = false });

        Assert.Equal(string.Empty, escondido.Apelido);
        Assert.Equal("x", visivel.Apelido);
    }

    public class Fonte3
    {
        public string Apelido { get; set; } = string.Empty;
        public bool EsconderApelido { get; set; }
    }

    public class Destino3
    {
        public string Apelido { get; set; } = string.Empty;
    }
}

public class ConstructionTests
{
    [Fact]
    public void ConstructUsing_replaces_the_factory()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte4, Destino4>()
                .ConstructUsing(s => new Destino4("custom-" + s.Id)))
            .CreateMapper();

        var dto = mapper.Map<Destino4>(new Fonte4 { Id = 1 });

        Assert.Equal("custom-1", dto.Tag);
    }

    public class Fonte4
    {
        public int Id { get; set; }
    }

    public class Destino4
    {
        public Destino4(string tag) => Tag = tag;
        public string Tag { get; }
    }

    [Fact]
    public void Records_map_via_constructor_binding_by_name()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<PedidoFonte, Pedido>()).CreateMapper();

        var pedido = mapper.Map<Pedido>(new PedidoFonte { Numero = 9, Cliente = "acme", Total = 99.9m });

        Assert.Equal(9, pedido.Numero);
        Assert.Equal("acme", pedido.Cliente);
        Assert.Equal(99.9m, pedido.Total);
    }

    public record PedidoFonte
    {
        public int Numero { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}

public class CallbackTests
{
    [Fact]
    public void BeforeMap_runs_before_and_AfterMap_after_member_mapping()
    {
        var steps = new List<string>();

        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<Fonte5, Destino5>()
                .BeforeMap((s, d) => steps.Add("before"))
                .AfterMap((s, d) => steps.Add("after:" + d.Valor)))
            .CreateMapper();

        mapper.Map<Destino5>(new Fonte5 { Valor = "v" });

        Assert.Equal(new[] { "before", "after:v" }, steps);
    }

    public class Fonte5
    {
        public string Valor { get; set; } = string.Empty;
    }

    public class Destino5
    {
        public string Valor { get; set; } = string.Empty;
    }

    [Fact]
    public void Mapper_aware_callbacks_receive_the_mapper()
    {
        var mapper = MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<Item6, Item6Dto>();
            cfg.CreateMap<Dono6, Dono6Dto>()
                .AfterMap((s, d, m) => d.Itens = m.MapList<Item6Dto>(s.Itens).ToArray());
        }).CreateMapper();

        var dto = mapper.Map<Dono6Dto>(new Dono6 { Itens = new[] { new Item6 { Nome = "a" } } });

        Assert.Single(dto.Itens!);
        Assert.Equal("a", dto.Itens![0].Nome);
    }

    public class Item6
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class Item6Dto
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class Dono6
    {
        public Item6[] Itens { get; set; } = System.Array.Empty<Item6>();
    }

    public class Dono6Dto
    {
        public Item6Dto[]? Itens { get; set; }
    }
}
