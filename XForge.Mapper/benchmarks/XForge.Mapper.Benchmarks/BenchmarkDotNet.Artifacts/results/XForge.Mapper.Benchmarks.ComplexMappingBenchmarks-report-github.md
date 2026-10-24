```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
AMD Ryzen 7 5700G with Radeon Graphics 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host] : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v3

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
| Method             | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------- |---------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| AutoMapper         | 290.5 ns | 289.6 ns | 15.87 ns |  1.00 |    0.07 | 0.1106 |      - |     928 B |        1.00 |
| XForge             | 830.2 ns | 443.0 ns | 24.28 ns |  2.86 |    0.16 | 0.2003 | 0.0010 |    1680 B |        1.81 |
| Mapster            | 163.1 ns | 196.6 ns | 10.77 ns |  0.56 |    0.04 | 0.0880 | 0.0002 |     736 B |        0.79 |
| Mapperly_SourceGen | 149.9 ns | 169.9 ns |  9.31 ns |  0.52 |    0.04 | 0.0880 | 0.0002 |     736 B |        0.79 |
