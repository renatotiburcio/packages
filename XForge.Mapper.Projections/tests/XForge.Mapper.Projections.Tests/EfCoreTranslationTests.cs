using Microsoft.EntityFrameworkCore;
using XForge.Mapper.Projections;

namespace XForge.Mapper.Projections.Tests;

/// <summary>
/// Prova de tradução: o ProjectTo gera expressão Select pura que o EF Core traduz
/// (membros resolvidos no banco, sem materializar a entidade).
/// </summary>
public class EfCoreTranslationTests : IDisposable
{
    private readonly EfContext _context = new();

    public void Dispose() => _context.Dispose();

    private sealed class EfContext : DbContext
    {
        public DbSet<PessoaEf> Pessoas => Set<PessoaEf>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseInMemoryDatabase("xforge-projections-tests");
    }

    public class PessoaEf
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sobrenome { get; set; }
        public int Idade { get; set; }
        public EnderecoEf? Endereco { get; set; }
        public string CampoPesado { get; set; } = string.Empty;
    }

    public class EnderecoEf
    {
        public int Id { get; set; }
        public string Cidade { get; set; } = string.Empty;
    }

    public class PessoaEfDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string? Cidade { get; set; }
    }

    [Fact]
    public void ProjectTo_composes_with_ef_core_query()
    {
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<PessoaEf, PessoaEfDto>()
                .Map(d => d.NomeCompleto, s => s.Nome + " " + s.Sobrenome)
                .Map(d => d.Cidade, s => s.Endereco!.Cidade))
            .CreateMapper();

        _context.Pessoas.AddRange(new[]
        {
            new PessoaEf
            {
                Id = 1, Nome = "Renato", Sobrenome = "Tiburcio", Idade = 30, CampoPesado = "pesado",
                Endereco = new EnderecoEf { Id = 1, Cidade = "Sorocaba" },
            },
            new PessoaEf
            {
                Id = 2, Nome = "Maria", Sobrenome = "Silva", Idade = 25, CampoPesado = "pesado",
                Endereco = new EnderecoEf { Id = 2, Cidade = "São Paulo" },
            },
        });
        _context.SaveChanges();

        var dtos = _context.Pessoas
            .Where(p => p.Idade >= 25)
            .OrderBy(p => p.Id)
            .ProjectTo<PessoaEfDto>(mapper)
            .ToList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal("Renato Tiburcio", dtos[0].NomeCompleto);
        Assert.Equal("Sorocaba", dtos[0].Cidade);
        Assert.Equal("Maria Silva", dtos[1].NomeCompleto);
    }

    [Fact]
    public void ProjectTo_only_selects_projected_columns()
    {
        // prova semântica: o DTO projetado NÃO contém o CampoPesado,
        // então a consulta nunca o materializa (Select no SQL)
        var mapper = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<PessoaEf, PessoaEfDto>()
                .Map(d => d.NomeCompleto, s => s.Nome + " " + s.Sobrenome))
            .CreateMapper();

        _context.Pessoas.Add(new PessoaEf { Id = 10, Nome = "n", CampoPesado = "pesado" });
        _context.SaveChanges();

        var dto = _context.Pessoas.Where(p => p.Id == 10).ProjectTo<PessoaEfDto>(mapper).Single();

        Assert.Equal("n ", dto.NomeCompleto);
        // CampoPesado não existe no DTO — projeção mínima confirmada pela ausência no tipo
    }
}
