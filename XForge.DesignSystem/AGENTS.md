# AGENTS.md - XForge Enterprise Development OS

## Mission

Transform a project into living, usable organizational knowledge.

## Core Rule

Recognize before creating.
Understand before changing.
Validate before finishing.

## CRITICAL: Tool Usage Rules

### ALWAYS Write Code to Files Directly

When the task requires creating or modifying code, you MUST use the edit or write tools.
Do NOT just describe what to do. Do NOT output code in chat that the user must copy.
Write the actual file, to the actual path, with the actual code.

**This is the #1 rule. Breaking it means the task is incomplete.**

Procedure for every code task:
1. Read existing file first (if it exists) to understand current state.
2. Use edit to modify specific sections, or write for new files.
3. Never skip writing the file.
4. Never say "here is the code" without actually writing it to disk.
5. If the file is large, write it in chunks (use write for first part, edit for additions).

### Complete Tasks Before Pausing

**NEVER STOP MID-TASK.** The agent must complete the entire task before pausing.

1. If context is running low: Save checkpoint, summarize progress, continue.
2. If an error occurs: Try fix (max 3 retries), then checkpoint and ask user.
3. If multiple steps needed: Execute ALL steps sequentially.
4. If user says "do X": Complete X entirely before asking "what's next?"
5. The agent should ONLY stop when:
   - Task is 100% complete
   - User explicitly asks to stop
   - Context is exhausted AND checkpoint is saved
   - An error cannot be recovered after 3 retries

### ALWAYS Fetch Web Pages When Needed

When the user asks to check a URL, read documentation online, or access any web resource,
you MUST use playwright-mcp tool. Do NOT use webfetch for page analysis — it only returns text, not visual/rendered content.

**This applies to ALL AI models (gemma, qwen, llama, claude, gpt, etc.) — no exceptions.**

Procedure for web access:
1. When user provides a URL or asks about something online, use playwright-mcp immediately.
2. playwright-mcp renders the page fully (HTML, CSS, JS) and captures visual state.
3. Use playwright-mcp for: screenshots, DOM analysis, visual inspection, form testing.
4. Use webfetch ONLY for: API responses, raw text, markdown files.
5. Never say "I cannot access the internet" — playwright-mcp is always available.

### When to use playwright-mcp vs webfetch

| Task | Use | Why |
|------|-----|-----|
| Analyze a website design | playwright-mcp | Renders full page with CSS/JS |
| Take screenshot of page | playwright-mcp | Captures visual state |
| Check page layout/UX | playwright-mcp | Sees rendered output |
| Read API documentation | webfetch | Returns markdown/text |
| Fetch raw file content | webfetch | Lighter, faster |
| Test form interactions | playwright-mcp | Can interact with elements |
| Check responsive design | playwright-mcp | Can resize viewport |

## Public Command Surface

Prefer these skills (use via skill tool):

- /xforge-init (validar sistema)
- /xforge (roteamento universal)
- /research (fase 1: analisar fontes → gerar documentação)
- /document-execute (fase 2: executar backlog → desenvolver)
- /analyze-project
- /create-project
- /develop
- /quality
- /security
- /knowledge
- /memory
- /document
- /release

## Operating Flow

1. Understand the user request.
2. **Clarify** if ambiguous — ask before executing (max 5 questions, recommend best option).
3. **Gather context** — read files, check dependencies, verify tests exist.
4. **Decide** if multiple approaches — present options with tradeoffs table, recommend one.
5. Recover memory only when needed.
6. Recognize project context.
7. Select one primary canonical agent.
8. Select the smallest useful set of skills.
9. Validate policies, security and quality gates.
10. Execute the task.
11. **Confirm** result — what changed, impact, next steps suggested.
12. **Suggest** improvements — max 3 relevant suggestions, explain impact.
13. Run focused validation.
14. Update .xforge memory, docs, backlog or roadmap only when useful.
15. Report what changed and what was validated.

### Interaction Rules (详见 .kilo/rules/interaction-intelligence.md)

- **Clarification Protocol**: Ask when ambiguous, before acting. Format: numbered options with letters.
- **Decision Support**: Present 2-4 options in table, always recommend one, explain why.
- **Iterative Refinement**: After each action, confirm result + suggest 2-3 next steps.
- **Context Gathering**: Read before writing, map dependencies, check tests, plan steps.
- **Suggestion Engine**: Max 3 suggestions per interaction, respect "no", explain impact.

### Completion Rules (CRITICAL)

**NEVER STOP MID-TASK.** The agent must complete the entire task before pausing.

1. **If context is running low**: Save checkpoint, summarize progress, continue in next turn.
2. **If an error occurs**: Try fix (max 3 retries), then checkpoint and ask user.
3. **If multiple steps needed**: Execute ALL steps sequentially, not one at a time.
4. **If user says "do X"**: Complete X entirely before asking "what's next?"
5. **Checkpoint frequency**: Save checkpoint every 5-10 significant actions.

