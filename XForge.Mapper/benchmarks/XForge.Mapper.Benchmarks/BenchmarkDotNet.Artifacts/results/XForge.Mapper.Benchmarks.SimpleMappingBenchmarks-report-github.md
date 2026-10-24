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
| AutoMapper         | 67.84 ns | 21.691 ns | 1.189 ns |  1.00 |    0.02 | 0.0067 |      56 B |        1.00 |
| XForge             | 60.54 ns |  6.077 ns | 0.333 ns |  0.89 |    0.01 | 0.0067 |      56 B |        1.00 |
| Mapster            | 21.63 ns |  8.510 ns | 0.466 ns |  0.32 |    0.01 | 0.0067 |      56 B |        1.00 |
| Mapperly_SourceGen | 10.94 ns |  8.677 ns | 0.476 ns |  0.16 |    0.01 | 0.0067 |      56 B |        1.00 |
