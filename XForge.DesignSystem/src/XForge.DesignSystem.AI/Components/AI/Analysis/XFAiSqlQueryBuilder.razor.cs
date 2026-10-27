using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Analysis;

/// <summary>
/// Enterprise AI SQL query builder. Converts natural language to SQL with
/// security validation, injection protection, read-only enforcement, and
/// approval workflow. Tailwind-first, accessible, dark/light aware.
/// </summary>
public partial class XFAiSqlQueryBuilder : XForgeComponentBase, IDisposable
{
    // -- Injection detection patterns ------------------------------------
    private static readonly Regex[] InjectionPatterns =
    [
        new(@"--", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@";", RegexOptions.Compiled),
        new(@"\b(UNION\s+ALL\s+SELECT|UNION\s+SELECT)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\b(xp_cmdshell|xp_\w+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bEXEC\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bEXECUTE\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"'\s*OR\s+'", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"'\s*OR\s+1\s*=\s*1", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bSLEEP\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bWAITFOR\s+DELAY\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bBENCHMARK\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bLOAD_FILE\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bINTO\s+OUTFILE\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bINTO\s+DUMPFILE\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
    ];

    private static readonly HashSet<string> DefaultBlockedOps = new(StringComparer.OrdinalIgnoreCase)
    {
        "DELETE", "DROP", "ALTER", "TRUNCATE", "INSERT", "UPDATE"
    };

    private static readonly HashSet<string> ReadOnlyPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "SELECT", "WITH", "SHOW", "DESCRIBE", "EXPLAIN", "DESC"
    };

    // -- State -----------------------------------------------------------
    private string? _nlQuery;
    private XFAiSqlQuery? _currentQuery;
    private bool _isLoading;
    private string? _errorMessage;
    private bool _showExplanation;
    private bool _showOptimization;
    private CancellationTokenSource? _cts;

    // -- Parameters ------------------------------------------------------

    /// <summary>AI provider for SQL generation.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Database schema context sent to the AI for accurate generation.</summary>
    [Parameter]
    public string? Schema { get; set; }

    /// <summary>Input placeholder text.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Descreva a consulta SQL que deseja...";

    /// <summary>Header title.</summary>
    [Parameter]
    public string Title { get; set; } = "Construtor de SQL";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Enforce read-only queries (blocks DML statements).</summary>
    [Parameter]
    public bool ReadOnly { get; set; } = true;

    /// <summary>Require human approval for non-read queries.</summary>
    [Parameter]
    public bool RequireApproval { get; set; }

    /// <summary>Append LIMIT clause to cap result rows.</summary>
    [Parameter]
    public int? MaxRows { get; set; }

    /// <summary>External loading state override.</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Custom system prompt override for the AI.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    /// <summary>Operations to block beyond the default set.</summary>
    [Parameter]
    public IReadOnlyList<string>? BlockedOperations { get; set; }

    /// <summary>Fired when SQL is generated and passes validation.</summary>
    [Parameter]
    public EventCallback<XFAiSqlQuery> OnSqlGenerated { get; set; }

    /// <summary>Fired on error during generation.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    /// <summary>Fired when a non-read query requires approval.</summary>
    [Parameter]
    public EventCallback<XFAiSqlQuery> OnApprovalRequired { get; set; }

    /// <summary>Custom SQL preview template.</summary>
    [Parameter]
    public RenderFragment<XFAiSqlQuery>? SqlPreview { get; set; }

    /// <summary>Custom header content (replaces default header).</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    // -- Computed --------------------------------------------------------

    private bool IsBusy => _isLoading || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanGenerate => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(_nlQuery);
    private bool HasQuery => _currentQuery is not null && !string.IsNullOrWhiteSpace(_currentQuery.GeneratedSql);
    private bool HasError => !string.IsNullOrWhiteSpace(_errorMessage);

    private IReadOnlyList<string> EffectiveBlockedOps =>
        BlockedOperations is { Count: > 0 }
            ? DefaultBlockedOps.Union(BlockedOperations, StringComparer.OrdinalIgnoreCase).ToList()
            : DefaultBlockedOps.ToList();

