# Migra√ß√£o ‚Äî XForge.DesignSystem

Migra√ß√£o do design system que vivia em `temp/xforge-design-system` (fora do controle de
versionamento do monorepo) para o monorepo `renatotiburcio/packages`.

## An√°lise do que foi migrado

Biblioteca de componentes **Blazor/Razor** (`Microsoft.NET.Sdk.Razor`, net10.0) madura e
extensa, com 15 projetos em 3 camadas:

| Camada | Projetos | Papel |
|---|---|---|
| **Componentes** | `XForge.DesignSystem` (n√∫cleo, 297 `.razor`), `.Shared` | componentes, temas e tokens |
| **Docs** | `.Docs`, `.Docs.AI`, `.Docs.Shared` | site de documenta√ß√£o (Blazor) |
| **Ferramentas** | `.Analyzers`, `.Generators.AssetsGenerator`, `.Generators.DocsCompiler`, `.Docs.MCP` | analisador Roslyn, geradores de assets/docs e servidor MCP de documenta√ß√£o |
| **Qualidade** | `.Tests` (+ `DocsQualityTests`), `.AI.Tests`, `.Benchmarks`, `.E2E` | testes, benchmarks e E2E |

O n√∫cleo √© compilado com `IsTrimmable`, s√≠mbolos empacotados (`snupkg`) e documenta√ß√£o XML ‚Äî
compat√≠vel com os padr√µes de publica√ß√£o do monorepo.

## O que foi preservado

- Estrutura completa de `src/`, `tests/`, `docs/`, `scripts/`, `.github/`, `.kilo/`,
  `.mimocode/`, `.xforge/`
- `Directory.Packages.props` (central package management), `NuGet.config`,
  `tsconfig.json`, `package.json`/`BUN-PIPELINE.md` (pipeline de assets), `docker-compose.yml`
- Os arquivos de conven√ß√£o do repo original (`AGENTS.md`, `.githooks/`, `.gitattributes`,
  `.kilocodeignore`, `kilo.jsonc`)

## O que N√ÉO foi copiado

`bin/`, `obj/`, `.vs/`, `tmp/` e artefatos de build ‚Äî reconstru√≠dos no primeiro build.

## Verifica√ß√£o

