```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
AMD Ryzen 7 5700G with Radeon Graphics 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host] : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v3

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
| Method             | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------- |---------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
| AutoMapper         | 293.3 ns | 206.02 ns | 11.29 ns |  1.00 |    0.05 | 0.1059 |     888 B |        1.00 |
| XForge             | 464.6 ns |  42.44 ns |  2.33 ns |  1.59 |    0.05 | 0.1106 |     928 B |        1.05 |
| Mapster            | 153.7 ns | 103.98 ns |  5.70 ns |  0.52 |    0.02 | 0.0832 |     696 B |        0.78 |
| Mapperly_SourceGen | 134.5 ns | 140.53 ns |  7.70 ns |  0.46 |    0.03 | 0.0832 |     696 B |        0.78 |
