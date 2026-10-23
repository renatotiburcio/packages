using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

// Execução rápida (indicativa): dotnet run -c Release -- --filter *
// Execução completa: dotnet run -c Release -- --filter * --job medium (sem in-process)
var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun.WithToolchain(BenchmarkDotNet.Toolchains.InProcess.Emit.InProcessEmitToolchain.Instance))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .WithOption(ConfigOptions.DisableOptimizationsValidator, true);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
