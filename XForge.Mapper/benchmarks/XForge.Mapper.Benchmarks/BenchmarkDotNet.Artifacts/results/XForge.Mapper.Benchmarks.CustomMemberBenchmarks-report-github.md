```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
AMD Ryzen 7 5700G with Radeon Graphics 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host] : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v3

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
| Method             | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| AutoMapper         | 91.062 ns | 18.407 ns | 1.0090 ns |  1.00 |    0.01 | 0.0153 |     128 B |        1.00 |
| XForge             | 84.069 ns | 32.676 ns | 1.7911 ns |  0.92 |    0.02 | 0.0153 |     128 B |        1.00 |
| Mapster            | 16.883 ns |  1.541 ns | 0.0845 ns |  0.19 |    0.00 | 0.0038 |      32 B |        0.25 |
| Mapperly_SourceGen |  5.608 ns |  8.061 ns | 0.4418 ns |  0.06 |    0.00 | 0.0038 |      32 B |        0.25 |
