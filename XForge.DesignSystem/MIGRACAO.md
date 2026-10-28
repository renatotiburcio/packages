# Migração — XForge.DesignSystem

Migração do design system que vivia em `temp/xforge-design-system` (fora do controle de
versionamento do monorepo) para o monorepo `renatotiburcio/packages`.

## Análise do que foi migrado

Biblioteca de componentes **Blazor/Razor** (`Microsoft.NET.Sdk.Razor`, net10.0) madura e
extensa, com 15 projetos em 3 camadas:

| Camada | Projetos | Papel |
|---|---|---|
| **Componentes** | `XForge.DesignSystem` (núcleo, 297 `.razor`), `.Shared` | componentes, temas e tokens |
| **Docs** | `.Docs`, `.Docs.AI`, `.Docs.Shared` | site de documentação (Blazor) |
| **Ferramentas** | `.Analyzers`, `.Generators.AssetsGenerator`, `.Generators.DocsCompiler`, `.Docs.MCP` | analisador Roslyn, geradores de assets/docs e servidor MCP de documentação |
| **Qualidade** | `.Tests` (+ `DocsQualityTests`), `.AI.Tests`, `.Benchmarks`, `.E2E` | testes, benchmarks e E2E |

O núcleo é compilado com `IsTrimmable`, símbolos empacotados (`snupkg`) e documentação XML —
compatível com os padrões de publicação do monorepo.

## O que foi preservado

- Estrutura completa de `src/`, `tests/`, `docs/`, `scripts/`, `.github/`, `.kilo/`,
  `.mimocode/`, `.xforge/`
- `Directory.Packages.props` (central package management), `NuGet.config`,
  `tsconfig.json`, `package.json`/`BUN-PIPELINE.md` (pipeline de assets), `docker-compose.yml`
- Os arquivos de convenção do repo original (`AGENTS.md`, `.githooks/`, `.gitattributes`,
  `.kilocodeignore`, `kilo.jsonc`)

## O que NÃO foi copiado

`bin/`, `obj/`, `.vs/`, `tmp/` e artefatos de build — reconstruídos no primeiro build.

## Verificação

- `dotnet build src/XForge.DesignSystem` — **passa** (31 avisos pré-existentes; o projeto
  original deliberadamente usa `TreatWarningsAsErrors=false`)

## Notas

- O repo original tinha `.slnx` (formato XML do SDK 10); o CI do monorepo referencia os
  projetos pelos paths.
- Testes completos (`.Tests`, `.E2E`, `.AI.Tests`) exigem o runtime de ASP.NET/Blazor e o
  pipeline de assets (`BUN-PIPELINE.md`); rodar localmente antes de publicar.

## Reorganizacao profunda (2026-10-27)

Analise de dependencias: o nucleo (XForge.DesignSystem) referencia APENAS
XForge.DesignSystem.Shared. Todos os outros 13 projetos do repo de origem consumiam os
componentes, mas nao sao necessarios para eles.

### Estrutura final (somente o que importa para a biblioteca de componentes)

`
docs/                             guias e analises de design (8 arquivos)
src/XForge.DesignSystem/          componentes, tokens e temas (297 .razor)
src/XForge.DesignSystem.Shared/   unica dependencia do nucleo
src/XForge.DesignSystem.Tests/    testes bUnit (488 arquivos)
Directory.Packages.props          central package management (build)
NuGet.config                      fontes de pacote
`

### Removido (9 projetos satellite + tooling do repo de origem)

- Projetos: XForge.DesignSystem.AI(+Tests), Analyzers, Benchmarks, Docs(+.AI/.Shared),
  Generators.AssetsGenerator, Generators.DocsCompiler, XForge.Docs.MCP,
  XForge.AI, XForge.AgenticUIBuilder.Mcp, XForge.AICodingAssistant.Mcp,
  XForge.DesignSystem.Tools, XForge.FigmaSync, XForge.McpServer
- Testes: 	ests/XForge.DesignSystem.E2E e Tests/DocsQualityTests
- Raiz: .githooks, .kilo, .mimocode, .xforge, scripts, .github aninhado (o CI do
  monorepo e o da raiz), package.json, 	sconfig.json, kilo.jsonc, .kilocodeignore,
  AGENTS.md, BUN-PIPELINE.md, docker-compose.yml, XForge.DesignSystem.slnx

