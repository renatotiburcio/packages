# Dependency Intelligence — XForge Engineer

## Visão Geral

Monitoramento proativo de todas as dependências do projeto: CVEs, atualizações, breaking changes, compatibilidade. Roda automaticamente e alerta antes que vulnerabilidades ou incompatibilidades virarem bugs.

## Fontes de Dados

### 1. NuGet Packages
```powershell
# Verificar vulnerabilidades
dotnet list package --vulnerable

# Verificar atualizações
dotnet list package --outdated

# Verificar licenças
dotnet list package --include-transitive
```

### 2. NPM Packages (para front-end/Tauri)
```powershell
# Verificar vulnerabilidades
npm audit

# Verificar atualizações
npm outdated
```

### 3. Docker Images
```bash
# Verificar vulnerabilidades na imagem
docker scout cves <image>

# Verificar atualizações
docker image inspect <image> --format '{{.RepoDigests}}'
```

### 4. GitHub Advisories
API do GitHub para CVEs em dependências públicas.

## Monitoramento Automático

### Frequência de Verificação

| Tipo | Frequência | Método |
|------|-----------|--------|
| **CVEs críticos** | A cada build | `dotnet list package --vulnerable` |
| **CVEs altos** | Diariamente | Script agendado |
| **Atualizações** | Semanalmente | `dotnet list package --outdated` |
| **Breaking changes** | Antes de atualizar | Análise de release notes |
| **Licenças** | A cada novo pacote | `dotnet nuget locals all --list` |

### Pipeline de Verificação

```
Build → dotnet restore
         → dotnet list package --vulnerable
         → Se CVE crítico: BLOQUEAR build + criar issue
         → Se CVE alto: WARN + sugerir atualização
         → Se seguro: prosseguir
```

## Regras de Alerta

### CVE Detection

| Severidade | Ação | Auto-fix |
|-----------|------|----------|
| **Crítico** | Bloquear build + criar issue + notificar | Atualizar pacote |
| **Alto** | Warn + sugerir atualização | Sugerir atualização |
| **Médio** | Informar no relatório | N/A |
| **Baixo** | Log silencioso | N/A |

### Update Detection

| Tipo de Atualização | Ação | Risco |
|---------------------|------|-------|
| **Patch** (1.0.x → 1.0.y) | Sugerir atualização | Baixo |
| **Minor** (1.x → 1.y) | Sugerir com cautela | Médio |
| **Major** (x → y) | Alertar + analisar breaking changes | Alto |
| **Preview/RC** | NUNCA auto-atualizar | Crítico |

### Breaking Change Detection

Para cada atualização major, o sistema:
1. Lê release notes do pacote
2. Analisa mudanças de API
3. Roda `dotnet build` após atualização
4. Se build quebrar: reverte automaticamente
5. Cria issue com detalhes do breaking change

## Configuração

```json
{
  "dependencyIntelligence": {
    "enabled": true,
    "nuget": {
      "checkVulnerabilities": true,
      "checkUpdates": true,
      "autoFixPatch": true,
      "autoFixMinor": false,
      "autoFixMajor": false,
      "neverUpdate": ["preview", "rc", "alpha", "beta"]
    },
    "npm": {
      "checkAudit": true,
      "checkUpdates": true
    },
    "docker": {
      "checkCves": true,
      "baseImage": "mcr.microsoft.com/dotnet/aspnet:10.0"
    },
    "alerting": {
      "criticalCve": ["create-issue", "block-build", "notify"],
      "highCve": ["warn", "suggest-update"],
      "outdated": ["weekly-report"]
    }
  }
}
```

## Comandos

```
/dependencias verificar vulnerabilidades em todas as dependências
/dependencias mostrar atualizações disponíveis
/dependencias atualizar pacotes patch (seguro)
/dependencias analisar breaking changes em [pacote]
/dependencias gerar relatório de dependências
```

## Relatório Semanal

```
╔══════════════════════════════════════════════╗
║'        DEPENDENCY INTELLIGENCE REPORT        ║'
║'              Semana de 09-15 Jun 2026        ║'
╠══════════════════════════════════════════════╣
║'                                              ║'
║ 🔒 SEGURANÇA                                ║
║'   CVEs críticos: 0 ✅                       ║'
║   CVEs altos: 1 ⚠️                          ║
║   → System.Net.Http 8.0.0 → atualizar       ║
║'                                              ║'
║ 📦 ATUALIZAÇÕES                             ║
║'   Disponíveis: 12                           ║'
║   Patch (seguro): 5 → auto-atualizar        ║
║   Minor: 4 → revisar                        ║
║   Major: 3 → analisar breaking changes      ║
║'                                              ║'
║ ⚠️ BREAKING CHANGES                         ║
║   → EF Core 10.0.1 → 10.1.0 (API changes)  ║
║   → FluentValidation 11.x → 12.x (nova API)║
║'                                              ║'
║ 📊 RESUMO                                   ║
║'   Total pacotes: 47                         ║'
║'   Atualizados: 42 (89%)                     ║'
║'   Desatualizados: 5 (11%)                   ║'
║'   Vulneráveis: 1 (2%)                       ║'
║'                                              ║'
╚══════════════════════════════════════════════╝
```

## Integração com Error Pattern Learning

Quando uma atualização causa erro, o sistema:
1. Registra o erro no `errors-solutions-graph.json`
2. Conecta: `erro ← dependência ← atualização`
3. Na próxima vez que detectar a mesma atualização, alerta automaticamente
4. Cria regra de prevenção: "NUNCA atualizar [pacote] para [versão] sem testes"
