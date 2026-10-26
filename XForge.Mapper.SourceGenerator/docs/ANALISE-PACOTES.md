# Análise REA — Source generators da categoria (XForge.Mapper.SourceGenerator)

Consolidação da análise estática dos source generators concorrentes, feita com REA (inspeção de
assemblies managed, sem execução de código dos alvos) sobre os pacotes extraídos do NuGet.

## Alvos analisados

| Pacote | Versão | Artefato | Bytes | Evidência |
|---|---|---|---:|---|
| Riok.Mapperly (generator) | 4.3.1 | `analyzers/roslyn4.11/dotnet/cs/Riok.Mapperly.dll` | 853.504 | 596 tipos, 3.861 métodos, 676 literais |
| Mediator.SourceGenerator (martinothamar/Mediator) | 3.0.2 | `analyzers/dotnet/cs/Mediator.SourceGenerator.dll` | 1.181.184 | 608 tipos, 4.410 métodos, 1.079 literais |

*(A análise de Mapperly 4.3.1 como solução de mapeamento — convenções, API, limitações — está em
`../XForge.Mapper/docs/ANALISE-PACOTES.md`; este documento cobre a camada de geração de código.
A análise de 6 bibliotecas de mediator/validação está em `../XForge.Mediator/docs/ANALISE-PACOTES.md`
e `../XForge.Validation/docs/ANALISE-PACOTES.md`.)*

## Achados — Riok.Mapperly (generator)

### Pipeline incremental (IIncrementalGenerator)
- Ponto de entrada `Riok.Mapperly.MapperGenerator` (token `0x0200004b`) com `Initialize`,
  `BuildDescriptor`, `BuildDefaults`, `BuildCompilationDiagnostics` e constantes de passo em
  `MapperGeneratorStepNames` (`BuildCompilationContext`, `BuildMapperDefaults`, `BuildMappers`,
  `ReportDiagnostics`) — confirma o pipeline de 4 estágios: contexto → defaults → geração → diagnóstico.
- Descoberta via `SyntaxProvider` sobre classes anotadas com o atributo `[Mapper]`.

### Modelo interno
- `Symbols.CompilationContext` / `MapperDeclaration` / `MappingMethodParameters` /
  `RuntimeTargetTypeMapping` — parâmetros de método de mapeamento (inclusive mapeamento por tipo
  de runtime, para dispatch polimórfico).
- `Symbols.Members` (`PropertyMember`, `FieldMember`, `ConstructorParameterMember`,
  `MemberPathGetter/Setter`) — caminhos de membro tipados: getter/setter por símbolo, sem reflexão.

### Emissão por "mapping" (o achado mais importante)
`Descriptors.Mappings.*` — cada forma de código gerado é um tipo próprio: `DirectAssignmentMapping`,
`NewObjectMapping`-como-`CtorMapping`, `CompositeMapping`, `DelegateMapping`, `CastMapping`,
`EnumerableMapping` (Select), `ArrayForMapping`/`ArrayForEachMapping`/`ArrayCloneMapping`,
`ForEachAddEnumerableMapping`, `ForEachSetDictionaryMapping`,
`DerivedTypeSwitchMapping`/`DerivedTypeIfExpressionMapping`/`DerivedExistingTargetTypeSwitchMapping`
(dispatch de tipos derivados).
`ObjectFactories.*` (com `GenericTargetObjectFactory` etc.) decide a construção do destino
(ctor sem parâmetros, ctor por nome, factory do usuário).

**Conclusão de design**: Mapperly traduz cada nó do grafo em uma *estrutura de decisão*, não em
string de template — o emissor vira sintaxe via `SyntaxFactory` (`Emit.SyntaxFactoryHelper`),
com indentação controlada (`SyntaxIndentationExtensions`). Nosso `XForge.Mapper.SourceGenerator`
segue a mesma arquitetura: `MappingCode` (decisões) + `CodeEmitter` (emissão via `SyntaxFactory`).

### Limitações observadas no artefato
- `UnsafeAccess.*` — usa `UnsafeAccessorAttribute` para acessar membros privados (não portado em v0.1).
- O gerador depende de `Riok.Mapperly.Abstractions` (lib separada) para o atributo — mesmo modelo
  que adotamos: `[XForgeMap]` vive no pacote `XForge.Mapper` (lib).

## Achados — Mediator.SourceGenerator (generator)

### Pipeline incremental
- `IncrementalMediatorGenerator` + `CompilationAnalyzer`/`CompilationAnalyzerContext` + `SyntaxReceiver`
  (fallback não-incremental) — análise das classes que implementam os contratos
  (`IRequestHandler<,>`, `INotificationHandler<>`, `IStreamPipelineBehavior<,>`…).

### Emissão via templates Scriban
- Literais observados: `resources/Mediator.bbn-cs`, `resources/MediatorFallback.bbn-cs`,
  arquivo gerado `Mediator.g.cs` — **o gerador do Mediator usa Scriban (template engine)**,
  com fallback para quando o assembly referenciado não está disponível.
- Modelos de template (`RequestMessageModel`, `RequestMessageHandlerModel`,
  `RequestMessageHandlerWrapperModel`, `PipelineBehaviorModel`, `NotificationPublisherTypeModel`)
  expõem propriedades como `FullNamespace`, `MessageType`, `TypeName`, `IsStreaming`,
  `MessageHandlerDelegateName`, `PipelineHandlerTypeName`.

