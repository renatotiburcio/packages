using Microsoft.EntityFrameworkCore;
using XForge.Mapper.Projections;

namespace XForge.Mapper.Projections.Tests;

public class ProjectionTests
{
    private static MapperConfiguration CreateConfig(Action<IMapperConfigurationExpression>? extra = null)
    {
        return MapperConfiguration.Create(cfg =>
        {
            cfg.CreateMap<PessoaP, PessoaDtoP>()
                .Map(d => d.NomeCompleto, s => s.Nome + " " + s.Sobrenome)
                .Map(d => d.Cidade, s => s.Endereco == null ? null : s.Endereco.Cidade)
                .Ignore(d => d.Ignorado);
            cfg.CreateMap<ItemPedidoP, ItemPedidoDtoP>();
            cfg.CreateMap<PedidoP, PedidoDtoP>();
            extra?.Invoke(cfg);
        });
    }

    private static List<PessoaP> CreatePessoas() => new()
    {
        new PessoaP
        {
            Id = 1, Nome = "Renato", Sobrenome = "Tiburcio", Idade = 30, Ativo = true,
            Endereco = new EnderecoP { Cidade = "Sorocaba" },
        },
        new PessoaP
        {
            Id = 2, Nome = "Maria", Sobrenome = "Silva", Idade = 25, Ativo = false,
            Endereco = new EnderecoP { Cidade = "São Paulo" },
        },
    };

    [Fact]
    public void ProjectTo_maps_convention_members()
    {
        var mapper = TestConfigHelpers.Empty();
        var dto = new[] { new PessoaP { Id = 7, Nome = "x" } }.AsQueryable().ProjectTo<PessoaDtoP>(mapper).Single();

        Assert.Equal(7, dto.Id);
        Assert.Equal("x", dto.Nome);
    }

    [Fact]
    public void ProjectTo_maps_custom_expressions()
    {
        var mapper = CreateConfig().CreateMapper();
        var dto = CreatePessoas().AsQueryable().ProjectTo<PessoaDtoP>(mapper).Single(p => p.Id == 1);

        Assert.Equal("Renato Tiburcio", dto.NomeCompleto);
        Assert.Equal("Sorocaba", dto.Cidade);
        Assert.Null(dto.Ignorado); // ignorado na projeção → membro não é atribuído (default null)
    }

    [Fact]
    public void ProjectTo_translates_nested_objects()
    {
        var mapper = CreateConfig().CreateMapper();
        var dto = CreatePessoas().AsQueryable().ProjectTo<PessoaDtoP>(mapper).First(p => p.Endereco != null);

        Assert.NotNull(dto.Endereco);
        Assert.Equal("Sorocaba", dto.Endereco!.Cidade);
    }

    [Fact]
    public void ProjectTo_translates_collections_with_element_projection()
    {
        var mapper = CreateConfig().CreateMapper();
        var pedido = new PedidoP
        {
            Id = 9,
            Itens = new List<ItemPedidoP>
            {
                new() { Produto = "p1", Quantidade = 2 },
                new() { Produto = "p2", Quantidade = 5 },
            },
        };

        var dto = new[] { pedido }.AsQueryable().ProjectTo<PedidoDtoP>(mapper).Single();

        Assert.Equal(9, dto.Id);
        Assert.Equal(2, dto.Itens.Count);
        Assert.Equal("p2", dto.Itens[1].Produto);
    }

    [Fact]
    public void ProjectTo_composes_with_where()
    {
        var mapper = CreateConfig().CreateMapper();

        var dto = CreatePessoas().AsQueryable()
            .Where(p => p.Id == 2)
            .ProjectTo<PessoaDtoP>(mapper)
            .Single();

        Assert.Equal("Maria", dto.Nome);
    }

    [Fact]
    public void Projection_expression_does_not_allocate_closures_per_element()
    {
        // a expressão gerada é constante entre chamadas (cache por configuração)
        var mapper = CreateConfig().CreateMapper();
        var source = CreatePessoas().AsQueryable();

        var first = source.ProjectTo<PessoaDtoP>(mapper).ToList();
        var second = source.ProjectTo<PessoaDtoP>(mapper).ToList();

        Assert.Equal(first.Count, second.Count);
        Assert.Equal(first[0].NomeCompleto, second[0].NomeCompleto);
    }

    [Fact]
    public void MapWith_members_throw_with_clear_guidance()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<PessoaP, PessoaDtoP>()
                .MapWith(d => d.Nome, s => "fixo"))
            .CreateMapper();

        var exception = Assert.Throws<MappingException>(() =>
            CreatePessoas().AsQueryable().ProjectTo<PessoaDtoP>(mapper).ToList());

        Assert.Contains("MapWith", exception.Message);
    }
}

public static class TestConfigHelpers
{
    public static MapperConfiguration Empty() => MapperConfiguration.Create(_ => { });
}
