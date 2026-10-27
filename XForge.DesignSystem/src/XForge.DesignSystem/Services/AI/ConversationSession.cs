using System;
using System.Collections.Generic;
namespace XForge.DesignSystem.Services.AI;
public record AIChatMessage(string Role, string Content, DateTime Timestamp);
public class ConversationSession
{
    public string Id { get; }
    public List<AIChatMessage> Messages { get; } = new();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;
    public ConversationSession(string id) { Id = id; }
    public void AddMessage(AIChatMessage msg) { Messages.Add(msg); LastUpdatedAt = DateTime.UtcNow; Trim(); }
    public void Clear() { Messages.Clear(); }
    private void Trim() { while (Messages.Count > 20) Messages.RemoveAt(0); }
    public int MessageCount => Messages.Count;
}