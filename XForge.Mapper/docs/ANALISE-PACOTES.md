# Análise REA — Consolidação dos pacotes de mapeamento (XForge.Mapper)

> Análise estática (shipped-artifact) via MCP **REA** (`inspect_managed_artifact` + `inspect_managed_members`)
> dos pacotes NuGet reais baixados de `api.nuget.org` (`.nupkg` → `lib/**.dll` / `analyzers/**.dll`).
> Evidências: metadados ECMA-335 completos (tipos, campos, métodos, aridades). Estático não carrega/executa o
> assembly; comportamento de runtime é inferência rotulada.

## 1. Pacotes analisados

| Pacote | Versão | DLL analisada | Tipos | Métodos | Evidência (parent ev_) |
|---|---|---|---:|---:|---|
| AutoMapper (LuckyPenny/jbogard) | 12.0.1 (última Apache) | `lib/netstandard2.1/AutoMapper.dll` | 329 | 2252 | `48a6acb7…d63` |
| AutoMapper | 16.2.0 (comercial) | `lib/net8.0/AutoMapper.dll` | 354 | 2481 | `860a8234…75d` |
| Mapster | 10.0.13 | `lib/net8.0/Mapster.dll` | 217 | 1346 | `19105b55…937` |
| Mapster.Core | 10.0.13 | `lib/netstandard2.0/Mapster.Core.dll` | 54 | 236 | `e2a8b3e8…79d` |
| Mapperly (riok) | 4.3.1 | `analyzers/roslyn4.11/dotnet/cs/Riok.Mapperly.dll` (gerador) | 596 | 3861 | `7f73b0b2…812` |
| Mapperly.Abstractions | 4.3.1 | `lib/netstandard2.0/Riok.Mapperly.Abstractions.dll` | 46 | 142 | `41e83be8…52c` |
| EggMapper (eggspot) | 1.17.19 | `lib/net8.0/EggMapper.dll` | 110 | 524 | `87e06c74…460` |

## 2. Superfície observada por biblioteca

### 2.1 AutoMapper 12.0.1 — a referência de simplicidade (namespace `AutoMapper`)
- **Núcleo**: `IMapper`/`IMapperBase`/`Mapper`, `MapperConfiguration` (+ `IConfigurationProvider`),
  `Profile`, `IProfileExpression`, `ResolutionContext`.
- **Fluent config** (evidência de aridades): `cfg.CreateMap<TSrc,TDest>()` → `IMappingExpression<TSrc,TDest>`:
  `ForMember(d => d.X, opt => …)`, `ForPath`, `ForAllMembers`, `ForSourceMember`, `Include/IncludeBase`
  (polimorfismo), `ReverseMap`, `As<T>`. Opções por membro (`IMemberConfigurationExpression<3>`):
  `MapFrom` (lambda/func/string — 8 overloads), `Condition` (5 overloads), `PreCondition` (4),
  `ConvertUsing` (6), `MapAtRuntime`, `SetMappingOrder`, `UseDestinationValue`/`DoNotUseDestinationValue`.
- **Uso**: `Map<TDest>(src)`, `Map(src, dest)` (merge em instância existente!), `Map(src, dest, opts)`
  (`IMappingOperationOptions`: Items, ServiceCtor), `ProjectTo<T>(IQueryable)` (projeção LINQ via
  `AutoMapper.QueryableExtensions` + 5 projection mappers).
- **Runtime** (evidência por namespaces): planos de mapeamento compilados via `Expression.Compile`
  (`Execution.TypeMapPlanBuilder`, `ExpressionBuilder`), `ProxyGenerator`/`ObjectFactory` (gera proxies em
  runtime para destinos sem implementação), `LockingConcurrentDictionary` (cache por `TypePair`/`MapRequest`),
  `ResolutionContext` com `Items`, `InstanceCache` (referências circulares), `TypeDepth`.
- **Convenções**: `INamingConvention` (PascalCase, LowerUnderscore, ExactMatch) + `ISourceToDestinationNameMapper`
  (`PrePostfixName`, `ReplaceName`, `MemberNameReplacer`, `NameSplitMember` — flattening por convenção).
- **Validação**: `MemberList` (Source/Destination/None), `AssertConfigurationIsValid`,
  `AutoMapperConfigurationException`, `DuplicateTypeMapConfigurationException`.