    // -- Lifecycle -------------------------------------------------------

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Loading != _isLoading)
        {
            _isLoading = Loading;
        }
    }

    // -- CSS Classes -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-sql-query-builder flex min-h-[28rem] flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string HeaderTitleClass =>
        "inline-flex items-center gap-2 text-sm font-semibold text-text-primary";

    private string ComposerClass =>
        "flex items-end gap-2 border-b border-border-light bg-surface-50 p-3";

    private string TextareaClass =>
        "min-h-10 flex-1 resize-none rounded-lg border border-surface-200 bg-bg-surface px-3 py-2 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";

    private string GenerateButtonClass =>
        "inline-flex h-10 min-w-10 items-center justify-center gap-1.5 rounded-lg bg-primary px-3 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";

    private string ResultContainerClass =>
        "flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto p-4 scroll-smooth";

    private string EmptyContainerClass =>
        "grid min-h-40 flex-1 place-items-center px-6 py-8";

    private string EmptyInnerClass =>
        "flex flex-col items-center gap-3 text-center";

    private string EmptyIconClass =>
        "inline-flex h-12 w-12 items-center justify-center rounded-full bg-surface-100 text-text-muted";

    private string EmptyTextClass =>
        "text-sm text-text-secondary max-w-xs";

    private string SqlCodeBlockClass =>
        "relative overflow-x-auto rounded-lg bg-surface-950 p-4 text-sm leading-6";

    private string SqlCodeClass =>
        "font-mono text-emerald-400 whitespace-pre-wrap break-all";

    private string CopyButtonClass =>
        "absolute right-2 top-2 inline-flex h-7 w-7 items-center justify-center rounded-md bg-surface-800 text-text-muted outline-none transition-all duration-150 hover:bg-surface-700 hover:text-text-primary focus-visible:ring-2 focus-visible:ring-border-focus";

    private string ValidationValidClass =>
        "flex items-center gap-2 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-xs text-emerald-700";

    private string ValidationInvalidClass =>
        "flex items-center gap-2 rounded-lg border border-danger-200 bg-danger-50 px-3 py-2 text-xs text-danger";

    private string ValidationWarningClass =>
        "flex items-center gap-2 rounded-lg border border-amber-200 bg-amber-50 px-3 py-2 text-xs text-amber-700";

    private string InfoSectionClass =>
        "rounded-lg border border-border-light bg-surface-50";

    private string InfoHeaderClass =>
        "flex cursor-pointer items-center justify-between gap-2 px-3 py-2 text-xs font-medium text-text-secondary select-none hover:bg-surface-100 transition-colors";

    private string InfoContentClass =>
        "border-t border-border-light px-3 py-2 text-xs text-text-secondary leading-5";

    private string TablesContainerClass =>
        "flex flex-wrap gap-1.5";

    private string TableBadgeClass =>
        "inline-flex items-center gap-1 rounded-full bg-primary/10 px-2 py-0.5 text-[11px] font-medium text-primary";

    private string ConfidenceBarContainerClass =>
        "flex items-center gap-2";

    private string ConfidenceBarTrackClass =>
        "h-1.5 w-20 overflow-hidden rounded-full bg-surface-200";

    private string ConfidenceBarFillClass =>
        "h-full rounded-full bg-primary transition-all duration-300";

    private string ConfidenceLabelClass =>
        "text-[10px] font-medium text-text-muted tabular-nums";

    private string ApprovalButtonClass =>
        "inline-flex items-center gap-1.5 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm font-semibold text-amber-800 outline-none transition-all duration-150 hover:bg-amber-100 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2";

    private string ErrorBannerClass =>
        "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";

    // -- Keyboard & Input ------------------------------------------------

    private void HandleInput(ChangeEventArgs args)
    {
        _nlQuery = args.Value?.ToString();
        StateHasChanged();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        // Escape clears the input
        if (args.Key == "Escape")
        {
            _nlQuery = string.Empty;
            _currentQuery = null;
            _errorMessage = null;
            StateHasChanged();
            return;
        }

        // Enter generates, Shift+Enter inserts newline
        if (args.Key == "Enter" && !args.ShiftKey && CanGenerate)
        {
            await GenerateAsync();
        }
    }


    // -- Collapsible keyboard handlers ------------------------------------

    private void HandleExplanationKeyDown(KeyboardEventArgs args)
    {
        if (args.Key is "Enter" or " ")
        {
            _showExplanation = !_showExplanation;
            StateHasChanged();
        }
    }

    private void HandleOptimizationKeyDown(KeyboardEventArgs args)
    {
        if (args.Key is "Enter" or " ")
        {
            _showOptimization = !_showOptimization;
            StateHasChanged();
        }
    }

    // -- Generation ------------------------------------------------------

    /// <summary>
    /// Generates SQL from the current natural language query.
    /// </summary>
    public async Task GenerateAsync()
    {
        if (!CanGenerate || Provider is null)
            return;

        _errorMessage = null;
        _currentQuery = null;
        _showExplanation = false;
        _showOptimization = false;
        _isLoading = true;
        StateHasChanged();

        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        try
        {
            var request = BuildAiRequest();
            var response = await Provider.CompleteAsync(request, ct);

            var sqlQuery = ParseResponse(response);
            var validation = ValidateSql(sqlQuery.GeneratedSql);

            sqlQuery.ValidationResult = validation;
            sqlQuery.IsReadOnly = validation.IsReadOnlyQuery;
            sqlQuery.RequiresApproval = RequireApproval && !validation.IsReadOnlyQuery;

            _currentQuery = sqlQuery;
            _isLoading = false;
            StateHasChanged();

            // Routing: approval vs. direct callback
            if (validation.IsValid && sqlQuery.RequiresApproval && OnApprovalRequired.HasDelegate)
            {
                await OnApprovalRequired.InvokeAsync(sqlQuery);
            }
            else if (validation.IsValid && OnSqlGenerated.HasDelegate)
            {
                await OnSqlGenerated.InvokeAsync(sqlQuery);
            }
            else if (!validation.IsValid)
            {
                _errorMessage = validation.ErrorMessage;
                if (OnError.HasDelegate)
                {
                    await OnError.InvokeAsync(validation.ErrorMessage ?? "Validacao falhou.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _isLoading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _isLoading = false;
            _errorMessage = ex.Message;
            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(ex.Message);
            }
            StateHasChanged();
        }
    }

    /// <summary>
    /// Clears the current query and input.
    /// </summary>
    public void Clear()
    {
        _nlQuery = string.Empty;
        _currentQuery = null;
        _errorMessage = null;
        _showExplanation = false;
        _showOptimization = false;
        StateHasChanged();
    }

    // -- AI Request Building ---------------------------------------------

    private AiChatRequest BuildAiRequest()
    {
        var systemPrompt = SystemPrompt ?? BuildDefaultSystemPrompt();

        var messages = new List<AiMessage>
        {
            new()
            {
                Role = AiRole.User,
                Content = _nlQuery!.Trim(),
                Timestamp = DateTimeOffset.UtcNow
            }
        };

        return new AiChatRequest
        {
            Messages = messages,
            SystemPrompt = systemPrompt,
            Temperature = 0.1,
            MaxTokens = 2048
        };
    }

    private string BuildDefaultSystemPrompt()
    {
        var prompt = """
            Voce e um especialista em SQL. Gere consultas SQL seguras e otimizadas.
            Responda SOMENTE com JSON no formato:
            {
              "sql": "A consulta SQL gerada",
              "explicacao": "Explicacao do que a consulta faz",
              "otimizacao": "Sugestoes de otimizacao (opcional)",
              "tabelas": ["tabela1", "tabela2"],
              "confianca": 0.95
            }
            """;

        if (!string.IsNullOrWhiteSpace(Schema))
        {
            prompt += $"\n\nSchema do banco de dados:\n{Schema}";
        }

        if (ReadOnly)
        {
            prompt += "\n\nIMPORTANTE: Gere SOMENTE consultas SELECT (somente leitura).";
        }

        if (MaxRows.HasValue)
        {
            prompt += $"\n\nLimite o resultado a {MaxRows.Value} linhas.";
        }

        var blocked = EffectiveBlockedOps;
        if (blocked.Count > 0)
        {
            prompt += $"\n\nNao use estas operacoes: {string.Join(", ", blocked)}.";
        }

        return prompt;
    }

    // -- Response Parsing ------------------------------------------------

    private XFAiSqlQuery ParseResponse(AiChatResponse response)
    {
        var content = response.Message.Content.Trim();
        var query = new XFAiSqlQuery
        {
            NaturalLanguageQuery = _nlQuery!.Trim()
        };

        // Try JSON parse
        try
        {
            // Extract JSON from possible markdown code blocks
            var jsonContent = ExtractJson(content);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("sql", out var sqlProp))
                query.GeneratedSql = sqlProp.GetString() ?? string.Empty;

            if (root.TryGetProperty("explicacao", out var expProp))
                query.Explanation = expProp.GetString();

            if (root.TryGetProperty("otimizacao", out var optProp))
                query.Optimization = optProp.GetString();

            if (root.TryGetProperty("tabelas", out var tablesProp) && tablesProp.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var tables = new List<string>();
                foreach (var t in tablesProp.EnumerateArray())
                {
                    var val = t.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                        tables.Add(val);
                }
                query.TouchedTables = tables;
            }

            if (root.TryGetProperty("confianca", out var confProp) && confProp.ValueKind == System.Text.Json.JsonValueKind.Number)
                query.Confidence = confProp.GetDouble();
        }
        catch
        {
            // Fallback: treat entire content as raw SQL
            query.GeneratedSql = content;
        }

        // Append MaxRows LIMIT if needed
        if (MaxRows.HasValue && query.GeneratedSql.Length > 0
            && !query.GeneratedSql.Contains("LIMIT", StringComparison.OrdinalIgnoreCase))
        {
            query.GeneratedSql = query.GeneratedSql.TrimEnd(';')
                + $" LIMIT {MaxRows.Value}";
        }

        return query;
    }

    private static string ExtractJson(string content)
    {
        // Try to extract from ```json ... ``` blocks
        var match = Regex.Match(content, @"```json\s*(.*?)\s*```", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
            return match.Groups[1].Value.Trim();

        // Try ``` ... ``` blocks
        match = Regex.Match(content, @"```\s*(.*?)\s*```", RegexOptions.Singleline);
        if (match.Success)
            return match.Groups[1].Value.Trim();

        // Try to find JSON object directly
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');
        if (start >= 0 && end > start)
            return content[start..(end + 1)];

        return content;
    }

    // -- SQL Validation --------------------------------------------------

    /// <summary>
    /// Validates generated SQL for security, policy, and injection risks.
    /// </summary>
    private XFAiSqlValidationResult ValidateSql(string sql)
    {
        var result = new XFAiSqlValidationResult();

        if (string.IsNullOrWhiteSpace(sql))
        {
            result.IsValid = false;
            result.ErrorMessage = "SQL gerado esta vazio.";
            return result;
        }

        var trimmedSql = sql.Trim();
        var upperSql = trimmedSql.ToUpperInvariant();

        // 1. Read-only check
        var firstWord = upperSql.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
        result.IsReadOnlyQuery = ReadOnlyPrefixes.Contains(firstWord);

        if (ReadOnly && !result.IsReadOnlyQuery)
        {
            result.IsValid = false;
            result.HasBlockedOperation = true;
            result.ErrorMessage = $"Operacao bloqueada: modo somente leitura ativo. SQL comeca com '{firstWord}'.";
            return result;
        }

        // 2. Blocked operations check
        var blocked = EffectiveBlockedOps;
        foreach (var op in blocked)
        {
            // Check if the operation keyword appears as a statement start or after semicolon
            if (StartsWithOperation(upperSql, op))
            {
                result.IsValid = false;
                result.HasBlockedOperation = true;
                result.ErrorMessage = $"Operacao bloqueada: {op} nao e permitido.";
                return result;
            }
        }

        // 3. Injection detection
        foreach (var pattern in InjectionPatterns)
        {
            if (pattern.IsMatch(trimmedSql))
            {
                result.HasInjectionRisk = true;
                var warnings = new List<string>(result.Warnings)
                {
                    $"Padrao de injecao detectado: {pattern.ToString()}"
                };
                result.Warnings = warnings;

                // Injection is a hard block
                result.IsValid = false;
                result.ErrorMessage = "Risco de injecao SQL detectado. Consulta rejeitada.";
                return result;
            }
        }

        return result;
    }

    private static bool StartsWithOperation(string upperSql, string operation)
    {
        // Check if SQL starts with the operation (possibly after WITH/CTE)
        var tokens = upperSql.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0) return false;

        // Direct start
        if (string.Equals(tokens[0], operation, StringComparison.OrdinalIgnoreCase))
            return true;

        // After semicolons (stacked queries)
        var parts = upperSql.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var partTokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partTokens.Length > 0 && string.Equals(partTokens[0], operation, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    // -- Copy Support ----------------------------------------------------

    private string? _copiedQueryId;

    private async Task CopySqlAsync(XFAiSqlQuery query)
    {
        _copiedQueryId = query.Id;
        StateHasChanged();

        // Reset after 2 seconds
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            _copiedQueryId = null;
            await InvokeAsync(StateHasChanged);
        });
    }

    private bool IsCopied(XFAiSqlQuery query) => _copiedQueryId == query.Id;

    // -- Approval --------------------------------------------------------

    private async Task ApproveAsync(XFAiSqlQuery query)
    {
        if (OnSqlGenerated.HasDelegate)
        {
            await OnSqlGenerated.InvokeAsync(query);
        }
    }

    // -- Dispose ---------------------------------------------------------

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    // -- Helpers ---------------------------------------------------------

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    private string ConfidencePercent(double confidence)
    {
        var clamped = Math.Clamp(confidence, 0, 1);
        return $"{clamped:P0}";
    }

    private double ConfidenceWidth(double confidence)
    {
        return Math.Clamp(confidence, 0, 1) * 100;
    }
}


