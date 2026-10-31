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

## Campanha do bug sistemico ó CONCLUIDA (2026-10-28)

201 de 210 ocorrencias corrigidas em 43 componentes (a restante e mensagem de exceÁ„o
em catch do Cropper ó correta como estava). 10 lotes de codemod, build verde em todos,
0 regressıes.

Resultado final (3 categorias + overlays): Forms 1154/1183, Navigation 569/570,
DataDisplay 1173/1179, Overlays 162/163.

## Residuo pos-codemod (2026-10-28) ó testes esperamStrings sem acento

~36 falhas restantes, majoritariamente Assert.Contains onde o teste espera texto
SEM acento ("Abrir calendario", "Alteracoes nao salvas", "Abrir opcoes", "Solte
arquivos") e o componente agora renderiza COM acento correto (locale/defaults
corrigidos). Correcao: atualizar as expectativas dos testes para as formas acentuadas.

Excecoes reais (gap de componente, nao de teste):
- XFColorInputExtendedTest: espera classe h-14 w-14 no swatch Lg (nao renderizada)
- XFDropDownDataGridExtendedTest: espera "Limpar" (label de clearable)

Fila: (1) atualizar expectativas dos testes; (2) corrigir os 2 gaps de componente.

## Expectativas dos testes atualizadas (2026-10-28)

21 ocorrencias sem acento corrigidas em 8 arquivos de teste: "Abrir calendario"?calend·rio,
"Alteracoes nao salvas"?n„o salvas, "Abrir opcoes"?opÁıes, "Proxima"?PrÛxima.
Forms 1154 ? 1163; totais: Navigation 569/570, DataDisplay 1173/1179, Overlays 162/163,
Feedback 366/368, Layout 370/372, Charts 21/21, Editors 14/14.

## Oportunidade de performance: ShouldRender (2026-10-28)

Diagnostico: 0 de 277 componentes sobrescrevem ShouldRender(). Em Blazor, TODO
StateHasChanged (keystroke em input, hover, timer, callback do pai) re-renderiza a
subarvore inteira do componente.

Alvo de maior valor identificado: XFChart.OnAfterRenderAsync ó NAO guardado por
irstRender: a cada render ele re-invoca o modulo JS (interop caro) para redesenhar o
grafico, mesmo quando os dados nao mudaram.

Plano (proxima frente): adicionar ShouldRender com snapshot dos parametros que dirigem o
render nos componentes de maior custo ó charts (JS interop), grids grandes, editores e
inputs de texto. Padrao: snapshot no primeiro render + comparacao em ShouldRender;
retornar false impede o render redundante (e, no caso do chart, o interop repetido).

## Frente ShouldRender ó 12 componentes (2026-10-29)

XFSwitch, XFCheckbox, XFTextInput, XFAutocomplete, XFSelect, XFDropDownDataGrid,
XFMultiSelect, XFDataGrid, XFUpload, XFTransferList, XFDatePicker, XFScheduler + XFChart
com guarda de interop JS (n„o repinta o gr·fico quando os par‚metros n„o mudam).

Padr„o consolidado: assinatura de render (string.Join dos par‚metros visuais) comparada
em ShouldRender(); no XFChart, OnAfterRenderAsync tambÈm pula o mÛdulo JS quando a
assinatura n„o mudou. 0 regressıes funcionais em todos os lotes.

### Expansao (2026-10-29): 22 componentes com ShouldRender

Acrescimo de XFSearch, XFValidationStatus, XFDialog, XFChip, XFAppBar, XFToolbar, XFAlert
ao lote original (Switch, Checkbox, TextInput, Autocomplete, Select, DropDownDataGrid,
MultiSelect, DataGrid, Upload, TransferList, DatePicker, Scheduler, PdfViewer, Markdown)
+ XFChart com guard de interop JS. Zero regressoes funcionais em todos os lotes.

### Consolidacao (2026-10-29): 42 componentes com ShouldRender

Resumo por categoria: inputs (11), dados/charts (12), navegaÁ„o (10), overlays/feedback (7),
botoes (3), layout (3). Todos os componentes de render caro cobertos. Padrao consolidado:
assinatura (string.Join) dos parametros visuais; build+teste a cada lote; 0 regressoes.

## Auditoria e correcao do ShouldRender (2026-10-29/30)

### Problema encontrado
O padrao implementado (assinatura so com parametros) ignorava **estado interno** dos
componentes. Um probe bUnit confirmou a semantica exata do Blazor (.NET 10):

    protected void StateHasChanged() {
        if (_hasPendingQueuedRender) return;
        if (_hasNeverRendered || ShouldRender() || ...) { _renderHandle.Render(_renderFragment); }
    }

ShouldRender e consultado em **todo** render (eventos e parametros) - a unica excecao
e a primeira renderizacao (_hasNeverRendered). Ou seja: qualquer estado interno que
mudasse o markup sem passar por parametro tinha o render bloqueado.

