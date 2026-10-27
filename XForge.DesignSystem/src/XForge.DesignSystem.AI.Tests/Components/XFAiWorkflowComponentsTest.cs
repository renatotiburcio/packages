using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Workflow;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFAiWorkflowComponentsTest : BunitContext
{
    #region XFAiWorkflowDesigner

    [Fact]
    public void Designer_RendersDefault()
    {
        var cut = Render<XFAiWorkflowDesigner>();

        Assert.Contains("xf-ai-workflow-designer", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Workflow Designer", cut.Markup);
    }

    [Fact]
    public void Designer_ShowsNodes()
    {
        var nodes = new List<XFAiWfNode>
        {
            new() { Label = "Inicio", Type = XFAiWfNodeType.Start, X = 10, Y = 10 },
            new() { Label = "Processar", Type = XFAiWfNodeType.AiProcessor, X = 200, Y = 10 }
        };

        var cut = Render<XFAiWorkflowDesigner>(p => p.Add(x => x.Nodes, nodes));

        Assert.Contains("Inicio", cut.Markup);
        Assert.Contains("Processar", cut.Markup);
        Assert.Contains("2 nos", cut.Markup);
    }

    [Fact]
    public void Designer_ShowsConnections()
    {
        var nodes = new List<XFAiWfNode>
        {
            new() { Id = "n1", Label = "A", Type = XFAiWfNodeType.Start, X = 10, Y = 10, Width = 160, Height = 80 },
            new() { Id = "n2", Label = "B", Type = XFAiWfNodeType.End, X = 200, Y = 10, Width = 160, Height = 80 }
        };
        var conns = new List<XFAiWfConnection>
        {
            new() { SourceNodeId = "n1", TargetNodeId = "n2" }
        };

        var cut = Render<XFAiWorkflowDesigner>(p =>
        {
            p.Add(x => x.Nodes, nodes);
            p.Add(x => x.Connections, conns);
        });

        Assert.Contains("1 arestas", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("<path", cut.Markup);
    }

    [Fact]
    public void Designer_EmptyText_Shown()
    {
        var cut = Render<XFAiWorkflowDesigner>();

        Assert.Contains("Arraste nos da paleta", cut.Markup);
    }

    [Fact]
    public void Designer_Palette_Shown()
    {
        var cut = Render<XFAiWorkflowDesigner>();

        Assert.Contains("Arrastar para adicionar:", cut.Markup);
    }

    #endregion

    #region XFAiWorkflowExecutor

    [Fact]
    public void Executor_RendersDefault()
    {
        var cut = Render<XFAiWorkflowExecutor>();

        Assert.Contains("xf-ai-workflow-executor", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Executor de Workflow", cut.Markup);
    }

    [Fact]
    public void Executor_ShowsExecutionState()
    {
        var state = new XFAiWfExecutionState
        {
            WorkflowName = "Teste",
            Status = XFAiWfExecutionStatus.Running,
            TotalNodes = 5,
            CompletedNodes = 3,
            FailedNodes = 0
        };

        var cut = Render<XFAiWorkflowExecutor>(p => p.Add(x => x.Execution, state));

        Assert.Contains("Executando", cut.Markup);
        Assert.Contains("60%", cut.Markup);
        Assert.Contains("Concluidos", cut.Markup);
    }

    [Fact]
    public void Executor_CancelButton_ShownWhenRunning()
    {
        var state = new XFAiWfExecutionState
        {
            Status = XFAiWfExecutionStatus.Running,
            TotalNodes = 3,
            CompletedNodes = 1
        };

        var cut = Render<XFAiWorkflowExecutor>(p => p.Add(x => x.Execution, state));

        Assert.Contains("Parar", cut.Markup);
    }

    [Fact]
    public void Executor_ShowsLogs()
    {
        var logs = new List<XFAiWfExecLog>
        {
            new() { NodeLabel = "Inicio", Level = XFAiWfLogLevel.Info, Message = "Workflow iniciado" },
            new() { NodeLabel = "IA", Level = XFAiWfLogLevel.Error, Message = "Timeout" }
        };
        var state = new XFAiWfExecutionState
        {
            Status = XFAiWfExecutionStatus.Failed,
            TotalNodes = 2,
            CompletedNodes = 1,
            FailedNodes = 1,
            Logs = logs
        };

        var cut = Render<XFAiWorkflowExecutor>(p => p.Add(x => x.Execution, state));

        Assert.Contains("Workflow iniciado", cut.Markup);
        Assert.Contains("Timeout", cut.Markup);
        Assert.Contains("2 entradas", cut.Markup);
    }

    [Fact]
    public void Executor_EmptyText_Shown()
    {
        var cut = Render<XFAiWorkflowExecutor>();

        Assert.Contains("Configure um workflow", cut.Markup);
    }

    #endregion

    #region XFAiAutomationRule

    [Fact]
    public void AutomationRule_RendersDefault()
    {
        var cut = Render<XFAiAutomationRule>();

        Assert.Contains("xf-ai-automation-rule", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Regras de Automacao", cut.Markup);
    }

    [Fact]
    public void AutomationRule_ShowsRules()
    {
        var rules = new List<XFAiAutomationRuleItem>
        {
            new()
            {
                Name = "Alerta de Estoque Baixo",
                Description = "Notificar quando estoque < 10",
                Enabled = true,
                Priority = 1,
                Conditions = new List<XFAiCondition>
                {
                    new() { Field = "estoque", Operator = XFAiConditionOperator.LessThan, Value = "10" }
                },
                Actions = new List<XFAiAutomationAction>
                {
                    new() { Type = XFAiAutomationActionType.SendNotification, Description = "Notificar compras" }
                }
            }
        };

        var cut = Render<XFAiAutomationRule>(p => p.Add(x => x.Rules, rules));

        Assert.Contains("Alerta de Estoque Baixo", cut.Markup);
        Assert.Contains("Notificar quando estoque", cut.Markup);
        Assert.Contains("Ativa", cut.Markup);
        Assert.Contains("estoque", cut.Markup);
        Assert.Contains("Notificar compras", cut.Markup);
    }

    [Fact]
    public void AutomationRule_ShowsAiBadge()
    {
        var rules = new List<XFAiAutomationRuleItem>
        {
            new()
            {
                Name = "Regra IA",
                AiEnhancementPrompt = "Otimizar automaticamente",
                Enabled = true,
                Conditions = new List<XFAiCondition>(),
                Actions = new List<XFAiAutomationAction>()
            }
        };

        var cut = Render<XFAiAutomationRule>(p => p.Add(x => x.Rules, rules));

        Assert.Contains("IA", cut.Markup);
    }

    [Fact]
    public void AutomationRule_EmptyText_Shown()
    {
        var cut = Render<XFAiAutomationRule>();

        Assert.Contains("Nenhuma regra configurada.", cut.Markup);
    }

    [Fact]
    public void AutomationRule_ConditionsWithLogicalOperator()
    {
        var rules = new List<XFAiAutomationRuleItem>
        {
            new()
            {
                Name = "Multipla",
                LogicalOperator = XFAiLogicalOperator.Or,
                Conditions = new List<XFAiCondition>
                {
                    new() { Field = "valor", Operator = XFAiConditionOperator.GreaterThan, Value = "1000" },
                    new() { Field = "urgente", Operator = XFAiConditionOperator.Equals, Value = "true" }
                },
                Actions = new List<XFAiAutomationAction>
                {
                    new() { Type = XFAiAutomationActionType.ExecuteWorkflow }
                }
            }
        };

        var cut = Render<XFAiAutomationRule>(p => p.Add(x => x.Rules, rules));

        Assert.Contains("valor", cut.Markup);
        Assert.Contains("urgente", cut.Markup);
        Assert.Contains("OU", cut.Markup);
    }

    #endregion

    #region XFAiBatchProcessor

    [Fact]
    public void BatchProcessor_RendersDefault()
    {
        var cut = Render<XFAiBatchProcessor>();

        Assert.Contains("xf-ai-batch-processor", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Processador em Lote", cut.Markup);
    }

    [Fact]
    public void BatchProcessor_ShowsItems()
    {
        var items = new List<XFAiBatchItem>
        {
            new() { Label = "NFe-001", Status = XFAiBatchItemStatus.Completed, Result = "OK" },
            new() { Label = "NFe-002", Status = XFAiBatchItemStatus.Failed, ErrorMessage = "Timeout" }
        };

        var cut = Render<XFAiBatchProcessor>(p => p.Add(x => x.Items, items));

        Assert.Contains("NFe-001", cut.Markup);
        Assert.Contains("NFe-002", cut.Markup);
        Assert.Contains("2 itens", cut.Markup);
    }

    [Fact]
    public void BatchProcessor_ShowsProgress()
    {
        var state = new XFAiBatchState
        {
            OperationName = "Importar NFe",
            Status = XFAiWfExecutionStatus.Running,
            TotalItems = 100,
            ProcessedItems = 45,
            SuccessfulItems = 40,
            FailedItems = 5
        };

        var cut = Render<XFAiBatchProcessor>(p =>
        {
            p.Add(x => x.Items, new List<XFAiBatchItem>());
            p.Add(x => x.State, state);
        });

        Assert.Contains("45 / 100", cut.Markup);
        Assert.Contains("Processando", cut.Markup);
        Assert.Contains("Sucesso", cut.Markup);
        Assert.Contains("Falha", cut.Markup);
    }

    [Fact]
    public void BatchProcessor_EmptyText_Shown()
    {
        var cut = Render<XFAiBatchProcessor>();

        Assert.Contains("Nenhum item para processar.", cut.Markup);
    }

    #endregion

    #region XFAiScheduler

    [Fact]
    public void Scheduler_RendersDefault()
    {
        var cut = Render<XFAiScheduler>();

        Assert.Contains("xf-ai-scheduler", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Agendador de Tarefas", cut.Markup);
    }

    [Fact]
    public void Scheduler_ShowsJobs()
    {
        var jobs = new List<XFAiScheduledJob>
        {
            new()
            {
                Name = "Backup Diario",
                Description = "Backup do banco de dados",
                Frequency = XFAiScheduleFrequency.Daily,
                Status = XFAiScheduledJobStatus.Active,
                NextRunAt = new DateTime(2026, 5, 21, 2, 0, 0),
                TotalRuns = 30,
                SuccessfulRuns = 29,
                FailedRuns = 1
            }
        };

        var cut = Render<XFAiScheduler>(p => p.Add(x => x.Jobs, jobs));

        Assert.Contains("Backup Diario", cut.Markup);
        Assert.Contains("Backup do banco", cut.Markup);
        Assert.Contains("Ativo", cut.Markup);
        Assert.Contains("Diario", cut.Markup);
    }

    [Fact]
    public void Scheduler_ShowsAiBadge()
    {
        var jobs = new List<XFAiScheduledJob>
        {
            new()
            {
                Name = "IA Otimizado",
                Frequency = XFAiScheduleFrequency.Daily,
                Status = XFAiScheduledJobStatus.Active,
                AiEnhanced = true,
                AiSuggestedSchedule = "Executar as 3h para menor impacto"
            }
        };

        var cut = Render<XFAiScheduler>(p => p.Add(x => x.Jobs, jobs));

        Assert.Contains("IA", cut.Markup);
        Assert.Contains("Sugestao IA:", cut.Markup);
        Assert.Contains("menor impacto", cut.Markup);
    }

    [Fact]
    public void Scheduler_EmptyText_Shown()
    {
        var cut = Render<XFAiScheduler>();

        Assert.Contains("Nenhuma tarefa agendada.", cut.Markup);
    }

    [Fact]
    public void Scheduler_Calendar_Renders()
    {
        var cut = Render<XFAiScheduler>();

        Assert.Contains("grid grid-cols-7", cut.Markup);
    }

    #endregion
}
