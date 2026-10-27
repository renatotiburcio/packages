# Self-Healing Rules â€” XForge Engineer

## VisÃ£o Geral

Regras de correÃ§Ã£o automÃ¡tica que detectam e corrigem problemas comuns sem intervenÃ§Ã£o humana. Rodam como prÃ©-checks antes de commit, build e deploy.

## Regras Ativas

### SH-001: Unused Imports
- **Detecta**: `using` declarations sem uso no arquivo
- **AÃ§Ã£o**: Remove imports nÃ£o utilizados
- **Severidade**: low
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  using System.Linq;
  using System.Collections.Generic;
  using Newtonsoft.Json; // nÃ£o usado

  // DEPOIS (auto-corrigido)
  using System.Linq;
  using System.Collections.Generic;
  ```

### SH-002: Missing Async Suffix
- **Detecta**: MÃ©todos async sem sufixo `Async`
- **AÃ§Ã£o**: Adiciona sufixo `Async` ao nome do mÃ©todo
- **Severidade**: medium
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  public async Task<List<Produto>> GetProdutos()

  // DEPOIS
  public async Task<List<Produto>> GetProdutosAsync()
  ```

### SH-003: Null-Conditional Access
- **Detecta**: Property/field access em variÃ¡vel que pode ser null
- **AÃ§Ã£o**: Adiciona `?.` (null-conditional operator)
- **Severidade**: high
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  var nome = cliente.Nome.ToUpper();

  // DEPOIS
  var nome = cliente?.Nome?.ToUpper();
  ```

### SH-004: Missing Null Check
- **Detecta**: Resultado de FirstOrDefault()/SingleOrDefault() usado sem null check
- **AÃ§Ã£o**: Adiciona `if (result == null) return NotFound();`
- **Severidade**: high
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
  return Ok(produto.Nome); // NullReferenceException!

  // DEPOIS
  var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
  if (produto == null) return NotFound();
  return Ok(produto.Nome);
  ```

### SH-005: Async Over Sync
- **Detecta**: Chamadas sÃ­ncronas (.Result, .Wait(), SaveChanges()) em contexto async
- **AÃ§Ã£o**: Substitui por variante Async
- **Severidade**: high
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  _context.SaveChanges();

  // DEPOIS
  await _context.SaveChangesAsync();
  ```

### SH-006: Missing Using Declaration
- **Detecta**: IDisposable sem using/await using
- **AÃ§Ã£o**: Adiciona `using` declaration ou `await using`
- **Severidade**: medium
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  var stream = new FileStream("file.txt", FileMode.Open);
  var reader = new StreamReader(stream);

  // DEPOIS
  await using var stream = new FileStream("file.txt", FileMode.Open);
  await using var reader = new StreamReader(stream);
  ```

