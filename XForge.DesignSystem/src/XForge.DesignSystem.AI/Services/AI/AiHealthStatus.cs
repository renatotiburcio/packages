namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Result of a provider health check.
/// </summary>
/// <param name="IsHealthy"><c>true</c> if the provider responded successfully; <c>false</c> otherwise.</param>
/// <param name="Message">Optional human-readable status message (e.g., model count or error details).</param>
/// <param name="LatencyMs">Round-trip latency in milliseconds, when available.</param>
public sealed record AiHealthStatus(bool IsHealthy, string? Message = null, long? LatencyMs = null);
