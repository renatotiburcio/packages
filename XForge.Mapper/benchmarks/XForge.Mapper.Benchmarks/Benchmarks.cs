using BenchmarkDotNet.Attributes;
using Mapster;
using Riok.Mapperly.Abstractions;

namespace XForge.Mapper.Benchmarks;

// Todos os cenários usam ServiceLifetime/Singleton equivalentes (configuração canônica de cada lib),
// handlers/mappers pré-aquecidos no GlobalSetup e objetos que completam sincronamente.

[MemoryDiagnoser]
public class SimpleMappingBenchmarks
{
    private IMapper _xforge = null!;
    private AutoMapper.IMapper _autoMapper = null!;
    private GeneratedMapper _mapperly = null!;
    private PessoaB _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        var config = MapperSetup.CreateXForgeConfig();
        _xforge = config.CreateMapper();

        var amConfig = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<PessoaB, PessoaDtoB>());
        _autoMapper = amConfig.CreateMapper();

        _mapperly = new GeneratedMapper();
        _source = MapperSetup.CreatePessoas(1)[0];
    }

    [Benchmark(Baseline = true)]
    public PessoaDtoB AutoMapper() => _autoMapper.Map<PessoaDtoB>(_source);

    [Benchmark]
    public PessoaDtoB XForge() => _xforge.Map<PessoaDtoB>(_source);

    [Benchmark]
    public PessoaDtoB Mapster() => _source.Adapt<PessoaDtoB>();

    [Benchmark]
    public PessoaDtoB Mapperly_SourceGen() => _mapperly.Map(_source);
}

[MemoryDiagnoser]
public class ComplexMappingBenchmarks
{
    private IMapper _xforge = null!;
    private AutoMapper.IMapper _autoMapper = null!;
    private GeneratedMapper _mapperly = null!;
    private PedidoB _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        var config = MapperSetup.CreateXForgeConfig();
        _xforge = config.CreateMapper();

        var amConfig = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ClienteB, ClienteDtoB>();
            cfg.CreateMap<ItemPedidoB, ItemPedidoDtoB>();
            cfg.CreateMap<PedidoB, PedidoDtoB>();
        });
        _autoMapper = amConfig.CreateMapper();

        _mapperly = new GeneratedMapper();
        _source = MapperSetup.CreatePedido();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        (_xforge as IDisposable)?.Dispose();
    }

    [Benchmark(Baseline = true)]
    public PedidoDtoB AutoMapper() => _autoMapper.Map<PedidoDtoB>(_source);

    [Benchmark]
    public PedidoDtoB XForge() => _xforge.Map<PedidoDtoB>(_source);

    [Benchmark]
    public PedidoDtoB Mapster() => _source.Adapt<PedidoDtoB>();

    [Benchmark]
    public PedidoDtoB Mapperly_SourceGen()
    {
        var dto = new PedidoDtoB();
        _mapperly.Map(_source, dto);
        return dto;
    }
}

[MemoryDiagnoser]
public class CustomMemberBenchmarks
{
    private IMapper _xforge = null!;
    private AutoMapper.IMapper _autoMapper = null!;
    private GeneratedMapper _mapperly = null!;
    private PessoaNomeB _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        var config = MapperConfiguration.Create(cfg =>
            cfg.CreateMap<PessoaNomeB, PessoaNomeDtoB>()
                .Map(d => d.NomeCompleto, s => s.PrimeiroNome + " " + s.UltimoNome));
        _xforge = config.CreateMapper();

        var amConfig = new AutoMapper.MapperConfiguration(cfg =>
            cfg.CreateMap<PessoaNomeB, PessoaNomeDtoB>()
                .ForMember(d => d.NomeCompleto, o => o.MapFrom(s => s.PrimeiroNome + " " + s.UltimoNome)));
        _autoMapper = amConfig.CreateMapper();