Regressao real comprovada por teste: XFMultiSelect Extended_Escape_ClosesDropdown e
Extended_Tab_ClosesDropdown falhavam com o guard e passavam sem ele.

### Componentes com guard removido (22)
XFButton, XFSplitButton, XFMarkdown, XFPdfViewer, XFScheduler, XFTreeView, XFDatePicker,
XFDropDownDataGrid, XFMultiSelect, XFTransferList, XFCepInput, XFAccordion, XFMenu,
XFTabs, XFToolbar, XFContextMenu, XFDialog, XFPopover, XFDragDrop, XFSortableList,
XFColorPicker, XFDataGrid

Motivo: estado interno que dirige o render (IsOpen, HighlightedIndex, SearchText,
currentDate, arraste, copiado, pagina atual, foco, X/Y do menu, _clicking, TrapActive...).
Nesses componentes a otimizacao nao compensa o risco.

### Componentes com assinatura estendida (6)
XFSlider, XFRangeSlider, XFSearch, XFDatePicker(removido depois), XFMultiSelect(removido),
XFDropDownDataGrid(removido). Slider/RangeSlider/Search mantiveram pois seu CurrentValue
e derivado de parametros (verificado), e a assinatura passou a incluir HasValidationError
e FirstValidationMessage (mensagens de validacao vinham do EditContext cascateado, fora
dos parametros).

### Componentes que mantem o guard (42)
Verificados sem estado interno real: apenas caches derivados (GetComponentClass) e
campos de interop JS. Ex.: XFChart (desenho proprio com _drawnSignature para nao
reinvocar o modulo JS), XFBarcode (encoders estaticos), XFList, XFPagination,
XFBarcode, cards, chips, badges, toasts, alerts, progress, skeleton, grids, stacks,
tabelas, charts SVG (Candlestick, Funnel, Heatmap, Radar, WordCloud), Latam e Editores.

### Resultado dos testes (filtros por categoria)
Baseline comitado: 60 falhas pre-existentes (divida conhecida ~0,7%).
Apos a correcao: 54 falhas. Nenhuma regressao nova; 6 testes consertados.

### Auditoria de fontes externas de estado (2026-10-30)

Varredurafinal sobre os componentes com guard procurou a classe de risco que o
detector de estado nao pega: estado que entra por **eventos externos** (nao por
parametros nem por handlers proprios). Encontrou e corrigiu 2 casos:

- XFValidationStatus: assinatura estendida com IsValid, HasMessages e
  HasModifiedFields ó o componente se inscreve em EditContext.OnValidationStateChanged
  e OnFieldChanged; sem isso as mensagens de validacao nao renderizavam.
- XFRailItem: assinatura estendida com _isActive e NavigationState.CurrentBrand ó
  o item se inscreve em NavigationState.ActiveItemChanged/BrandChanged.

XFSlider e XFRangeSlider apareceram na varredura por usarem EditContext cascateado,
mas ja estavam cobertos pela extensao anterior (HasValidationError, FirstValidationMessage).

Teste Perf_ValidationStatus_RendersUnderThreshold falhou uma vez por carga da maquina
(varreu 2,9s vs 0,5s) e passou 3/3 em isolamento ó flake, nao regressao.

## Fechamento da frente ShouldRender (2026-10-30)

### Numeros finais
130 componentes com assinatura de render (47% dos 277 .razor.cs), 71 confirmados
stateful e portanto sem guard, 2 validadores que nao renderizam nada (fora de
escopo), e o restante sao arquivos sem parametros.

### Os 71 stateful (nao adicionar guard sem incluir o estado na assinatura)
XFAccordion, XFAccordionItem, XFAIChat, XFAnimate, XFAvatar, XFBreakpointObserver,
XFButton, XFCalendar, XFCarousel, XFCepInput, XFChat, XFCnpjInput, XFCode, XFCollapse,
XFColorInput, XFColorPicker, XFCombobox, XFContextMenu, XFCpfInput, XFDataGrid,
XFDateInput, XFDatePicker, XFDialog, XFDragDrop, XFDragDropZone, XFDrawer,
XFDropDownDataGrid, XFDropdownMenu, XFDropZone, XFEditForm, XFFileInput, XFFocusTrap,
XFGanttAdvanced, XFImage, XFInputMask, XFMap, XFMarkdown, XFMenu, XFMultiSelect,
XFMultiSplitter, XFNumberInput, XFOneTimeInput, XFPageProgressProvider,
XFPasswordStrength, XFPdfViewer, XFPopover, XFRail, XFRangeNavigator, XFRating,
XFRichTextEditor, XFSankey, XFScheduler, XFSchedulerAdvanced, XFSignaturePad,
XFSortableList, XFSparkline, XFSpeechToText, XFSplitButton, XFSplitter, XFSpreadsheet,
XFTabs, XFTextarea, XFTileLayout, XFTimeInput, XFTimeSpan, XFToastProvider, XFToolbar,
XFTransferList, XFTreeView, XFVehiclePlateInput, XFWizard

