# Changelog — XForge.Mapper.SourceGenerator

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.1.0] - 2026-10-26

### Adicionado
- Source generator (IIncrementalGenerator) que emite o corpo de métodos parciais em classes
  marcadas com `[XForgeMap]` (atributo no pacote `XForge.Mapper`).
- Formas suportadas: `Dest Map(Src source)` (cria e popula), `void Map(Src source, Dest destination)`
  (merge) e `List<Dest> Map(IEnumerable<Src> source)` (lista inteira).
- Convenções idênticas ao motor de runtime: nomes case-insensitive, propriedades e campos,
  aninhados com null-guard recursivo, coleções materializadas por função local pré-dimensionada
  (`ICollection.Count`), records por binding de construtor (membros init-only não reatribuídos),
  enum→enum (cast) e enum→string (`ToString()`).
- Detecção de ciclos em build-time: membros que fecham ciclo são ignorados com diagnóstico
  (XFG005) — o motor de runtime resolve ciclos com cache de instâncias.
- Diagnósticos: XFG001 (classe não-parcial), XFG002 (tipos incompatíveis),
  XFG003 (membro sem contraparte na origem), XFG004 (assinatura não suportada), XFG005 (ciclo).
- 7 testes com harness Roslyn real: compila o código gerado, carrega o assembly e valida o
  comportamento em runtime.
- Documentação da análise REA dos generators concorrentes (Mapperly 4.3.1, Mediator 3.0.2) —
  ver `docs/ANALISE-PACOTES.md`.

### Limitações (v0.1)
- Sem conversores definidos pelo usuário no generator (membro fica default + XFG002) — usar o
  motor de runtime nesses casos.
- Sem dispatch polimórfico por tipo de runtime, `IgnoreIf`/`MaxDepth`, `HashSet<T>` ou
  dicionários no generator (roadmap).
