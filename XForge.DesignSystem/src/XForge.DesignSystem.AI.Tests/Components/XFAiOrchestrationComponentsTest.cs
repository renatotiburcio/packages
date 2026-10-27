#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Orchestration;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-AI006 AI Orchestration components:
/// XFAiAgentConsole.
/// Covers rendering, parameters, states, hierarchy, filtering, accessibility, and models.
/// </summary>
public class XFAiOrchestrationComponentsTest : BunitContext
{
    public XFAiOrchestrationComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiAgentConsole

    [Fact]
    public void AgentConsole_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiAgentConsole>();
        Assert.Contains("Console de Agentes", cut.Markup);
        Assert.Contains("xf-ai-agent-console", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_CustomTitle()
    {
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Title, "Orquestracao Enterprise"));
        Assert.Contains("Orquestracao Enterprise", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_WithAgents()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Id = "ceo-1", Name = "CEO Agent", Role = XFAiAgentRole.CEO, Status = XFAiAgentStatus.Running },
            new() { Id = "dir-1", Name = "Director Financeiro", Role = XFAiAgentRole.Director, Status = XFAiAgentStatus.Idle, ParentId = "ceo-1" },
            new() { Id = "spec-1", Name = "Especialista NFe", Role = XFAiAgentRole.Specialist, Status = XFAiAgentStatus.Running, ParentId = "dir-1" },
            new() { Id = "w-1", Name = "Worker OCR", Role = XFAiAgentRole.Worker, Status = XFAiAgentStatus.Idle, ParentId = "spec-1" }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("CEO Agent", cut.Markup);
        Assert.Contains("Director Financeiro", cut.Markup);
        Assert.Contains("Especialista NFe", cut.Markup);
        Assert.Contains("Worker OCR", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_WithLogs()
    {
        var logs = new List<XFAiAgentLogEntry>
        {
            new() { AgentId = "ceo-1", AgentName = "CEO Agent", Level = XFAiAgentLogLevel.Info, Message = "Tarefa delegada ao diretor" },
            new() { AgentId = "dir-1", AgentName = "Director", Level = XFAiAgentLogLevel.Tool, Message = "Executando analise", ToolName = "analyze_data" },
            new() { AgentId = "spec-1", AgentName = "Especialista", Level = XFAiAgentLogLevel.Error, Message = "Timeout na requisicao" }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Logs, logs));
        Assert.Contains("Tarefa delegada ao diretor", cut.Markup);
        Assert.Contains("analyze_data", cut.Markup);
        Assert.Contains("Timeout na requisicao", cut.Markup);
    }

