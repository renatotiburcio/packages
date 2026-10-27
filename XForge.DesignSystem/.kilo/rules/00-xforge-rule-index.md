# Rules Index — XForge Engineer

Guia rápido para encontrar a rule correta. O agente deve ler este arquivo PRIMEIRO antes de carregar qualquer rule.

## Como Usar

1. Identificar a situação
2. Encontrar a rule correspondente nesta tabela
3. Ler APENAS a rule necessária (não todas)

## Rules por Categoria

### Core
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Golden Rules | `01-xforge-golden-rules.md` | Sempre — regras fundamentais |
| Active Project | `active-project-root-rules.md` | Ao iniciar sessão |

### Memória & Aprendizado
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Session Memory | `session-memory.md` | Preferências, decisões, histórico do usuário |
| Memory Rules | `memory-rules.md` | Regras básicas de memória |

### Inteligência & Qualidade
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Self-Healing | `self-healing-rules.md` | Correção automática de erros |
| Quality Gates | `proactive-quality-gates.md` | Checks automáticos pré-commit/push |
| Dependency Intel | `dependency-intelligence.md` | CVEs, updates, breaking changes |
| Proactive Intel | `proactive-intelligence.md` | Análise preventiva |


### CSS & Design System
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Tailwind First | 	ailwind-first-rule.md | Criar/modificar componentes Blazor, garantir tailwind-first |

### Interação
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Interaction Intel | `interaction-intelligence.md` | Perguntar, sugerir, decidir, confirmar |

### AutoResearch
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Experiment Loop | `autonomous-experiment-loop.md` | Loop autônomo de experimentos |
| Immutable Scoring | `immutable-scoring.md` | Sistema de pontuação (imutável) |
| Simplicity | `simplicity-criterion.md` | Avaliar complexidade vs ganho |

### Orquestração
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Multi-Agent | `multi-agent-orchestration.md` | Coordenar múltiplos agentes |

### Segurança & Compliance
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Security | `security-rules.md` | Validação de segurança |
| RBAC | `rbac-rules.md` | Controle de acesso |
| Audit Trail | `audit-trail-rules.md` | Rastreamento de ações |
| Policy | `policy-rules.md` | Políticas de governança |

### Operações
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Cost | `cost-rules.md` | Controle de custos de IA |
| Provider Routing | `provider-routing-rules.md` | Roteamento entre providers |
| TTL | `ttl-rules.md` | Time-to-live de caches |
| State Machine | `state-machine-rules.md` | Máquina de estados |
| Trust Score | `trust-score-rules.md` | Pontuação de confiança |
| Event Bus | `event-bus-rules.md` | Sistema de eventos |

### Documentação & Manutenção
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Manual Sync | `manual-sync-rules.md` | Sincronizar manual com código |
| Knowledge | `knowledge-rules.md` | Gerenciar knowledge base |
| Curation | `curation-rules.md` | Curadoria de conteúdo |

### Outros
| Rule | Arquivo | Quando usar |
|------|---------|-------------|
| Enterprise Arch | `enterprise-architecture-rules.md` | Arquitetura empresarial |
| Human Review | `human-review-rules.md` | Revisão humana obrigatória |
| Legacy Import | `legacy-import-rules.md` | Importação de sistemas legados |
| Promotion | `promotion-rules.md` | Promoção de código |

## Resumo

- **Total**: 30 rules
- **Leitura mínima**: 00-xforge-rule-index.md + 01-xforge-golden-rules.md
- **Leitura por situação**: Consultar tabela acima
- **NUNCA**: Ler todas as 30 rules de uma vez