- **Anotações**: `[AutoMap]`, `[Ignore]`, `[SourceMember]`, `[NullSubstitute]`, `[MappingOrder]`,
  `[ValueResolver]`, `[ValueConverter]`, `[UseExistingValue]`, `[MapAtRuntime]`.
- **Extensões**: `IValueResolver<3>`, `IMemberValueResolver<4>`, `ITypeConverter<2>`, `IValueConverter<2>`,
  `ValueTransformerConfiguration`, `Features` (v12), `MapperRegistry` (mappers built-in: assignable, collections,
  nullable, enum↔enum, string↔enum, parse, dictionaries, dynamic/Expando, constructors).
- **v16** (comercial, evidência de diff): + `AutoMapper.Licensing.*`; API de mapeamento essencialmente a mesma
  (+ `IDestinationFactory`, `ICondition`, `IPreCondition`).

### 2.2 Mapster 10.0.13 — config global + fluent DSL própria
- **Núcleo**: `TypeAdapterConfig` (singleton global, `Default`, `RuleMap`, `When(...)`, `Scan(assembly de IRegister)`,
  `Apply`, `Fork`, `Clone`, `Compile()`/`CompileProjection()`), `TypeAdapterConfig<TSrc,TDest>` via
  `NewConfig()`/`ForType<TSrc,TDest>()` → `TypeAdapterSetter<TSrc,TDest>` fluent (evidência de métodos):
  `Map`, `Ignore`, `IgnoreIf`, `ConstructUsing`, `BeforeMapping/AfterMapping` (6 overloads), `MapToConstructor`,
  `MapWith`/`MapToTargetWith` (substituição do mapeamento por delegate), `Include`, `Inherits`, `Fork`,
  `TwoWays`, `Compile`.
- **Uso**: extensão `Adapt<TDest>(src)` (Extensions), `Adapt(src, dest)` (merge — `IApplyable`/`MapToTarget`),
  `ProjectToType<T>()` (projeção), `MapsterMapper.IMapper`/`Mapper` (`From(src)` fluent → `MapToTarget`/`ProjectToType`).
- **Convenções**: `NameMatchingStrategy` (Exact/Flexible/PascalCase/CamelCase/LowerCase), `ShouldMapMember`,
  `ValueAccessingStrategy`, `DestinationTransform`, `EnumMappingStrategy`, `GetMemberName`.
- **Runtime**: adapters compilados por expressão (`Mapster.Adapters.*`: Class/Collection/Array/Dictionary/Enum/
  Primitive/Record/Nullable/Delegate/ObjectType) + `DynamicTypeGenerator`/`InterfaceDynamicMapper` (proxies de
  interface em runtime), `CompileContext`/`PreCompileArgument`.
- **Anotações (Mapster.Core)**: `[AdaptTo]`/`[AdaptFrom]`/`[AdaptTwoWays]`/`[AdaptIgnore]`/`[AdaptMember]`/
  `[AdaptWith]`/`[UseDestinationValue]`/`[ProjectTo]` + codegen `[GenerateMapper]`/`[Mapper]` com
  `ICodeGenerationRegister`.
- **Modelos**: `Mapster.Models` (ClassMapping, MemberMapping, InvokerModel…), `MapContext` (contexto por escopo).

### 2.3 Mapperly 4.3.1 (riok) — source generator, zero runtime
- **Runtime**: apenas atributos (`Riok.Mapperly.Abstractions`, 46 tipos). O gerador (596 tipos) emite métodos
  `static` em classes parciais decoradas com `[Mapper]` — cada assinatura `public static TDest Map(TSrc src)`
  vira código C# puro.
- **Configuração por atributos** (evidência de propriedades de `[Mapper]`): `PropertyNameMappingStrategy`,
  `EnumMappingStrategy`/`EnumNamingStrategy`/`IgnoreCase`, `ThrowOnMappingNullMismatch`,
  `ThrowOnPropertyMappingNullMismatch`, `AllowNullPropertyAssignment`, `UseDeepCloning`, `EnabledConversions`
  (flags `MappingConversionType`), `UseReferenceHandling`, `IgnoreObsoleteMembersStrategy`,
  `RequiredMappingStrategy`, `IncludedMembers`/`IncludedConstructors`, `PreferParameterlessConstructors`,
  `AutoUserMappings`.
