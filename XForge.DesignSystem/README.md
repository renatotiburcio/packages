# XForge Enterprise Development OS

<p align="center">
  <strong>Template operacional que transforma KiloCode em assistente enterprise completo.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/version-54.0-blue" alt="Version">
  <img src="https://img.shields.io/badge/skills-151-green" alt="Skills">
  <img src="https://img.shields.io/badge/agents-43-purple" alt="Agents">
  <img src="https://img.shields.io/badge/commands-129-orange" alt="Commands">
  <img src="https://img.shields.io/badge/rules-30-red" alt="Rules">
  <img src="https://img.shields.io/badge/knowledge-172-cyan" alt="Knowledge">
  <img src="https://img.shields.io/badge/manual-1741%20lines-yellow" alt="Manual">
</p>

---

## O que é?

O XForge é um **template operacional** para KiloCode que inclui tudo que você precisa para desenvolver software enterprise no Brasil:

- **151 skills** especializadas (fiscal, contabilidade, folha, .NET, Blazor, etc.)
- **43 agents** com roles definidos
- **129 slash commands** prontos para uso
- **30 rules** de governança e qualidade
- **172 knowledge files** com domínio brasileiro completo
- **Split Architecture** (Router + Worker) para IA local
- **AutoResearch Engine** com loop autônomo de experimentos
- **Manual completo** com 53 seções e exemplos reais

## Arquitetura

```
┌─────────────────────────────────────────────┐
│  AGENTS.md → Instruções gerais              │
│  kilo.jsonc → instructions, agent, skills   │
├─────────────────────────────────────────────┤
│  .kilo/ → Camada Operacional               │
│    agents/   → 43 especialistas             │
│    skills/   → 151 habilidades              │
│    commands/ → 129 comandos                 │
│    rules/    → 30 regras                    │
│    automation/ → 31 scripts                 │
├─────────────────────────────────────────────┤
│  .xforge/ → Camada Persistente             │
│    knowledge/ → 172 arquivos + INDEX.json   │
│    memory/    → Memória do projeto          │
│    config/    → Ollama, Router, Versioning  │
│    experiments/ → AutoResearch results      │
└─────────────────────────────────────────────┘
```

## Quick Start

### 1. Instalação

```powershell
# Copiar template para seu projeto
# Depois rodar:
.\.kilo\automation\scripts\onboarding.ps1
```

### 2. Primeiro Uso

No KiloCode, digite:
```
/xforge-init
```
Isso valida todo o sistema e mostra o status.

### 3. Começar a Usar

```
/xforge criar API REST com Clean Architecture, .NET 10, PostgreSQL
/xforge analisar projeto existente
/xforge calcular folha de 50 funcionários com INSS e IRRF
```

## Sistemas Implementados

### Split Architecture (IA Local)

Detecção automática de modelos — funciona com qualquer combinação Ollama:

| Capacidade | Modelos Exemplo | Uso |
|-----------|----------------|-----|
| **Pequeno** | &lt; 7B params | Perguntas rápidas, formatação |
| **Médio** | 7B-14B params | Features, refactor, docs |
| **Grande** | 14B-70B params | Código complexo, arquitetura |
| **Muito Grande** | 70B+ params | Tudo, tarefas críticas |

> **Não fixar modelos no config** — o usuário seleciona no prompt. O sistema detecta e adapta automaticamente.

### AutoResearch Engine

