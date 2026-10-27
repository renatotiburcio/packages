# CHANGELOG — XForge Enterprise Development OS

## v54.0 (2026-06-11) — Golden Key Release

### 🚀 Novos Sistemas (21+)

#### Performance
- **Split Architecture**: Router (gemma4-26b) + Worker (qwen3-coder-30b) + Fallback (qwen3.6-27b)
- **INDEX.json**: 172 knowledge files indexados com keywords e summaries
- **Context Budget**: 32k tokens com eviction policy (4 níveis)
- **Resumo Executivo**: 172/172 knowledge files com resumo + quick reference
- **Ollama Config**: 5 modelos, GPU offload 35%, mlock, AVX2/FMA3

#### Inteligência
- **Error Pattern Learning**: 20 erros catalogados, 20 regras de prevenção, 11 auto-fix
- **Proactive Quality Gates**: 4 estágios (pré-commit/build/push/merge)
- **Proactive Intelligence**: 5 tipos de análise (context-aware, error prevention, impact, pattern, gaps)
- **Self-Healing Rules**: 12 regras de correção automática (SH-001 a SH-012)
- **Dependency Intelligence**: CVE detection, updates, breaking changes

#### Interação
- **Clarification Protocol**: Pergunta antes de executar, max 5, recomenda opção
- **Decision Support**: Tabelas comparativas com tradeoffs
- **Iterative Refinement**: Confirma resultado + sugere próximos passos
- **Context Gathering**: Lê antes de escrever, mapeia dependências
- **Suggestion Engine**: Max 3 sugestões, respeita "não"

#### Validação
- **Session Memory**: Preferências e decisões entre sessões
- **Knowledge Versioning**: 8 itens rastreados (INSS, IRRF, eSocial, etc.)
- **Multi-Agent Orchestration**: 6 agentes especializados
- **Live Dashboard**: Status em tempo real do sistema
- **Knowledge Validation**: 6 áreas críticas validadas

#### AutoResearch
- **Autonomous Experiment Loop**: Loop eterno de experimentos
- **Immutable Scoring**: xfs com 5 dimensões (imutável)
- **Simplicity Criterion**: Matriz ganho vs complexidade
- **Experiment Logger**: results.tsv append-only
- **Checkpoint & Resume**: Tarefas ilimitadas com 10 retomadas

### 📊 Números Finais

| Métrica | Valor |
|---------|-------|
| Rules | 30 |
| Scripts | 29 |
| Knowledge | 173 |
| Agents | 43 |
| Skills | 148 |
| Commands | 132 |
| Manual | v54.0, 1.680 linhas, 52 seções |
| Concorrentes | 15 apps analisados |

### 🔧 Scripts Novos

- `xforge-init.ps1` — Golden command: ativa e valida todo o sistema
- `score.ps1` — Scoring imutável (xfs)
- `pre-commit.ps1` — Quality gate pré-commit
- `dependency-check.ps1` — Scanner de dependências
- `proactive-intelligence.ps1` — Análise proativa
- `live-dashboard.ps1` — Dashboard em tempo real
- `knowledge-validation.ps1` — Validação de tabelas
- `ollama-setup.ps1` — Setup dos modelos Ollama

### 📚 Knowledge Base

- 173 knowledge files com Resumo Executivo
- 172 files indexados no INDEX.json
- 20 error patterns com auto-fix
- 8 itens de versioning (tabelas fiscais)
- 15 concorrentes analisados (5 contábeis + 5 empresariais + 5 ERPs)
- 3 playbooks de produção (deploy, code review, debug fiscal)

### 📖 Manual

- 52 seções com navegação completa
- 50+ prompts prontos
- Exemplos reais de diálogo
- Exemplo de AutoResearch (5 experimentos)
- Quick Reference com 50+ comandos
- FAQ com 8 perguntas frequentes
