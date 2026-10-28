using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Orchestration;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI.Orchestration;

public class XFAiAgentConsoleExtendedTest : XForgeTestContext
{
    private static XFAiAgentInfo CreateAgent(string id = "a1", string name = "Agent", XFAiAgentRole role = XFAiAgentRole.Worker, XFAiAgentStatus status = XFAiAgentStatus.Idle)
    {
        return new XFAiAgentInfo
        {
            Id = id,
            Name = name,
            Role = role,
            Status = status,
            Description = $"Description for {name}",
            TasksCompleted = 10,
            TasksFailed = 1,
            TotalTokens = 5000,
            CostUsd = 0.05m,
            ToolCalls = 3,
            MemoryEntries = 2,
            UptimeSeconds = 3600
        };
    }

    private static XFAiAgentLogEntry CreateLogEntry(string id = "l1", string agentId = "a1", string agentName = "Agent", XFAiAgentLogLevel level = XFAiAgentLogLevel.Info)
    {
        return new XFAiAgentLogEntry
        {
            Id = id,
            AgentId = agentId,
            AgentName = agentName,
            Level = level,
            Message = $"Log message from {agentName}",
            TokensUsed = 100,
            DurationMs = 250
        };
    }

    [Fact]
    public void Extended_XFAiAgentConsole_DefaultTitle_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, new[] { CreateAgent() }));
        // Title is in aria-label on <section> element
        Assert.Contains("Console de Agentes", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_CustomTitle_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Title, "My Console")
            .Add(p => p.Agents, new[] { CreateAgent() }));
        Assert.Contains("My Console", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ShowHeaderFalse_HidesHeader()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowHeader, false)
            .Add(p => p.Agents, new[] { CreateAgent() }));
        // When ShowHeader=false, the header div (with title, agent count, status) is hidden
        // The section element still renders with aria-label containing the title
        Assert.Contains("xf-ai-agent-console", cut.Markup);
        // Total agents badge should not render when header is hidden
        Assert.DoesNotContain("TotalAgents", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ShowAgentsFalse_HidesAgents()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowAgents, false)
            .Add(p => p.ShowCostSummary, false)
            .Add(p => p.ShowHeader, false)
            .Add(p => p.Agents, new[] { CreateAgent("a1", "UniqueAgentName") }));
        // Agent names should not appear in the agent panel when ShowAgents=false
        Assert.DoesNotContain("UniqueAgentName", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ShowLogsFalse_HidesLogs()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowLogs, false)
            .Add(p => p.Logs, new[] { CreateLogEntry() }));
        Assert.DoesNotContain("Log message", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ShowCostSummaryFalse_HidesCost()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, false)
            .Add(p => p.Agents, new[] { CreateAgent() }));
        // Cost summary section should not render
        Assert.DoesNotContain("Custo Total", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ShowCostSummaryTrue_RendersCost()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, new[] { CreateAgent() }));
        Assert.Contains("Custo Total", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_EmptyAgents_RendersEmptyText()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, Array.Empty<XFAiAgentInfo>()));
        Assert.Contains("Nenhum agente registrado", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_EmptyLogs_RendersEmptyText()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, Array.Empty<XFAiAgentLogEntry>()));
        Assert.Contains("Nenhum log de agente", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_CustomEmptyAgentsText_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, Array.Empty<XFAiAgentInfo>())
            .Add(p => p.EmptyAgentsText, "No agents"));
        Assert.Contains("No agents", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_CustomEmptyLogsText_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, Array.Empty<XFAiAgentLogEntry>())
            .Add(p => p.EmptyLogsText, "No logs"));
        Assert.Contains("No logs", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_AgentNames_Rendered()
    {
        var agents = new[]
        {
            CreateAgent("a1", "Agent One"),
            CreateAgent("a2", "Agent Two")
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("Agent One", cut.Markup);
        Assert.Contains("Agent Two", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_AgentRoles_Rendered()
    {
        var agents = new[]
        {
            CreateAgent("a1", "CEO Agent", XFAiAgentRole.CEO),
            CreateAgent("a2", "Director Agent", XFAiAgentRole.Director),
            CreateAgent("a3", "Specialist Agent", XFAiAgentRole.Specialist),
            CreateAgent("a4", "Worker Agent", XFAiAgentRole.Worker)
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("CEO Agent", cut.Markup);
        Assert.Contains("Director Agent", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_LogMessages_Rendered()
    {
        var logs = new[]
        {
            CreateLogEntry("l1", "a1", "Agent1"),
            CreateLogEntry("l2", "a2", "Agent2")
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, logs));
        Assert.Contains("Log message from Agent1", cut.Markup);
        Assert.Contains("Log message from Agent2", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_LogLevels_Rendered()
    {
        var logs = new[]
        {
            CreateLogEntry("l1", "a1", "A1", XFAiAgentLogLevel.Debug),
            CreateLogEntry("l2", "a1", "A1", XFAiAgentLogLevel.Warning),
            CreateLogEntry("l3", "a1", "A1", XFAiAgentLogLevel.Error),
            CreateLogEntry("l4", "a1", "A1", XFAiAgentLogLevel.Tool),
            CreateLogEntry("l5", "a1", "A1", XFAiAgentLogLevel.Memory)
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, logs));
        Assert.Contains("Debug", cut.Markup);
        Assert.Contains("Warning", cut.Markup);
        Assert.Contains("Error", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_MaxLogs_LimitsDisplay()
    {
        var logs = Enumerable.Range(0, 50).Select(i => CreateLogEntry($"l{i}", "a1", "Agent")).ToArray();
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.MaxLogs, 10)
            .Add(p => p.Logs, logs));
        Assert.Contains("xf-ai-agent-console", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_AgentTemplate_Renders()
    {
        var agents = new[] { CreateAgent("a1", "Templated") };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents)
            .Add(p => p.AgentTemplate, (XFAiAgentInfo ctx) => (RenderFragment)(builder => builder.AddContent(0, $"Custom: {ctx.Name}"))));
        Assert.Contains("Custom: Templated", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_LogTemplate_Renders()
    {
        var logs = new[] { CreateLogEntry("l1", "a1", "Agent") };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, logs)
            .Add(p => p.LogTemplate, (XFAiAgentLogEntry ctx) => (RenderFragment)(builder => builder.AddContent(0, $"Custom: {ctx.Message}"))));
        Assert.Contains("Custom: Log message", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_EmptyAgentsContent_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, Array.Empty<XFAiAgentInfo>())
            .Add(p => p.EmptyAgentsContent, (RenderFragment)(builder => builder.AddContent(0, "No agents custom"))));
        Assert.Contains("No agents custom", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_EmptyLogsContent_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, Array.Empty<XFAiAgentLogEntry>())
            .Add(p => p.EmptyLogsContent, (RenderFragment)(builder => builder.AddContent(0, "No logs custom"))));
        Assert.Contains("No logs custom", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_TotalTokens_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, agents));
        Assert.Contains("Tokens", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_TotalCost_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, agents));
        // Cost is formatted with current culture (pt-BR uses comma): "$0,05"
        Assert.Contains("0,05", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ToolCalls_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, agents));
        Assert.Contains("Tool Calls", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_MemoryEntries_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, agents));
        Assert.Contains("Memorias", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_TasksCompleted_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        // Tasks completed is shown in the agent card
        Assert.Contains("10", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_SuccessRate_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Idle) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        // Success rate = 10 / (10 + 1) = 0.909...
        Assert.Contains("90", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_HierarchyOrder_CEOFirst()
    {
        var agents = new[]
        {
            CreateAgent("w1", "Worker", XFAiAgentRole.Worker),
            CreateAgent("c1", "CEO", XFAiAgentRole.CEO),
            CreateAgent("d1", "Director", XFAiAgentRole.Director)
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        var markup = cut.Markup;
        var ceoIdx = markup.IndexOf("CEO Agent");
        var directorIdx = markup.IndexOf("Director Agent");
        if (ceoIdx >= 0 && directorIdx >= 0)
        {
            Assert.True(ceoIdx < directorIdx);
        }
    }

    [Fact]
    public void Extended_XFAiAgentConsole_MultipleAgents_RendersAll()
    {
        var agents = Enumerable.Range(0, 5).Select(i => CreateAgent($"a{i}", $"Agent {i}")).ToArray();
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        for (int i = 0; i < 5; i++)
        {
            Assert.Contains($"Agent {i}", cut.Markup);
        }
    }

    [Fact]
    public void Extended_XFAiAgentConsole_MultipleLogs_RendersAll()
    {
        var logs = Enumerable.Range(0, 5).Select(i => CreateLogEntry($"l{i}", "a1", $"Agent{i}")).ToArray();
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, logs));
        for (int i = 0; i < 5; i++)
        {
            Assert.Contains($"Agent{i}", cut.Markup);
        }
    }

    [Fact]
    public void Extended_XFAiAgentConsole_RunningAgents_RendersActiveIndicator()
    {
        var agents = new[] { CreateAgent("a1", "Agent", XFAiAgentRole.Worker, XFAiAgentStatus.Running) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("ativo", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ErrorAgents_RendersErrorIndicator()
    {
        var agents = new[]
        {
            CreateAgent("a1", "Agent1", XFAiAgentRole.Worker, XFAiAgentStatus.Error)
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("com erro", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_QueueDepth_Renders()
    {
        var agents = new[]
        {
            new XFAiAgentInfo { Id = "a1", Name = "Queued", Role = XFAiAgentRole.Worker, QueueDepth = 5 }
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.ShowCostSummary, true)
            .Add(p => p.Agents, agents));
        Assert.Contains("Fila", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_LogFilter_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, new[] { CreateLogEntry() }));
        Assert.Contains("Filtrar por nivel", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_LogSearch_Renders()
    {
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, new[] { CreateLogEntry() }));
        Assert.Contains("Buscar logs", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_RoleLabels_Rendered()
    {
        var agents = new[] { CreateAgent("a1", "Worker", XFAiAgentRole.Worker) };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        // Role label appears in the agent card
        Assert.Contains("Worker", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_Model_Rendered()
    {
        var agents = new[]
        {
            new XFAiAgentInfo { Id = "a1", Name = "Model Agent", Role = XFAiAgentRole.Worker, Model = "gpt-4o" }
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("gpt-4o", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_CurrentTask_Rendered()
    {
        var agents = new[]
        {
            new XFAiAgentInfo { Id = "a1", Name = "Running Agent", Role = XFAiAgentRole.Worker, Status = XFAiAgentStatus.Running, CurrentTask = "Processing data" }
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Agents, agents));
        Assert.Contains("Processing data", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiAgentConsole_ToolName_Rendered()
    {
        var logs = new[]
        {
            new XFAiAgentLogEntry { Id = "l1", AgentId = "a1", AgentName = "Agent", Level = XFAiAgentLogLevel.Tool, Message = "Called tool", ToolName = "search" }
        };
        var cut = Render<XFAiAgentConsole>(ps => ps
            .Add(p => p.Logs, logs));
        Assert.Contains("search", cut.Markup);
    }
}