**The agent should ONLY stop when:**
- Task is 100% complete
- User explicitly asks to stop
- Context is exhausted AND checkpoint is saved
- An error cannot be recovered after 3 retries

### AutoResearch Loop (详见 .kilo/rules/autonomous-experiment-loop.md)

When running autonomous experiments:
1. Try a change → commit → run `score.ps1` → measure xfs
2. If xfs improved: keep, advance, log to results.tsv
3. If xfs worsened: discard (git reset), analyze why, try different approach
4. If crash: attempt fix (max 3 tries), then discard and move on
5. NEVER STOP until human interrupts
6. NEVER modify the scoring system (immutable)

### Auto-Retry & Recovery (详见 .kilo/rules/auto-retry-recovery.md)

When encountering an error during execution:
1. **Always try at least 1 fix** before stopping
2. **Auto-fix patterns**: build error → add import, null ref → add null check, test failure → fix code
3. **Max 3 retries** per error type
4. **Alternative approach**: if fix fails, try different strategy
5. **Checkpoint on failure**: save progress before stopping
6. **NEVER stop without trying** — the agent must always attempt recovery

## Context Strategy

Default context window: 32k tokens (optimized for local AI on CPU — 3-4x faster than 128k).

### Model Detection (Runtime)

The agent should detect which model is being used at runtime and adapt behavior:
- **Small models** (< 7B): Simple tasks, quick answers, formatting
- **Medium models** (7B-14B): Features, refactoring, documentation
- **Large models** (14B-70B): Complex architecture, multi-file, security
- **Very large models** (70B+): Full capability, critical tasks

If a specific capability is needed (e.g., large context for big files), the agent should ask the user to select an appropriate model.

### Context Budget Rules

Every turn must respect the 32k budget:

1. **Lazy loading**: Read `.xforge/knowledge/INDEX.json` first. Select only 2-3 relevant files.
2. **Resumo first**: Load only the `summary` field from INDEX.json. If sufficient, skip full file load.
3. **Incremental edits**: Use edit, not write. Small changes, not large rewrites.
4. **One file at a time**: Do not write multiple files in one response.
5. **Eviction triggers**:
   - At 80% (25.6k): Summarize history older than 3 turns.
   - At 90% (28.8k): Evict knowledge files not referenced in last 2 turns.
   - At 95% (30.4k): Truncate tool outputs > 300 lines.
   - At 98% (31.4k): Force compact — summarize everything, keep only current task.
6. **Stop signal**: If context is full after eviction, save checkpoint and stop. Resume in next session.

### Checkpoint & Resume (详见 .kilo/rules/checkpoint-resume.md)

When context limit is reached even after compaction:
1. Save checkpoint to `.xforge/checkpoints/<task-id>.json`
2. Checkpoint contains: completed steps, current step, remaining steps, files created/modified
3. In next session, detect pending checkpoint and resume automatically
4. Maximum 10 resume attempts per task, then reevaluate approach

### Cloud Models (Fallback to OpenRouter/Anthropic)

When cloud models are available:
1. Can load full files when needed, but prefer indexes first.
2. Use grep/glob to find relevant code, then read matching sections.
3. Write incrementally. Make small edits, not large rewrites.
4. Summarize before acting. If context is getting large, summarize what you know and proceed.
5. Use indexes first. Check .xforge/knowledge/INDEX.json before reading deep files.
6. Prefer edit over write for existing files.
7. 2-3 files per response is safe.

## Structure Rule

```
.kilo   = replaceable operational layer
.xforge = persistent project layer
```

Never delete .xforge during updates.

## Context Rule

Do not load all .xforge content by default. Start with the semantic index:

- `.xforge/knowledge/INDEX.json` — **PRIMARY**: Search by keywords, load only matched files
- `.xforge/knowledge/_index.md` — Project overview (Nivel 1)
- `.xforge/project-dna/PROJECT-DNA.md`
- `.xforge/decisions/ADR-INDEX.md`

### Lazy Loading Protocol

1. Read `INDEX.json` and search by task keywords
2. Match files by keywords, domain, and category
3. Load only the `summary` field of matched files (from INDEX.json)
4. If summary is sufficient → proceed without full file load
5. If full content needed → `Read(filePath, limit=100)` first
6. Expand with offset only if more context is needed

## Validation

After structural changes, run:

`powershell
.\.kilo\automation\scripts\doctor.ps1
`

## Manual Sync Rule

After ANY significant change (new skill, agent, command, rule, config, script, provider, security feature, bug fix, migration), verify if `docs/index.html` needs updating. If yes, update the manual immediately. See `.kilo/rules/manual-sync-rules.md` for the full checklist.