Tipos de estado encontrados: atribuicao em handler; colecao readonly mutada por
metodos (Insert/Remove/Clear); subscription de servico ou do EditContext; servico
injetado com callbacks. Os tres ultimos so foram pegos por verificacao manual.

### Regra para manutencao futura
Antes de adicionar ShouldRender a um componente: (1) listar todo estado proprio
(incluindo colecoes readonly e expressoes derivadas); (2) listar estado que entra por
eventos externos (EditContext, servicos, timers); (3) so usar assinatura se TODO o
estado que direciona o render entrar nela ó caso contrario, nao usar o padrao.

## Migracao do app de documentacao (2026-10-30)

### O que migrou
- XForge.DesignSystem.Docs.Shared -> docs/app/XForge.DesignSystem.Docs.Shared
  (shell do manual, navegacao, contratos, produto, servicos de idioma)
- XForge.DesignSystem.Docs -> docs/app/XForge.DesignSystem.Docs
  (app Blazor WASM: 237 paginas, uma por componente, + App/Program/index)

246 arquivos no app + 25 no Shared. Assets estaticos (_content/XForge.DesignSystem/*)
ja existiam no wwwroot do core e continuam resolvendo pelo caminho padrao.

### Namespaces normalizados no core
O core migrado tinha 40 arquivos ainda declarando namespaces legados
(XForge.Components.*). Isso impedia o app de documentacao de compilar e deixava a
API publica com duas raizes de namespace. Normalizado para
XForge.DesignSystem.Components.<pasta> (namespace segue a pasta):

  Base, Charts, DataGrid, Diagram, Editors, Forms.Draft, Gantt(->Scheduler),
  Latam, Maps, Pdf, Pivot, Scheduler

Ajustados: 40 declaracoes em .cs, 39 @namespace em .razor, 44 usings nos testes,
e as strings de catalogo (new("Namespace", ...)). 79 arquivos no total.

## Descarte do XForge.DesignSystem.Docs.AI (2026-10-31)

Decisao do usuario: descartar definitivamente a variante AI do manual. Justificativa:
ela depende do satelite XForge.DesignSystem.AI, removido na reorganizacao ó restauralo
reinflaria o escopo que foi enxugado. O nucleo mantem apenas o chatbot proprio
(XFAiChatWithEmji em Components/AI/Smart), que segue documentado normalmente.

### Consequencias aplicadas
1. Catalogo: 64 itens /ai/* sem pagina foram removidos (113 itens, zero links quebrados).
2. Testes de arquitetura reescritos para a estrutura real (XForgeEcosystemBoundaryTest):
   - GetRepositoryRoot procurava XForge.slnx, removido na reorganizacao ó agora
     identifica a raiz por XForge.DesignSystem + NuGet.config (13 falhas -> 0).
   - Testes que exigiam o projeto AI viraram afirmacao de que ele NAO existe.
   - DocsApp_ShouldDeclareTailwindPipeline passou a validar que cada glob de @source
     resolve para uma pasta real.
3. ComponentCoverageTests reescrito: a lista [InlineData] manual tinha 64 entradas de
   componentes inexistentes (satelite AI + ferramentas). Agora descobre os componentes
   por reflexao (>= 200), com piso de sanidade, e trata tipos genericos (XFDataGrid1).
4. NoVersionedComponentNamingTest: mesma causa raiz (XForge.slnx); agora varre
   src/ e docs/app.
5. Bug real encontrado no caminho: os @source do app.css do manual apontavam para o
   layout antigo (o Tailwind resolve relativo ao wwwroot, nao ao projeto). Corrigido
   para ../../XForge.DesignSystem.Docs.Shared e ../../../../src/XForge.DesignSystem.

### Regra de marca (esclarecida)
As regras :root[data-brand=...] vivem no CSS do nucleo (xf-designsystem.css), carregado
via <link>; o app.css do manual tem apenas Tailwind. Teste ajustado para essa realidade.

### Limpeza de peso morto (2026-10-31)

Alem dos testes corrigidos, 48 arquivos de teste do satelite AI estavam no disco mas
EXCLUIDOS da compilacao via <Compile Remove> no csproj (Componente\AI\**, Analyzers,
Benchmarks, XFGanttAdvancedTest, XFAiServiceCollectionExtensionsTest). Nao compilavam
por referencia namespaces inexistentes (XForge.DesignSystem.Components.AI.Accounting,
.Financial, .Forms, .Common). Removidos, e o csproj ficou sem nenhuma exclusao.

Suite completa (run interrompido aos 97% pelo host travar, contagem estavel):
  antes: 199 falhas unicas / depois: 121 falhas unicas
  78 testes consertados, 0 regressoes novas.
