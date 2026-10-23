using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace XForge.Mediator.Internal;

/// <summary>
/// Lightweight diagnostics for the dispatch hot path (ActivitySource + Meter), inspired by Arbiter.Mediation.
/// Cost is near-zero when no listener/reader is attached; can be fully disabled via
/// <c>AddXForgeMediator(cfg => cfg.EnableDiagnostics = false)</c>.
/// </summary>
internal static class XForgeMediatorDiagnostics
{
    /// <summary>ActivitySource/Meter name.</summary>
    internal const string SourceName = "XForge.Mediator";

    /// <summary>Activity names.</summary>
    internal const string SendActivityName = "mediator.send";
    internal const string PublishActivityName = "mediator.publish";

    internal const string RequestTypeTag = "xforge.request_type";
    internal const string ResponseTypeTag = "xforge.response_type";
    internal const string NotificationTypeTag = "xforge.notification_type";

    /// <summary>Master switch (set from registration options).</summary>
    internal static bool Enabled = true;

    internal static readonly string? Version = typeof(Mediator).Assembly.GetName().Version?.ToString();

    internal static readonly ActivitySource ActivitySource = new(SourceName, Version);

    internal static readonly Meter Meter = new(SourceName, Version);

    internal static readonly Counter<long> SendCounter =
        Meter.CreateCounter<long>("xforge.mediator.send.count", unit: "{request}", description: "Number of mediator send operations");

    internal static readonly Counter<long> PublishCounter =
        Meter.CreateCounter<long>("xforge.mediator.publish.count", unit: "{notification}", description: "Number of mediator publish operations");

    internal static readonly Histogram<double> SendDuration =
        Meter.CreateHistogram<double>("xforge.mediator.send.duration", unit: "ms", description: "Duration of mediator send operations");

    internal static readonly Histogram<double> PublishDuration =
        Meter.CreateHistogram<double>("xforge.mediator.publish.duration", unit: "ms", description: "Duration of mediator publish operations");

    /// <summary>Elapsed milliseconds since <paramref name="startTimestamp"/> (Stopwatch.GetTimestamp).</summary>
    internal static double ElapsedMilliseconds(long startTimestamp)
    {
#if NET8_0_OR_GREATER
        return Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
#else
        var elapsedTicks = Stopwatch.GetTimestamp() - startTimestamp;
        return elapsedTicks * 1000d / Stopwatch.Frequency;
#endif
    }
}