Loop autônomo de experimentos inspirado no [karpathy/autoresearch](https://github.com/karpathy/autoresearch):

- **Immutble Scoring (xfs)**: 5 dimensões (correctness 30%, performance 20%, quality 20%, simplicity 15%, prevention 15%)
- **Simplicity Criterion**: Matriz ganho vs complexidade
- **Experiment Logger**: results.tsv append-only
- **Checkpoint & Resume**: Tarefas ilimitadas com 10 retomadas

### Inteligência

- **Error Pattern Learning**: 20 erros catalogados com auto-fix
- **Proactive Quality Gates**: 4 estágios (pré-commit/build/push/merge)
- **Self-Healing Rules**: 12 regras de correção automática
- **Dependency Intelligence**: CVE detection, updates, breaking changes
- **Proactive Intelligence**: Análise preventiva antes de agir
- **Auto-Retry & Recovery**: Tenta fix antes de parar (máx 3 tentativas)

### Interação

- **Clarification Protocol**: Pergunta antes de executar
- **Decision Support**: Tabelas comparativas com tradeoffs
- **Iterative Refinement**: Confirma resultado + sugere próximos passos
- **Context Gathering**: Lê antes de escrever
- **Suggestion Engine**: Max 3 sugestões proativas

### Validação

- **Session Memory**: Preferências entre sessões
- **Knowledge Versioning**: 8 tabelas rastreadas (INSS, IRRF, eSocial, etc.)
- **Multi-Agent Orchestration**: 6 agentes trabalhando em paralelo
- **Live Dashboard**: Status em tempo real
- **Knowledge Validation**: 6 áreas críticas validadas
- **Checkpoint & Resume**: Tarefas ilimitadas com 10 retomadas
- **Adaptive Eviction**: Limiares baseados em % do contexto (adapta a 32k/64k/128k)

## Domínios Cobertos

| Domínio | Arquivos | Conteúdo |
|---------|----------|----------|
| **Folha de Pagamento** | 28 | INSS 2025, IRRF 2025, FGTS, férias, 13º, rescisão, eSocial |
| **Fiscal** | 19 | ICMS, PIS, COFINS, NF-e, SPED, Reforma Tributária |
| **Contabilidade** | 11 | Plano de contas, DRE, ECD, ECF, LCPR |
| **Financeiro** | 7 | CNAB, fluxo de caixa, conciliação |
| **ERP** | 14 | Estoque, vendas, compras, PDV, CRM, Curva ABC |
| **Arquitetura** | 18 | .NET 10, ASP.NET Core, Blazor, MAUI, EF Core |
| **Padrões** | 12 | Hangfire, MediatR, FluentValidation, AutoMapper |
| **Concorrentes** | 1 | 15 apps analisados (Conta Azul, TOTVS, SAP, etc.) |

## Ferramentas e Web Access

### playwright-mcp (Obrigatório para análise de páginas)

**Regra**: Ao analisar qualquer página web, SEMPRE usar `playwright-mcp` — independente do modelo de IA.

| Tarefa | Ferramenta | Por quê |
|--------|-----------|---------|
| Analisar design de site | playwright-mcp | Renderiza página completa |
| Screenshot de página | playwright-mcp | Captura estado visual |
| Verificar layout/UX | playwright-mcp | Vê output renderizado |
| Testar formulários | playwright-mcp | Pode interagir com elementos |
| Verificar responsivo | playwright-mcp | Pode redimensionar viewport |
| Ler documentação API | webfetch | Retorna markdown/texto |
| Buscar conteúdo raw | webfetch | Mais leve, mais rápido |

### webfetch (Só para API/texto)

Usar `webfetch` APENAS para:
- Respostas de API REST
- Arquivos raw (markdown, txt)
- Conteúdo sem renderização

## Comandos Disponíveis

### Setup
```powershell
.\.kilo\automation\scripts\xforge-init.ps1      # Golden command
.\.kilo\automation\scripts\doctor.ps1           # Validar sistema
.\.kilo\automation\scripts\ollama-setup.ps1     # Configurar Ollama
.\.kilo\automation\scripts\live-dashboard.ps1    # Dashboard
```

### Desenvolvimento
```
/xforge [tarefa]                          # Roteamento universal
/create-project API com Clean Architecture # Criar projeto
/develop feature [descrição]          # Criar feature
/develop bugfix [descrição]           # Corrigir bug
/develop refactor [descrição]         # Refatorar
```

### Qualidade
```
/quality                                # Quality gates
/security                                # Auditoria segurança
.\.kilo\automation\scripts\pre-commit.ps1 # Pré-commit
.\.kilo\automation\scripts\dependency-check.ps1 # Dependências
```

### Domínios Brasileiros
```
/xforge calcular folha de 50 funcionários  # Folha
/xforge gerar EFD ICMS/IPI do mês         # Escrituração
/xforge criar módulo de estoque            # ERP
/xforge verificar conformidade LGPD       # LGPD
```

## Hardware Recomendado

| Componente | Mínimo | Recomendado |
|-----------|--------|-------------|
| GPU | 8GB VRAM | 16GB VRAM |
| RAM | 16GB | 32GB DDR4 |
| CPU | 4 cores | 8 cores (Ryzen 7 5700G) |
| Storage | 50GB SSD | 100GB SSD |

## Estrutura de Arquivos

```
terminal/
├── AGENTS.md                    # Instruções do agente
├── kilo.jsonc                   # Config Kilo
├── CHANGELOG.md                 # Histórico de versões
├── docker-compose.yml           # PostgreSQL + Redis + RabbitMQ
├── docs/index.html              # Manual completo (v54.0)
├── .kilo/                       # Camada operacional
│   ├── agents/                  # 43 agents (EN kebab-case)
│   ├── skills/                  # 151 skills (SKILL.md)
│   ├── commands/                # 129 slash commands
│   ├── rules/                   # 30 regras
│   ├── automation/              # 31 scripts
│   └── mcp/                     # MCP servers
└── .xforge/                     # Camada persistente
    ├── knowledge/               # 172 arquivos + INDEX.json
    ├── config/                  # Ollama, Router, Versioning
    ├── checkpoints/             # AutoResearch checkpoints
    └── experiments/             # AutoResearch results
```

## Para Desenvolvedores

### Criar Novo Skill

```markdown
# .kilo/skills/minha-skill/SKILL.md
---
name: minha-skill
description: Quando usar e o que faz
---

# minha-skill
## Objetivo
[descrição]
## Procedimento
1. Passo 1
2. Passo 2
```

### Criar Novo Agent

```markdown
# .kilo/agents/meu-agent.md
---
name: meu-agent
description: O que faz e quando usar
mode: subagent
permission:
  read: allow
  edit: {"*.md": allow, "*": deny}
  bash: ask
---
```

### Validar Mudanças

```powershell
.\.kilo\automation\scripts\doctor.ps1           # Validação completa
.\.kilo\automation\scripts\validate-kilo-strict.ps1  # Strict validation
.\.kilo\automation\scripts\audit-naming.ps1     # Naming conventions
```

## Links Úteis

- **Manual completo**: `docs/index.html`
- **CHANGELOG**: `CHANGELOG.md`
- **Kilo Code docs**: https://kilo.ai/docs/customize
- **Ollama**: https://ollama.ai

## Licença

MIT

---

<p align="center">
  <strong>v54.0</strong> · Split Architecture · AutoResearch · IA Local · 100% Kilo Compatible
</p>