- **Mapeamento manual**: `[MapProperty(Source, Target, StringFormat, Use…)]`, `[MapPropertyFromSource]`,
  `[MapValue]`, `[MapperIgnoreSource/Target(Value)]`, `[MapDerivedType<2>]`, `[MapNestedProperties]`,
  `[MapEnum/MapEnumValue]`, `[ObjectFactory]`, `[UseStaticMapper]`, `[NamedMapping]`, `[MapperDefaults]`.
- **Referências circulares**: `IReferenceHandler`/`PreserveReferenceHandler` + `[ReferenceHandler]`.
- **Validação**: `RequiredMappingStrategy` (Target/Source/Both) em compile-time; zero reflection em runtime.

### 2.4 EggMapper 1.17.19 — clone de AutoMapper + extras
- **API espelhada do AutoMapper** (evidência): `IMapper` (`Map` 6 overloads, **`MapList`**, **`Patch`**),
  `IMapperConfigurationExpression`, `IMappingExpression<TSrc,TDest>` (`ForMember`, `ForPath`, `ReverseMap`,
  `ConstructUsing`, `BeforeMap/AfterMap`, `IncludeBase`, **`IncludeAllDerived`**, **`MaxDepth`**, `ConvertUsing`,
  `ForAllMembers`, `ForAllOtherMembers`, **`Validate`**), `IMemberConfigurationExpression<3>`,
  `ITypeConverter<2>`, `IMemberValueResolver<4>`, `Profile`, `MapperConfiguration`, `ResolutionContext`,
  `QueryableExtensions` (ProjectTo), `MappingException`/`MappingValidationException`, `NonGeneric*` (dispatch
  dinâmico), DI (`EggMapperServiceCollectionExtensions`).
- **Extras próprios**: caches `FastCache`/`PatchCache` — semântica de **Patch** (atualização parcial de
  instância existente) como cidadã de primeira classe.

## 3. Matriz de recursos consolidada

| Recurso | AutoMapper | Mapster | Mapperly | EggMapper | **XForge.Mapper** |
|---|:-:|:-:|:-:|:-:|:-:|
| Fluent config (CreateMap/ForMember) | ✅ | ✅ (NewConfig) | ❌ (atributos) | ✅ | ✅ (fluent end-to-end) |
| Fluent de USO (`Map`/`Adapt`) | ✅ | ✅ | gerado | ✅ | ✅ |
| Convenção por nome (case-insensitive) | ✅ | ✅ | ✅ (strategy) | ✅ | ✅ |
| Naming strategies / flattening | ✅ | ✅ | ✅ parcial | ✅ | ✅ (v0.1: exato/insensível + casing) |
| Custom member mapping (expressão) | ✅ MapFrom | ✅ Map | ✅ [MapProperty] | ✅ ForMember | ✅ `Map(dest, src)` |
| Ignore / IgnoreIf | ✅ / ✅ | ✅ / ✅ | ✅ / ❌ | ✅ / ❌ | ✅ / ✅ |
| ConstructUsing (factory) | ✅ | ✅ | ✅ (factory implícita) | ✅ | ✅ |
| BeforeMap/AfterMap | ✅ | ✅ | ✅ (parcial/código) | ✅ | ✅ |
| Merge em instância existente | ✅ | ✅ (MapToTarget) | ❌ | ✅ (Patch) | ✅ |
| Polimorfismo (Include/derived) | ✅ | ✅ (Include/Inherits) | ✅ [MapDerivedType] | ✅ (+AllDerived) | ✅ |
| Referências circulares / MaxDepth | ✅ (InstanceCache) | ✅ | ✅ (ReferenceHandler) | ✅ MaxDepth | ✅ (contexto sob demanda) |
| Coleções/dicionários/arrays | ✅ | ✅ | ✅ | ✅ | ✅ |
| Enum ↔ nome/número/string | ✅ | ✅ | ✅ (flags) | ✅ | ✅ |
| Records/ctor-only (imutáveis) | ✅ | ✅ | ✅ | ✅ | ✅ (binding por construtor) |
| Validação de configuração | ✅ (Assert) | ✅ (Require*) | ✅ (compile-time) | ✅ Validate | ✅ (`Validate()`, Require*) |
| Projeção LINQ (ProjectTo) | ✅ | ✅ | ❌ | ✅ | 🗺️ roadmap (`XForge.Mapper.Projections`) |
| Atributos de mapeamento | ✅ | ✅ (core) | ✅ (nativo) | ❌ | ✅ v0.2 (roadmap) |
| Sem reflection em runtime | ❌ | ❌ | ✅ | ❌ | ⚠️ paridade AutoMapper/Mapster (delegates compilados por membro); source-gen no roadmap |
| Zero alocação de contexto por mapa | ❌ | ❌ | ✅ | ❌ | ✅ (contexto só quando o plano exige) |
| DI integrada | ✅ (AddAutoMapper) | ✅ (MapsterMapper) | ❌ (estático) | ✅ | ✅ (`AddXForgeMapper`) |
| Licença aberta | ❌ (13+ comercial) | ✅ | ✅ | ✅ | ✅ MIT |