        _mapperly = new GeneratedMapper();
        _source = new PessoaNomeB { Id = 1, PrimeiroNome = "Renato", UltimoNome = "Tiburcio" };
    }

    [Benchmark(Baseline = true)]
    public PessoaNomeDtoB AutoMapper() => _autoMapper.Map<PessoaNomeDtoB>(_source);

    [Benchmark]
    public PessoaNomeDtoB XForge() => _xforge.Map<PessoaNomeDtoB>(_source);

    [Benchmark]
    public PessoaNomeDtoB Mapster()
    {
        // config global registrada uma única vez no GlobalSetup (MapsterCustom.EnsureConfig)
        return _source.Adapt<PessoaNomeDtoB>();
    }

    [Benchmark]
    public PessoaNomeDtoB Mapperly_SourceGen()
    {
        var dto = new PessoaNomeDtoB();
        _mapperly.Map(_source, dto);
        return dto;
    }
}

internal static class MapsterCustom
{
    private static TypeAdapterConfig? _config;

    public static TypeAdapterSetter<PessoaNomeB, PessoaNomeDtoB> EnsureConfig()
    {
        var config = TypeAdapterConfig<PessoaNomeB, PessoaNomeDtoB>.NewConfig()
            .Map(d => d.NomeCompleto, s => s.PrimeiroNome + " " + s.UltimoNome);
        config.Compile();
        _config = config.Config;
        return config;
    }

    private static PessoaNomeDtoB MapToTarget(this TypeAdapterConfig config, PessoaNomeB source, PessoaNomeDtoB destination)
    {
        source.Adapt(destination);
        return destination;
    }
}

[MemoryDiagnoser]
public class MergeBenchmarks
{
    private IMapper _xforge = null!;
    private AutoMapper.IMapper _autoMapper = null!;
    private GeneratedMapper _mapperly = null!;
    private PessoaB _source = null!;
    private PessoaDtoB _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var config = MapperSetup.CreateXForgeConfig();
        _xforge = config.CreateMapper();

        var amConfig = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<PessoaB, PessoaDtoB>());
        _autoMapper = amConfig.CreateMapper();

        _mapperly = new GeneratedMapper();
        _source = MapperSetup.CreatePessoas(1)[0];
        _target = new PessoaDtoB();
    }

    [Benchmark(Baseline = true)]
    public PessoaDtoB AutoMapper()
    {
        var destination = new PessoaDtoB();
        _autoMapper.Map(_source, destination);
        return destination;
    }

    [Benchmark]
    public PessoaDtoB XForge()
    {
        var destination = new PessoaDtoB();
        _xforge.Map(_source, destination);
        return destination;
    }

    [Benchmark]
    public PessoaDtoB Mapster()
    {
        var destination = new PessoaDtoB();
        _source.Adapt(destination);
        return destination;
    }

    [Benchmark]
    public PessoaDtoB Mapperly_SourceGen()
    {
        var destination = new PessoaDtoB();
        _mapperly.Map(_source, destination);
        return destination;
    }
}

[MemoryDiagnoser]
public class ListMappingBenchmarks
{
    private IMapper _xforge = null!;
    private AutoMapper.IMapper _autoMapper = null!;
    private GeneratedMapper _mapperly = null!;
    private List<PessoaB> _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        var config = MapperSetup.CreateXForgeConfig();
        _xforge = config.CreateMapper();

        var amConfig = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<PessoaB, PessoaDtoB>());
        _autoMapper = amConfig.CreateMapper();

        _mapperly = new GeneratedMapper();
        _source = MapperSetup.CreatePessoas(10);
    }

    [Benchmark(Baseline = true)]
    public List<PessoaDtoB> AutoMapper() => _autoMapper.Map<List<PessoaDtoB>>(_source);

    [Benchmark]
    public List<PessoaDtoB> XForge() => _xforge.MapList<PessoaB, PessoaDtoB>(_source);

    [Benchmark]
    public List<PessoaDtoB> Mapster() => _source.Adapt<List<PessoaDtoB>>();

    [Benchmark]
    public List<PessoaDtoB> Mapperly_SourceGen() => _mapperly.Map(_source);
}