### Registry + dispatch gerado
- Namespace gerado `.Internals` contendo `HandlerWrapper`, `HandlerBase<TResponse>`,
  `IMessageHandlerBase`, `IStreamMessageHandlerBase` — **um wrapper tipado por par
  (request, response)** que encadeia o handler com os pipeline behaviors, resolvido por um
  registry gerado; delegates `MessageHandlerDelegate<TRequest, TResponse>` e
  `StreamHandlerDelegate<TRequest, TResponse>` com `ValueTask`/`IAsyncEnumerable`.

**Conclusão de design**: o caminho de dispatch é *wrapper tipado por par de tipos* + registry
estático, NÃO dicionário de `object`. É exatamente o formato necessário para o
`XForge.Mediator.SourceGenerator` (roadmap): cada `(TRequest, TResponse)` recebe um wrapper
gerado que compõe behaviors + handler em uma única chamada.

## Matriz de consolidação

| Recurso | Mapperly SG | Mediator SG | **XForge.Mapper.SourceGenerator v0.1** |
|---|:-:|:-:|:-:|
| Incremental generator | ✅ | ✅ | ✅ |
| Métodos parciais como contrato | ✅ | ✅ | ✅ (`Map`/`Map(…, dest)`/`Map(list)`) |
| Emissão por SyntaxFactory (sem template) | ✅ | ❌ (Scriban) | ✅ |
| Convenção por nome (case-insensitive) | ✅ | n/a | ✅ |
| Aninhados com null-guard recursivo | ✅ | n/a | ✅ (MemberInit) |
| Coleções com materializador pré-dimensionado | ✅ | n/a | ✅ (função local + `ICollection.Count`) |
| Records (ctor por nome) | ✅ | n/a | ✅ (membros init-only não reatribuídos) |
| Enum→enum / enum→string | ✅ | n/a | ✅ (cast / ToString) |
| Detecção de ciclo em build-time | ✅ | n/a | ✅ (par repetido no caminho → XFG005, membro ignorado) |
| Diagnósticos por membro não mapeado | ✅ | ✅ | ✅ (XFG002/XFG003) |
| Atributo em pacote lib separado | ✅ | ✅ | ✅ (`XForge.Mapper`) |
| Dispatch polimórfico em runtime | ✅ | n/a | 🗺️ roadmap |
| Conversores arbitrários (usuário) | ✅ | n/a | 🗺️ roadmap (v0.1 vira aviso) |
| Streams (IAsyncEnumerable) | n/a | ✅ | n/a |

## Decisões de design do XForge.Mapper.SourceGenerator

1. **Mesmas convenções do motor de runtime** — o generator é um *espelho* do `PlanBuilder`:
   nomes, aninhados, coleções, records. Onde o motor tem caminho clássico (ciclos), o generator
   ignora o membro com aviso (XFG005) —Não há estado de execução para cache de instâncias.
2. **Emissão via `SyntaxFactory`** (como Mapperly) — sem dependência de engine de templates;
   controle total de formatação e sem riscos de escaping.
3. **Atributo no pacote lib** (`XForge.Mapper`) e generator no pacote analyzer que referencia o
   lib — o consumidor recebe os dois por dependência transitiva.
4. **Diagnósticos honestos**: XFG001 (classe não-parcial), XFG002 (tipos incompatíveis),
   XFG003 (membro sem origem), XFG004 (assinatura não suportada), XFG005 (ciclo).
5. **Materializador local pré-dimensionado** — mesmo formato do `MaterializeList` do runtime
   (paridade de custo: `ICollection.Count` + skip de nulos).

## Riscos e limitações conhecidas (v0.1)

- Sem suporte a conversores definidos pelo usuário no generator (membro fica default + XFG002) —
  usar o motor de runtime nesses casos.
- Sem dispatch polimórfico por tipo de runtime (roadmap).
- Sem `IgnoreIf`/`MaxDepth` no generator — essas features são do motor de runtime.
- Sem materialização `HashSet<T>`/dicionários (v0.1 = List/array).

## Como reproduzir a análise

```bash
# extrair os nupkgs (feito em C:\Users\renat\AppData\Local\Temp\opencode\rea-nuget\ext)
Invoke-WebRequest https://api.nuget.org/v3-flatcontainer/riok.mapperly/4.3.1/riok.mapperly.4.3.1.nupkg -OutFile riok.mapperly.zip
Expand-Archive riok.mapperly.zip -DestinationPath ext\riok.mapperly.4.3.1

Invoke-WebRequest https://api.nuget.org/v3-flatcontainer/mediator.sourcegenerator/3.0.2/mediator.sourcegenerator.3.0.2.nupkg -OutFile mediator.zip
Expand-Archive mediator.zip -DestinationPath ext\mediator.sourcegenerator.3.0.2

# inspecionar com REA MCP (inspect_managed_members / inspect_analysis_view por collection)
# assemblies:
#   ext\riok.mapperly.4.3.1\analyzers\roslyn4.11\dotnet\cs\Riok.Mapperly.dll
#   ext\mediator.sourcegenerator.3.0.2\analyzers\dotnet\cs\Mediator.SourceGenerator.dll
```