- `dotnet build src/XForge.DesignSystem` ‚Äî **passa** (31 avisos pr√©-existentes; o projeto
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

- dotnet build src/XForge.DesignSystem ó passa
- testes filtrados (Scheduler) ó 51/51; suÌte completa no job dedicado do CI

### Backlog @key ó remanescente consciente (2026-10-27)

De 77 componentes com @foreach, 75 receberam chave estavel em 18 lotes. Remanescente deliberado:

- XFTreeView e XFTOC: o corpo do loop e um RenderFragment (@RenderNode/@RenderItem),
  onde @key so se aplica a elementos. Aplicar exigiria envolver o fragmento num <div> ó
  mudanca de DOM+CSS que e decisao de design, nao correcao mecanica.

## Suite de testes ó limpeza de stales (2026-10-27)

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

- 75 de 77 componentes com @foreach receberam chave estavel (nao o indice ó fragil em
  reordenacao/filtragem). Build verde em todos os 18 lotes.
- Remanescente consciente: XFTreeView e XFTOC (corpo do loop e RenderFragment;
  @key exigiria wrapper com mudanca de DOM ó decisao de design pendente).
- Auditoria de callbacks: 376 EventCallback vs 1 Action<> nas APIs de componente ó
  migraco para o idioma Blazor praticamente concluida no nucleo.
- Metodologia apos incidentes: replaces via shell corromperam arquivos duas vezes (<
  engolido); protocolo consolidado = edit tool + git checkout para restaurar + build a
  cada lote.
- Impactos: foco/selecao em listas longas (5.570 municipios), drag&drop (Scheduler,
  Repeater, SortableList, Gantt), digitacao (Autocomplete, Combobox, EditForm,
  PasswordStrength), charts densos (QRCode, Sparkline, Gauge).

## Achado da validacao por categorias (2026-10-27) ó NAO e regressao

Forms 482/1183 ∑ Navigation 417/570 ∑ DataDisplay 627/1179.
Causa-raiz unica: InvalidOperationException: Cannot provide a value for property
'Localizer' ... no registered service of type 'XForge.Localization.IXFLocalizer'.

- 43 classes de teste registram Services.AddSingleton<IXFLocalizer, TestLocalizer>()
  no construtor ó essas passam. As demais ~1.400 simply nunca registraram o servico.
- Nenhuma falha envolve @key/chave duplicada: os 75 componentes alterados nao regrediram.
- Contexto: a suite nunca compilou por inteiro antes desta limpeza (stales dos satelites),
  entao essas falhas estavam ocultas ó sao higiene pre-existente do repo de origem.

### Opcoes de correcao (decisao pendente)

1. **Null-object fallback no produto**: XForgeComponentBase (ou os componentes que injetam
   IXFLocalizer) caem para um NullLocalizer quando o servico nao esta registrado.
   Torna a biblioteca resiliente e destrava a suite sem tocar em 1.400 arquivos de teste.
   Muda comportamento: ausencia de localizacao fica silenciosa.
2. **Infraestrutura de teste**: registrar o TestLocalizer globalmente para as classes que
   na o fazem (bUnit: fixture de colecao ou inicializador por classe).
   Nao muda produto, mas exige tocar em dezenas de arquivos de teste.

### Refinamento da opcao 1 (2026-10-28)

A opcao 1 tem blast radius maior que o estimado: **56 componentes** declaram
[Inject] private IXFLocalizer? Localizer { get; set; }. A injeÁ„o por atributo do
Blazor LAN«A quando o serviÁo n„o est· registrado (n„o deixa a propriedade null) ó
ent„o um null-object exigiria trocar os 56 [Inject] por resoluÁ„o lazy
(service-locator com fallback), o que È mudanÁa arquitetural ampla no produto.

Alternativa de menor risco que destrava a suite **sem tocar no produto**: registrar o
TestLocalizer no **projeto de teste** de forma global ó bUnit expıe
BunitContext.TestServiceProvider/Services, e um fixture de coleÁ„o xUnit cobre
todas as classes que herdam BunitContext. Fica para decis„o.

## Triagem das falhas restantes (2026-10-28)

Apos o localizer pt-BR real, ~60 falhas permanecem ó todas do tipo
Assert.Contains(): Sub-string not found, nenhuma de @key/chave duplicada.
Amostras: Render_DefaultNavigationAriaLabel_IsPortuguese,
Extended_CustomAriaLabel_Renders, Render_WithCustomEmptyText,
ButtonClass_Dismissible_RendersDismissLabel.

Leitura: testes esperando strings pt-BR que nao aparecem ó ou a chave nao existe em
Localization/Json/Locales/pt-BR.json, ou o componente usa default hardcoded em vez do
localizer. ClassificaÁ~ao por teste: ajustar a expectativa, completar o locale, ou
migrar o default do componente para o localizer.

### BUG REAL (produto) encontrado na triagem (2026-10-28)

XFVirtualList.razor.cs sobrescrevia o parametro do usuario com o valor localizado em
OnInitialized (EmptyText = L("virtuallist.empty", EmptyText)) ó qualquer valor
customizado era perdido. Corrigido: default aplicado apenas quando n„o informado.
VirtualList: 24/24 verde.

**Padr„o sistÍmico** (mesmo bug, outros componentes ó a corrigir):
XFAvatar (5 labels), XFBadge (4 tÌtulos/aria), XFChart (3), e demais que fazem
<Param> = L("chave", <Param>) em OnInitialized/OnParametersSet. Regra correta:
aplicar o default localizado SOMENTE quando o usu·rio n„o informou o valor.

## BUG SISTEMICO ó 210 ocorrencias em 43 componentes (2026-10-28)

Varredura por (?m)^\s*\w+ = L\(" em OnInitialized/OnParametersSet:

| Componente | Ocorrencias | | Componente | Ocorrencias |
|---|---:|---|---|---:|
| XFPdfViewer | 12 | | XFGantt | 5 |
| XFDatePicker | 13 | | XFAutocomplete | 5 |
| XFUpload | 14 | | XFCombobox | 5 |
| XFTransferList | 13 | | XFSignaturePad | 5 |
| XFPagination | 12 | | XFDialog | 5 |
| XFEditForm | 15 | | XFTOC | 5 |
| XFScheduler | 15 | | XFSortableList | 4 |
| XFTreeView | 10 | | XFPivotDataGrid | 4 |
| XFFileInput | 10 | | XFValidationStatus | 4 |
| XFChat | 7 | | XFWizard | 4 |
| XFMultiSelect | 7 | | XFMarkdown | 3 |
| XFDropDownDataGrid | 6 | | XFMessageList | 3 |
| + XFSwitch(2), XFSearch(2), XFSplashScreen(2), XFMenu(2), XFAppShell(2),
XFProgress(1), XFProgressRing(1), XFList(1), XFTable(1), XFChip(1), XFCropper(1),
XFInputMask(1), XFTextInput(1), XFAppBar(1), XFSidebar(1), XFToolbar(1),
XFContextMenu(1), XFPopover(1), XFVirtualList(1 ó ja corrigido)

### Impacto

Todo parametro de customizacao textual desses componentes e descartado quando o
componente È renderizado ó o valor do usuario È sobrescrito pelo default localizado.
Ja corrigidos: XFVirtualList, XFAvatar, XFBadge, XFChart (21 ocorrencias).

### Correcao proposta (codemod)

Por componente: (1) <Param> = L("k", <Param>) -> if (string.IsNullOrWhiteSpace(<Param>)) <Param> = L("k", "fallback");
(2) default da declaracao do parametro vira default!; (3) fallback explicito em pt-BR
com acentuaÁ„o correta; (4) build + teste do componente apos cada arquivo.