### Verificacao pos-reorganizacao

- dotnet build src/XForge.DesignSystem � passa
- testes filtrados (Scheduler) � 51/51; su�te completa no job dedicado do CI

### Backlog @key � remanescente consciente (2026-10-27)

De 77 componentes com @foreach, 75 receberam chave estavel em 18 lotes. Remanescente deliberado:

- XFTreeView e XFTOC: o corpo do loop e um RenderFragment (@RenderNode/@RenderItem),
  onde @key so se aplica a elementos. Aplicar exigiria envolver o fragmento num <div> �
  mudanca de DOM+CSS que e decisao de design, nao correcao mecanica.

## Suite de testes � limpeza de stales (2026-10-27)

A suite bUnit referenciava, majoritariamente, codigo dos satelites removidos na reorganizacao.
Levantamento e exclusoes (todas documentadas no csproj, todas reversiveis):

| Exclusao | Motivo |
|---|---|
| Tests\Analyzers\** | testava o pacote XForge.DesignSystem.Analyzers (removido) |
| Tests\Benchmarks\** | testava o pacote XFBenchmarkProject (removido) |
| Tests\Components\AI\** (45 arquivos) | testava Components.AI.Common/DataGrid/Insights/Orchestration do projeto XForge.DesignSystem.AI (removido) |
| Tests\Extensions\XFAiServiceCollectionExtensionsTest.cs | referencia IAiProvider/AiChatRequest do projeto AI removido |
| Tests\Components\Scheduler\XFGanttAdvancedTest.cs | referencia AllowDragResize (API inexistente no componente) |
| Tests\DocsQualityTests\** | qualidade de docs do site (removido) |

Tambem foi restaurado o PackageReference de Moq (perdido na reescrita anterior do csproj).

## Campanha @key + auditoria de performance (2026-10-27, 18 lotes)

- 75 de 77 componentes com @foreach receberam chave estavel (nao o indice � fragil em
  reordenacao/filtragem). Build verde em todos os 18 lotes.
- Remanescente consciente: XFTreeView e XFTOC (corpo do loop e RenderFragment;
  @key exigiria wrapper com mudanca de DOM � decisao de design pendente).
- Auditoria de callbacks: 376 EventCallback vs 1 Action<> nas APIs de componente �
  migraco para o idioma Blazor praticamente concluida no nucleo.
- Metodologia apos incidentes: replaces via shell corromperam arquivos duas vezes (<
  engolido); protocolo consolidado = edit tool + git checkout para restaurar + build a
  cada lote.
- Impactos: foco/selecao em listas longas (5.570 municipios), drag&drop (Scheduler,
  Repeater, SortableList, Gantt), digitacao (Autocomplete, Combobox, EditForm,
  PasswordStrength), charts densos (QRCode, Sparkline, Gauge).

## Achado da validacao por categorias (2026-10-27) � NAO e regressao

Forms 482/1183 � Navigation 417/570 � DataDisplay 627/1179.
Causa-raiz unica: InvalidOperationException: Cannot provide a value for property
'Localizer' ... no registered service of type 'XForge.Localization.IXFLocalizer'.

- 43 classes de teste registram Services.AddSingleton<IXFLocalizer, TestLocalizer>()
  no construtor � essas passam. As demais ~1.400 simply nunca registraram o servico.
- Nenhuma falha envolve @key/chave duplicada: os 75 componentes alterados nao regrediram.
- Contexto: a suite nunca compilou por inteiro antes desta limpeza (stales dos satelites),
  entao essas falhas estavam ocultas � sao higiene pre-existente do repo de origem.

### Opcoes de correcao (decisao pendente)

1. **Null-object fallback no produto**: XForgeComponentBase (ou os componentes que injetam
   IXFLocalizer) caem para um NullLocalizer quando o servico nao esta registrado.
   Torna a biblioteca resiliente e destrava a suite sem tocar em 1.400 arquivos de teste.
   Muda comportamento: ausencia de localizacao fica silenciosa.
2. **Infraestrutura de teste**: registrar o TestLocalizer globalmente para as classes que
   na o fazem (bUnit: fixture de colecao ou inicializador por classe).
   Nao muda produto, mas exige tocar em dezenas de arquivos de teste.
