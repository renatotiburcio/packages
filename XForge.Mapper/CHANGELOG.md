# Changelog — XForge.Mapper

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.2.0] - 2026-10-25

### Adicionado
- **Inlining recursivo de planos**: objetos aninhados (MemberInit) e coleções (materializador
  pré-dimensionado com projeção de elemento pré-compilada) são compilados em uma única expression
  tree por plano — a técnica de AutoMapper/Mapster. Grafos complexos ficaram **3,3× mais rápidos**
  (830 ns → 249,5 ns no cenário Pedido + Cliente aninhado + 10 itens) e as alocações caíram para
  **abaixo do AutoMapper** (0.84×; 776 B vs 928 B).
- Detecção de ciclos em build-time (`VisitedPairs` no caminho de construção): grafos com ciclos caem
  automaticamente para o caminho clássico com cache de instâncias; diamantes (mesmo par em ramos
  distintos) são suportados sem falso positivo.
- Fast path do `Map` sem callbacks: um único delegate cria E popula a instância
  (`createAndPopulate`), eliminando a chamada separada de fábrica + população.
- Materialização de coleções com pré-dimensionamento via `ICollection.Count` (elimina o crescimento
  dinâmico do `ToList` sobre `Select` — ~8,5 KB economizados em listas de 1.000 elementos).
- Projeções de elemento compiladas separadamente e injetadas como `Expression.Constant`
  (lambdas citadas/Quote dentro da árvore têm invocação ~3× mais lenta que delegados constantes).

### Alterado
- Coleções com elementos classe→classe agora são inlinadas recursivamente (antes: dispatch por
  elemento via plano). Dicionários e coleções aninhadas com elementos por classe continuam no
  conversor dedicado (paridade de comportamento).
- Mapas aninhados com callbacks (`BeforeMap`/`AfterMap`), `MaxDepth` ou `ConstructUsing` não são
  inlinados — o par cai para o caminho clássico com contexto (execução equivalente à v0.1).
- 14 novos testes de inlining (grafos profundos, diamantes, IgnoreIf + inline, merge com aninhados,
  records com coleções, `HashSet`/array/`IReadOnlyList` de destino) — 52 no total.

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