    [Fact]
    public void AgentConsole_ShowsCostSummary()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "Agent A", Role = XFAiAgentRole.Worker, TotalTokens = 5000, CostUsd = 0.15m, ToolCalls = 10, MemoryEntries = 3 },
            new() { Name = "Agent B", Role = XFAiAgentRole.Worker, TotalTokens = 3000, CostUsd = 0.08m, ToolCalls = 5, MemoryEntries = 1 }
        };
        var cut = Render<XFAiAgentConsole>(p =>
        {
            p.Add(x => x.Agents, agents);
            p.Add(x => x.ShowCostSummary, true);
        });
        Assert.Contains("Custo Total", cut.Markup);
        Assert.Contains("Tool Calls", cut.Markup);
        Assert.Contains("Memorias", cut.Markup);
        Assert.Contains("Fila", cut.Markup);
    }

    [Fact]
    public void AgentConsole_HidesCostSummary_WhenDisabled()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "Agent A", TotalTokens = 5000, CostUsd = 0.15m }
        };
        var cut = Render<XFAiAgentConsole>(p =>
        {
            p.Add(x => x.Agents, agents);
            p.Add(x => x.ShowCostSummary, false);
        });
        Assert.DoesNotContain("Custo Total", cut.Markup);
    }

    [Fact]
    public void AgentConsole_HidesHeader_WhenDisabled()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "A1", Status = XFAiAgentStatus.Running }
        };
        var cut = Render<XFAiAgentConsole>(p =>
        {
            p.Add(x => x.ShowHeader, false);
            p.Add(x => x.Agents, agents);
        });
        // Header-specific elements should be hidden
        Assert.DoesNotContain("ativo", cut.Markup);
    }

    [Fact]
    public void AgentConsole_HidesAgents_WhenDisabled()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "Agent A" }
        };
        var cut = Render<XFAiAgentConsole>(p =>
        {
            p.Add(x => x.Agents, agents);
            p.Add(x => x.ShowAgents, false);
        });
        Assert.DoesNotContain("Agent A", cut.Markup);
    }

    [Fact]
    public void AgentConsole_ShowsRunningCount()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "A1", Status = XFAiAgentStatus.Running },
            new() { Name = "A2", Status = XFAiAgentStatus.Running },
            new() { Name = "A3", Status = XFAiAgentStatus.Idle }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("2 ativos", cut.Markup);
    }

    [Fact]
    public void AgentConsole_ShowsErrorCount()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "A1", Status = XFAiAgentStatus.Error },
            new() { Name = "A2", Status = XFAiAgentStatus.Error },
            new() { Name = "A3", Status = XFAiAgentStatus.Idle }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("2 com erro", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_RoleLabels()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "CEO", Role = XFAiAgentRole.CEO },
            new() { Name = "Dir", Role = XFAiAgentRole.Director },
            new() { Name = "Spec", Role = XFAiAgentRole.Specialist },
            new() { Name = "Work", Role = XFAiAgentRole.Worker }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("CEO", cut.Markup);
        Assert.Contains("Diretor", cut.Markup);
        Assert.Contains("Especialista", cut.Markup);
        Assert.Contains("Worker", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_AgentModel()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "GPT Agent", Model = "gpt-4o", Role = XFAiAgentRole.Worker }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("gpt-4o", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Renders_CurrentTask()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "Worker", Status = XFAiAgentStatus.Running, CurrentTask = "Processando NF-e #12345" }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("Processando NF-e #12345", cut.Markup);
    }

    [Fact]
    public void AgentConsole_ShowsQueueDepth()
    {
        var agents = new List<XFAiAgentInfo>
        {
            new() { Name = "Busy Agent", QueueDepth = 7 }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Agents, agents));
        Assert.Contains("7 na fila", cut.Markup);
    }

    [Fact]
    public void AgentConsole_AriaRoles_Correct()
    {
        var cut = Render<XFAiAgentConsole>();
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("aria-label", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Logs_AriaLiveRegion()
    {
        var logs = new List<XFAiAgentLogEntry>
        {
            new() { AgentName = "Test", Message = "Test log" }
        };
        var cut = Render<XFAiAgentConsole>(p => p.Add(x => x.Logs, logs));
        Assert.Contains("role=\"log\"", cut.Markup);
        Assert.Contains("aria-live=\"polite\"", cut.Markup);
    }

    [Fact]
    public void AgentConsole_EmptyState_RendersDefaultText()
    {
        var cut = Render<XFAiAgentConsole>();
        Assert.Contains("Nenhum agente registrado", cut.Markup);
    }

    [Fact]
    public void AgentConsole_EmptyLogs_RendersDefaultText()
    {
        var cut = Render<XFAiAgentConsole>();
        Assert.Contains("Nenhum log de agente", cut.Markup);
    }

    [Fact]
    public void AgentConsole_Model_SuccessRate()
    {
        var agent = new XFAiAgentInfo { TasksCompleted = 80, TasksFailed = 20 };
        Assert.Equal(0.8, agent.SuccessRate, 2);
    }

    [Fact]
    public void AgentConsole_Model_ZeroTasks_SuccessRate()
    {
        var agent = new XFAiAgentInfo();
        Assert.Equal(1.0, agent.SuccessRate);
    }

    [Fact]
    public void AgentLogEntry_DefaultValues()
    {
        var entry = new XFAiAgentLogEntry();
        Assert.False(string.IsNullOrEmpty(entry.Id));
        Assert.Equal(XFAiAgentLogLevel.Info, entry.Level);
        Assert.Equal(string.Empty, entry.Message);
    }

    [Fact]
    public void AgentInfo_DefaultValues()
    {
        var agent = new XFAiAgentInfo();
        Assert.False(string.IsNullOrEmpty(agent.Id));
        Assert.Equal(XFAiAgentRole.Worker, agent.Role);
        Assert.Equal(XFAiAgentStatus.Idle, agent.Status);
    }

    #endregion

}