## 4. Decisões de design do XForge.Mapper

1. **Fluent API obrigatória end-to-end** (pedido explícito): configuração
   (`MapperConfiguration.Create(cfg => cfg.CreateMap<A,B>().Map(…).Ignore(…)…)`) e uso
   (`mapper.Map<TDest>(source)`, `mapper.Map(source, existing)`, `mapper.MapList<TDest>(…)`).
   Sintaxe de membro em **uma lambda só** (`Map(d => d.X, s => s.Y)` — estilo Mapster, evita o
   `ForMember(d => d.X, o => o.MapFrom(...))` aninhado do AutoMapper).
2. **Motor: planos de mapeamento por par de tipos com delegates tipados por membro** — cada binding compila
   getter/setter tipados (`PropertyInfo` → `CreateDelegate`) + conversor específico; **sem `Expression.Compile`
   de árvore inteira** (build ~ordens de magnitude mais rápido que AutoMapper/Mapster; execução sem boxing).
   Planos genéricos `MappingPlan<TSource,TDest>` cacheados por `MapperConfiguration`.
3. **Zero alocação de contexto por mapa** — `MappingContext` (cache de instâncias p/ referências circulares,
   Items, MaxDepth) só é criado quando o plano declara necessidade (ciclos detectados, `BeforeMap` com contexto,
   mapas aninhados recursivos).
4. **Recursos v0.1** (matriz acima): convenção por nome, fluent completo (`Map/Ignore/IgnoreIf/ConstructUsing/
   BeforeMap/AfterMap/Include/IncludeBase/ReverseMap/TwoWays`), merge em instância existente, polimorfismo,
   ciclos + `MaxDepth`, coleções/dicionários, enums, nullables, strings, records/ctor-binding, validação
   (`Validate()`, `RequireDestinationMemberSource`), DI (`AddXForgeMapper`), exceções tipadas.
5. **Roadmap explícito**: projeção LINQ (`ProjectTo`), atributos de mapeamento, source generator
   (`XForge.Mapper.SourceGenerator` — dispatch gerado, paridade Mapperly com config em runtime).
6. **Multi-arquitetura**: Clean/CQRS (DTOs), DDD (VOs imutáveis via ctor), Vertical Slices, modular monolith —
   nada no núcleo conhece infraestrutura; `netstandard2.0` + `net8.0`.

### Não entram (escopo v0.1)
Projeção LINQ, codegen de DTOs, ExpandoObject/dynamic, proxy de interface em runtime (geração de tipos),
naming split avançado (PrePostfix/Replace), value transformers globais.

## 5. Evidências REA

- Origem: `.nupkg` de `api.nuget.org` extraídos para
  `C:\Users\renat\AppData\Local\Temp\opencode\rea-nuget\ext` (paths nas evidências).
- IDs de evidência (membros completos): tabela §1. Limitações registradas: inspeção estática não carrega/executa
  o assembly; tokens build-local (sha256+MVID); assinaturas genéricas retornam operandos codificados.
- Obs.: o repositório `LuckyPennySoftware/AutoMapper` é a era legado (pacote `AutoMapper` — as versões 13+ são
  da era jbogard e 15+ comercial; a última Apache é a 12.0.1, analisada aqui como baseline aberto).