### SH-007: Parameterized Query
- **Detecta**: SQL query com interpolaÃ§Ã£o de string
- **AÃ§Ã£o**: Converte para parameterized query
- **Severidade**: critical (security)
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  var result = await _context.Produtos
      .FromSqlRaw($"SELECT * FROM Produtos WHERE Nome = '{nome}'")
      .ToListAsync();

  // DEPOIS
  var result = await _context.Produtos
      .FromSqlRaw("SELECT * FROM Produtos WHERE Nome = {0}", nome)
      .ToListAsync();
  ```

### SH-008: Secret Detection
- **Detecta**: PadrÃµes de secret no cÃ³digo (API key, password, token)
- **AÃ§Ã£o**: Move para variÃ¡vel de ambiente ou appsettings.json
- **Severidade**: critical (security)
- **Auto-fix**: parcial (movÃª-lo, nÃ£o o valor)
- **Exemplo**:
  ```csharp
  // ANTES
  var apiKey = "sk-1234567890abcdef";

  // DEPOIS
  var apiKey = _configuration["ExternalServices:ApiKey"]
      ?? throw new InvalidOperationException("ApiKey not configured");
  ```

### SH-009: Cancellation Token
- **Detecta**: MÃ©todos async sem CancellationToken parameter
- **AÃ§Ã£o**: Adiciona `CancellationToken cancellationToken = default` como Ãºltimo parÃ¢metro
- **Severidade**: medium
- **Auto-fix**: sim
- **Exemplo**:
  ```csharp
  // ANTES
  public async Task<List<Produto>> GetProdutosAsync()

  // DEPOIS
  public async Task<List<Produto>> GetProdutosAsync(CancellationToken cancellationToken = default)
  ```

### SH-010: IHttpClientFactory
- **Detecta**: `new HttpClient()` direto
- **AÃ§Ã£o**: Substitui por IHttpClientFactory injeÃ§Ã£o
- **Severidade**: high (performance)
- **Auto-fix**: parcial (sugere refactoring)

### SH-011: Missing ConfigureAwait
- **Detecta**: Await sem ConfigureAwait(false) em bibliotecas/classes de serviÃ§o
- **AÃ§Ã£o**: Adiciona `ConfigureAwait(false)` apÃ³s await
- **Severidade**: low
- **Auto-fix**: sim

### SH-012: Disposable Pattern
- **Detecta**: Classe que implementa IDisposable mas nÃ£o chama GC.SuppressFinalize
- **AÃ§Ã£o**: Completa o padrÃ£o Dispose
- **Severidade**: medium
- **Auto-fix**: parcial



### SH-013: SVG Comment Strip (License Hygiene)
- **Detecta**: Qualquer arquivo `.svg` no repo que contem comentarios `<!--...-->` (especialmente metadata de Font Awesome Pro 7.1.0, Bootstrap Icons, Material Icons, ou qualquer lib Commercial)
- **Ação**: Remove comentarios HTML/XML do arquivo SVG antes de commit, MAS preserva elementos `<title>`/`<desc>` (W3C SVG accessibility)
- **Severidade**: **critical (license compliance)**
- **Auto-fix**: sim
- **Padrao problematico**:
  ```xml
  <!--! Font Awesome Pro 7.1.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2025 Fonticons, Inc. -->
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
    <path d="M..."/>
  </svg>
  ```
- **Padrao correto** (apos SH-013):
  ```xml
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
    <title>House icon</title>
    <path d="M..."/>
  </svg>
  ```
- **Implementacao de referencia**: `SvgContentSanitizer.cs` (criado em T078)
  ```csharp
  public static class SvgContentSanitizer
  {
      private static readonly Regex CommentRegex = new(
          @"<!--.*?-->",
          RegexOptions.Compiled | RegexOptions.Singleline);

      public static string Sanitize(string svg)
      {
          if (string.IsNullOrEmpty(svg)) return svg;
          var sanitized = CommentRegex.Replace(svg, string.Empty);
          sanitized = PromoteTitleFromComment(sanitized);
          return sanitized.Trim();
      }
  }
  ```
- **Validacao automatica**:
  - `Assert_No_SVG_Comments(svgContent)` — regex `<!--.*?-->` retorna 0 matches
  - `Assert_No_External_License_Traces(svgContent)` — zero matches de `Font Awesome|@fontawesome|Fonticons|Commercial License|fontawesome\.com`
- **Referencias**:
  - D039 (DECISIONS.md) — License Hygiene rationale
  - T078 (IconCatalog refactor) — implementacao primaria
  - T097 (SVG optimizer) — implementacao secundaria
  - W3C SVG Accessibility: https://www.w3.org/TR/SVG-access/

## ConfiguraÃ§Ã£o

As regras sÃ£o avaliadas nesta ordem:
1. **Critical** (SH-007, SH-008, **SH-013**) â€” sempre aplicar
2. **High** (SH-003, SH-004, SH-005, SH-010) â€” aplicar com confirmaÃ§Ã£o
3. **Medium** (SH-002, SH-006, SH-009, SH-012) â€” aplicar se auto-fix habilitado
4. **Low** (SH-001, SH-011) â€” aplicar silenciosamente

## IntegraÃ§Ã£o com Quality Gates

```
PrÃ©-commit â†’ SH-001, SH-003, SH-004, SH-005, SH-007, SH-008, **SH-013** (license hygiene)
PrÃ©-build  â†’ SH-002, SH-006, SH-009, SH-013
PrÃ©-push   â†’ SH-010, SH-014, SH-015, SH-016
PÃ³s-merge  â†’ Todos os SH rules
```

## MÃ©tricas

| MÃ©trica | Meta |
|---------|------|
| Auto-fix success rate | > 95% |
| False positive rate | < 5% |
| Time per check | < 2s |
| Issues caught per build | 3-8 (mÃ©dia) |

