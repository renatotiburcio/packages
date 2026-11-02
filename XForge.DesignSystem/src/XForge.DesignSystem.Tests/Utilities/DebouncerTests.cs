using System.Threading;
using XForge.DesignSystem.Components.Utilities.Timing;
using Xunit;
namespace XForge.DesignSystem.Tests.Utilities;
[Collection("TimingSensitive")]
public class DebouncerTests
{
    [Fact] public void Debouncer_Starts_With_Zero_Pending() { var d = new Debouncer(50, () => { }); Assert.Equal(0, d.PendingCount); }
    [Fact] public void Debouncer_Trigger_Increments_Pending() { var d = new Debouncer(50, () => { }); d.Trigger(); Assert.Equal(1, d.PendingCount); }
    // O timer de 50ms roda em thread do pool; sob a execucao paralela do xUnit o pool pode
    // levar segundos para escalonar o callback. A espera e generosa porque o que o teste
    // afirma e a coalescencia (3 triggers -> 1 acao), nao a latencia de 50ms.
    private const long EsperaMaximaMs = 10_000;

    private static bool EsperarAte(ref int contador, long limiteMs)
    {
        var limite = Environment.TickCount64 + limiteMs;
        while (Volatile.Read(ref contador) == 0 && Environment.TickCount64 < limite)
        {
            Thread.Sleep(10);
        }

        return Volatile.Read(ref contador) > 0;
    }

    [Fact] public void Debouncer_Multiple_Triggers_Coalesce_To_Single_Action() { int n = 0; using var d = new Debouncer(50, () => n++); d.Trigger(); d.Trigger(); d.Trigger(); var disparou = EsperarAte(ref n, EsperaMaximaMs); Assert.True(disparou, $"a acao do debouncer nao disparou em {EsperaMaximaMs}ms"); Assert.Equal(1, Volatile.Read(ref n)); }
    [Fact] public void Debouncer_Cancel_Resets_Pending() { var d = new Debouncer(50, () => { }); d.Trigger(); d.Cancel(); Assert.Equal(0, d.PendingCount); }
    [Fact] public void Debouncer_Action_Fires_After_Delay() { int n = 0; using var d = new Debouncer(50, () => n++); d.Trigger();
        // espera limitada: timer de 50ms em thread do pool pode atrasar sob carga
        var disparou = EsperarAte(ref n, EsperaMaximaMs);
        Assert.True(disparou, $"a acao do debouncer nao disparou em {EsperaMaximaMs}ms");
        Assert.Equal(1, Volatile.Read(ref n)); }
    [Fact] public void Throttler_Starts_With_Zero_Count() { var t = new Throttler(50, () => { }); Assert.Equal(0, t.ThrottleCount); }
    [Fact] public void Throttler_Trigger_Increments_Count() { var t = new Throttler(50, () => { }); t.Trigger(); Assert.Equal(1, t.ThrottleCount); }
    [Fact] public void Throttler_Cancel_Resets() { var t = new Throttler(50, () => { }); t.Trigger(); t.Cancel(); Assert.Equal(0, t.ThrottleCount); }
}