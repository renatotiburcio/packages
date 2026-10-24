# Changelog — XForge.Mapper

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.1.0] - 2026-10-23

### Adicionado
- Configuração fluente: `CreateMap` + `Map` (seletor de destino e fonte em expressões únicas), `MapWith`,
  `Ignore`, `IgnoreIf`, `ConstructUsing`, `BeforeMap`/`AfterMap` (com e sem `IMapper`),
  `Include`/`IncludeBase`, `MaxDepth`, `ReverseMap`/`TwoWays`.
- Uso fluente: `MapTo`, `MapOver`, `MapListTo` (com `IMapper` injetado ou mapper estático default).
- Motor: planos por par de tipos com getters/setters/conversores tipados compilados uma única vez
  (sem `Expression.Compile` de árvore inteira); `MappingContext` criado apenas quando o plano exige
  (ciclos, `MaxDepth`, callbacks com mapper).
- Convenção automática: nome case-insensitive, propriedades e campos, objetos aninhados e coleções
  (`List`, arrays, `HashSet`, `IEnumerable`/`IReadOnly*`, `Dictionary`) sem `CreateMap` explícito.
- Polimorfismo: dispatch por runtime type com resolução de hierarquia (`Include` + maps derivados).
- Merge: `Map(source, existing)` com callbacks (padrão PATCH).
- Ciclos: cache de instâncias por grafo (cache antes de popular) e `MaxDepth` (níveis além do limite ficam default).
- Records/imutáveis: binding de construtor por nome com conversões.
- Conversões: enums (nome/numérico), strings (parse/format invariante), nullables (sem boxing no caso comum),
  operadores `implicit`/`explicit`, `IConvertible`, coleções/dicionários com conversor de elemento.
- Validação: `Validate()` + `RequireDestinationMemberSource` com `MappingValidationException` detalhada.
- DI: `AddXForgeMapper` (IMapper singleton).
- Multi-target: `netstandard2.0` + `net8.0`.
- 39 testes + documentação de análise REA de 4 bibliotecas da categoria.
