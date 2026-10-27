# XForge Design System - E2E Tests

## Configuração

### Pré-requisitos
- .NET 10.0 SDK
- Node.js (para Playwright)

### Instalação

```bash
# Restaurar pacotes
dotnet restore

# Build do projeto
dotnet build

# Instalar navegadores Playwright
pwsh bin/Debug/net10.0/playwright.ps1 install --with-deps
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test --filter "BasicE2ETests"
dotnet test --filter "XFSchedulerE2ETests"
dotnet test --filter "XFGanttE2ETests"

# Com trace (para debug)
dotnet test --filter "BasicE2ETests" --results TestResults --diag TestResults/log.txt
```

### Estrutura

```
tests/XForge.DesignSystem.E2E/
├── Components/
│   ├── ComponentAccessibilityTests.cs
│   ├── XFButtonE2ETests.cs
│   ├── XFSchedulerE2ETests.cs
│   ├── XFGanttE2ETests.cs
│   ├── XFBIDashboardE2ETests.cs
│   ├── XFTileLayoutE2ETests.cs
│   ├── XFProfileMenuE2ETests.cs
│   ├── XFRepeaterE2ETests.cs
│   └── XFOnScreenKeyboardE2ETests.cs
├── VisualRegression/
│   └── VisualRegressionTests.cs
├── BasicE2ETests.cs
├── XForge.DesignSystem.E2E.csproj
├── playwright.config.json
└── setup-playwright.ps1
```

### Tipos de Testes

1. **Testes de Acessibilidade** - Verificam roles, labels e atributos ARIA
2. **Testes de Componentes** - Testam renderização e interação básica
3. **Testes Visuais** - Screenshot comparison para regressão visual

### CI/CD

Os testes E2E são executados automaticamente via GitHub Actions:
- Push para main/develop
- Pull requests
- Agendamento semanal (segunda 6h)
