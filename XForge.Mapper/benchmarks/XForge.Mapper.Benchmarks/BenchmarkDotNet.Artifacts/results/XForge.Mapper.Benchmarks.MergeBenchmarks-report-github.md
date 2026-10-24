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
| AutoMapper         | 66.70 ns | 40.321 ns | 2.210 ns |  1.00 |    0.04 | 0.0067 |      56 B |        1.00 |
| XForge             | 60.83 ns | 51.669 ns | 2.832 ns |  0.91 |    0.04 | 0.0067 |      56 B |        1.00 |
| Mapster            | 28.87 ns |  8.590 ns | 0.471 ns |  0.43 |    0.01 | 0.0067 |      56 B |        1.00 |
| Mapperly_SourceGen | 12.34 ns |  6.979 ns | 0.383 ns |  0.19 |    0.01 | 0.0067 |      56 B |        1.00 |
