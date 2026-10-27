---
description: Verificar atualizações online nas fontes oficiais (Receita Federal, eSocial, .NET, OWASP, etc).
agent: code
---

# /verificar-atualizacoes

## Categoria

manutenção

## Objetivo

Verificar se houve atualizações nas fontes oficiais que afetam o conhecimento do XForge.

## Fontes Monitoradas

### Fiscal (6 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| Receita Federal | gov.br/receitafederal | Mensal |
| eSocial | esocial.gov.br | Mensal |
| Portal SPED | sped.fazenda.gov.br | Trimestral |
| SEFAZ Estados | sefaz.rs/sp/mg | Mensal |
| ENCAT | encat.org.br | Trimestral |
| Comitê Simples | receita.fazenda.gov.br | Anual |

### Trabalhista (3 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| Min. Trabalho | gov.br/trabalho-e-emprego | Mensal |
| INSS | gov.br/inss | Mensal |
| CEF (FGTS) | caixa.gov.br | Mensal |

### Contabilidade (2 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| CFC | cfc.org.br | Trimestral |
| IFRS | ifrs.org | Trimestral |

### .NET/Dev (7 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| Microsoft Docs | docs.microsoft.com/dotnet | Mensal |
| GitHub .NET | github.com/dotnet | Semanal |
| NuGet | nuget.org | Semanal |
| Hangfire | github.com/HangfireIO | Mensal |
| MediatR | github.com/jbogard | Mensal |
| FluentValidation | github.com/FluentValidation | Mensal |
| AutoMapper | github.com/AutoMapper | Mensal |

### Segurança (3 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| OWASP | owasp.org | Trimestral |
| NIST NVD | nvd.nist.gov | Diário |
| CVE Details | cvedetails.com | Semanal |

### Banco de Dados (3 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| PostgreSQL | postgresql.org/docs | Mensal |
| MySQL | dev.mysql.com/doc | Mensal |
| Redis | redis.io/docs | Mensal |

### Azure (2 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| Azure Docs | docs.microsoft.com/azure | Mensal |
| Azure Updates | azure.microsoft.com/updates | Semanal |

### Legislação (2 fontes)
| Fonte | URL | Frequência |
|-------|-----|-----------|
| Planalto | planalto.gov.br | Mensal |
| Diário Oficial | in.gov.br | Semanal |

## Procedimento

1. Rodar script de verificação
2. Analisar relatório
3. Identificar atualizações relevantes
4. Atualizar knowledge files afetados
5. Atualizar manual se necessário
6. Registrar no audit trail

## Comando

```powershell
.\.kilo\automation\scripts\check-updates.ps1
```

## Saída

Relatório salvo em `.xforge/operations/update-check-report.json` com:
- Timestamp da verificação
- Fontes verificadas
- Atualizações encontradas
- Erros de conectividade
