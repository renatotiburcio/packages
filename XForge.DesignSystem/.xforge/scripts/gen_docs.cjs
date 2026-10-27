const fs = require("fs");
const path = require("path");
const COMPONENTS_ROOT = path.join("D:\\dev\\XForge\\components\\src\\XForge.DesignSystem\\Components");
const DOCS_ROOT = path.join("D:\\dev\\XForge\\components\\src\\XForge.DesignSystem.Docs\\Components\\Pages");
const SKIP = new Set(["XFComponentGrid"]);
const DASH = "\u2014";
const TODAY = new Date().toISOString().slice(0, 10);

function walkDir(dir, ext, results) {
  results = results || [];
  var entries = fs.readdirSync(dir, { withFileTypes: true });
  for (var i = 0; i < entries.length; i++) {
    var entry = entries[i];
    var full = path.join(dir, entry.name);
    if (entry.isDirectory()) walkDir(full, ext, results);
    else if (entry.name.endsWith(ext)) results.push(full);
  }
  return results;
}

function getDocName(c) { return c.startsWith("XF") ? c.slice(2) : c; }

function getExistingDocs() {
  if (!fs.existsSync(DOCS_ROOT)) return new Set();
  var docs = new Set();
  var files = fs.readdirSync(DOCS_ROOT);
  for (var i = 0; i < files.length; i++) {
    var f = files[i];
    if (f.endsWith("Docs.razor")) docs.add(f.replace("Docs.razor", ""));
  }
  return docs;
}

function extractNs(s) {
  var m = s.match(/namespace\s+([\w.]+)/);
  return m ? m[1] : null;
}

function extractNsR(s) {
  var m = s.match(/@namespace\s+([\w.]+)/);
  return m ? m[1] : null;
}

function extractParams(s) {
  var params = [];
  var re = /\[Parameter\]\s*(?:public\s+)?([\w<>\?\[\],\s]+?)\s+(\w+)\s*\{\s*get;\s*set;\s*\}(?:\s*=\s*(.+?))?;/g;
  var m;
  while ((m = re.exec(s)) !== null) {
    var def = m[3] ? m[3].trim() : DASH;
    if (def === "") def = DASH;
    params.push({ name: m[2], type: m[1].trim(), "default": def });
  }
  return params;
}

function getCat(rp) {
  var p = rp.split(path.sep);
  return p.length <= 1 ? "General" : p[0];
}

function esc(s) { return s.replace(/\\/g, "\\\\").replace(/"/g, '\\"'); }

function buildParamRows(params) {
  if (!params.length) return 'new("' + DASH + '", "' + DASH + '", "' + DASH + '", T("No parameters documented.", "No parameters documented."))';
  return params.map(function(p) {
    var dv = p["default"] === DASH ? 'T("' + DASH + '", "' + DASH + '")' : '"' + esc(p["default"]) + '"';
    return 'new("' + p.name + '", "' + esc(p.type) + '", ' + dv + ', T("' + p.name + ' parameter.", "' + p.name + ' parameter."))';
  }).join(",\n        ");
}

function generateDoc(compName, ns, category, params) {
  var docName = getDocName(compName);
  var compNameLower = docName.charAt(0).toLowerCase() + docName.slice(1);
  var actualNs = ns || "XForge.DesignSystem.Components";
  var paramRows = buildParamRows(params);
  var o = [];
  o.push('@page "/components/' + compNameLower + '"');
  o.push("@using " + actualNs);
  o.push("");
  o.push("@inject XForgeDocsLanguageService LanguageService");
  o.push("");
  o.push('<XForgeDocsShell ProductLabel="@ProductLabelText"');
  o.push('                 Title="@PageTitleText"');
  o.push('                 Status="@PageStatusText"');
  o.push('                 Items="XForgeDocsCatalog.CoreItems">');
  o.push('    <XFStack Orientation="XFOrientation.Vertical" Gap="6" FullWidth="true">');
  o.push('        <XFCard Variant="XFCardVariant.Outlined" Padding="XFPadding.Xl" AreaRestricted="false">');
  o.push("            <ChildContent>");
  o.push('                <XFStack Orientation="XFOrientation.Vertical" Gap="5" FullWidth="true">');
  o.push('                    <XFStack Orientation="XFOrientation.Horizontal" Gap="2" Wrap="XFWrap.Wrap">');
  o.push('                        <XFBadge Variant="XFBadgeVariant.Solid" Color="XFBadgeColor.Primary" Text="' + compName + '" />');
  o.push('                        <XFBadge Variant="XFBadgeVariant.Soft" Color="XFBadgeColor.Success" Text="WAI-ARIA" />');
  o.push("                    </XFStack>");
  o.push('                    <XFStack Orientation="XFOrientation.Vertical" Gap="2" FullWidth="true">');
  o.push('                        <XFTypography Variant="XFTypographyVariant.H2" Weight="XFTypographyWeight.Bold">');
  o.push('                            @T("Componente ' + docName + ' do XForge Design System.", "' + docName + ' component of XForge Design System.")');
  o.push("                        </XFTypography>");
  o.push('                        <XFTypography Variant="XFTypographyVariant.Body1" Color="XFTypographyColor.Secondary">');
  o.push('                            @T("Description of ' + docName + ' component and its features.", "Description of ' + docName + ' component and its features.")');
  o.push("                        </XFTypography>");
  o.push("                    </XFStack>");
  o.push('                    <div class="grid gap-4 lg:grid-cols-[minmax(0,1fr)_22rem]">');
  o.push('                        <div class="rounded-lg border border-border-light bg-surface-50 p-5">@HeroPreview()</div>');
  o.push('                        <XFStack Orientation="XFOrientation.Vertical" Gap="3" FullWidth="true">');
  o.push("                            @foreach (var item in QuickFacts) { @Tile(item) }");
  o.push("                        </XFStack>");
  o.push("                    </div>");
  o.push("                </XFStack>");
  o.push("            </ChildContent>");
  o.push("        </XFCard>");
  o.push("");
  o.push('        @Section(T("Metadata", "Metadata"), @<div>@NameValueTable(MetadataRows, T("Field", "Field"), T("Value", "Value"))</div>)');
  o.push("");
  o.push('        <XFStack Orientation="XFOrientation.Vertical" Gap="5" FullWidth="true">');
  o.push('            @Section(T("Overview", "Overview"), @<div>@TileGrid(OverviewItems, 3)</div>)');
  o.push('            @Section(T("Purpose", "Purpose"), @<div>@TileGrid(PurposeItems, 3)</div>)');
  o.push('            @Section(T("Problem Solved", "Problem Solved"), @<div>@TileGrid(ProblemItems, 3)</div>)');
  o.push('            @Section(T("When To Use", "When To Use"), @<div>@TileGrid(UseItems, 2)</div>)');
  o.push('            @Section(T("When Not To Use", "When Not To Use"), @<div>@WarningGrid(AvoidItems)</div>)');
  o.push('            @Section(T("Anatomy", "Anatomy"), @<div>@TileGrid(AnatomyItems, 3)</div>)');
  o.push('            @Section(T("Design Principles", "Design Principles"), @<div>@TileGrid(DesignPrincipleItems, 3)</div>)');
  o.push("        </XFStack>");
  o.push("");
  o.push('        <XFStack Orientation="XFOrientation.Vertical" Gap="5" FullWidth="true">');
  o.push('            @Example(T("1. Basic", "1. Basic"), T("Basic example of ' + docName + '.", "Basic example of ' + docName + '."), ExBasic(), CodeBasic)');
  o.push('            @Example(T("2. Advanced", "2. Advanced"), T("Advanced example with custom configurations.", "Advanced example with custom configurations."), ExAdvanced(), CodeAdvanced)');
  o.push("        </XFStack>");
  o.push("");
  o.push('        <XFStack Orientation="XFOrientation.Vertical" Gap="5" FullWidth="true">');
  o.push('            @Section(T("API Reference", "API Reference"), @<div>@ApiTable(ApiRows)</div>)');
  o.push('            @Section(T("Accessibility", "Accessibility"), @<div>@TileGrid(AccessibilityItems, 2)</div>)');
  o.push('            @Section(T("Keyboard", "Keyboard"), @<div>@NameValueTable(KeyboardRows, T("Key", "Key"), T("Action", "Action"))</div>)');
  o.push('            @Section(T("FAQ", "FAQ"), @<div>@NameValueTable(FaqRows, T("Question", "Question"), T("Answer", "Answer"))</div>)');
  o.push('            @Section(T("Changelog", "Changelog"), @<div>@NameValueTable(ChangelogRows, T("Date", "Date"), T("Change", "Change"))</div>)');
  o.push("        </XFStack>");
  o.push("    </XFStack>");
  o.push("</XForgeDocsShell>");
  o.push("");
  return o.join("\n");
}

function generateCodeBlock(compName, docName, category, actualNs, paramRows) {
  var o = [];
  o.push("@code {");
  o.push('    private string ProductLabelText => "' + compName + '";');
  o.push('    private string PageTitleText => "' + docName + '";');
  o.push('    private string PageStatusText => T("Stable", "Stable");');
  o.push("");
  o.push('    private RenderFragment HeroPreview() => @<div class="flex items-center justify-center rounded-lg border border-border-light bg-surface-50 p-8">');
  o.push('        <XFStack Orientation="XFOrientation.Vertical" Gap="4" Alignment="XFAlignment.Center">');
  o.push('            <XFTypography Variant="XFTypographyVariant.Body2" Color="XFTypographyColor.Muted">@T("Preview of ' + docName + ' component", "Preview of ' + docName + ' component")</XFTypography>');
  o.push("        </XFStack>");
  o.push("    </div>;");
  o.push("");
  o.push('    private RenderFragment ExBasic() => @<div class="flex flex-col gap-4"><XFStack Orientation="XFOrientation.Vertical" Gap="2"><XFTypography Variant="XFTypographyVariant.Caption" Color="XFTypographyColor.Muted">@T("Basic example", "Basic example")</XFTypography></XFStack></div>;');
  o.push('    private RenderFragment ExAdvanced() => @<div class="flex flex-col gap-4"><XFStack Orientation="XFOrientation.Vertical" Gap="2"><XFTypography Variant="XFTypographyVariant.Caption" Color="XFTypographyColor.Muted">@T("Advanced example", "Advanced example")</XFTypography></XFStack></div>;');
  o.push("");
  o.push('    private string CodeBasic => T("\\x3C' + compName + ' /\\x3E", "\\x3C' + compName + ' /\\x3E");');
  o.push('    private string CodeAdvanced => T("\\x3C' + compName + ' Variant=\\"\\" Color=\\"\\" /\\x3E", "\\x3C' + compName + ' Variant=\\"\\" Color=\\"\\" /\\x3E");');
  o.push("");
  o.push('    private RenderFragment Section(string title, RenderFragment content) => @<XFCard Variant="XFCardVariant.Outlined" Padding="XFPadding.Lg"><Header><XFTypography Variant="XFTypographyVariant.H4" Weight="XFTypographyWeight.Semibold">@title</XFTypography></Header><ChildContent>@content</ChildContent></XFCard>;');
  o.push('    private RenderFragment Example(string title, string description, RenderFragment preview, string codeText) => @<XFCard Variant="XFCardVariant.Outlined" Padding="XFPadding.None" AreaRestricted="false"><Header><div class="px-6 pt-6 pb-2"><XFTypography Variant="XFTypographyVariant.H4" Weight="XFTypographyWeight.Semibold">@title</XFTypography><XFTypography Variant="XFTypographyVariant.Body2" Color="XFTypographyColor.Muted">@description</XFTypography></div></Header><ChildContent><div class="flex flex-col"><div class="border-t border-border-light bg-surface-50/30 p-6">@preview</div><div class="border-t border-border-light bg-surface-50 p-6"><XFCode Text="@codeText" Language="razor" Title="@CopyableCodeTitle" Variant="XFCodeVariant.Block" Copyable="true" Wrap="true" /></div></div></ChildContent></XFCard>;');
  o.push('    private RenderFragment Tile(DocTile item) => @<XFCard Variant="XFCardVariant.Filled" Padding="XFPadding.Lg"><ChildContent><XFStack Orientation="XFOrientation.Vertical" Gap="3" FullWidth="true"><div class="flex items-center gap-4"><span class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-lg border border-primary/10 bg-primary/10 text-primary" aria-hidden="true">@((MarkupString)XFSvgIcons.MarkupWithSize(item.Icon, 18))</span><div class="min-w-0"><XFTypography Variant="XFTypographyVariant.Body1" Weight="XFTypographyWeight.Semibold">@item.Title</XFTypography></div></div><XFTypography Variant="XFTypographyVariant.Body2" Color="XFTypographyColor.Secondary">@item.Description</XFTypography></XFStack></ChildContent></XFCard>;');
  o.push('    private RenderFragment TileGrid(IReadOnlyList<DocTile> items, int cols) => @<XFGrid Cols="1" Md="@Math.Min(cols, 2)" Lg="@cols" Gap="4">@foreach (var item in items) { @Tile(item) }</XFGrid>;');
  o.push('    private RenderFragment WarningGrid(IReadOnlyList<DocTile> items) => @<XFGrid Cols="1" Md="2" Gap="4">@foreach (var item in items) { <XFCard Variant="XFCardVariant.Filled" Padding="XFPadding.Lg"><ChildContent><XFStack Orientation="XFOrientation.Vertical" Gap="3" FullWidth="true"><div class="flex items-center gap-4"><span class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-lg border border-danger/10 bg-danger/10 text-danger" aria-hidden="true">@((MarkupString)XFSvgIcons.MarkupWithSize(item.Icon, 18))</span><div class="min-w-0"><XFTypography Variant="XFTypographyVariant.Body1" Weight="XFTypographyWeight.Semibold">@item.Title</XFTypography></div></div><XFTypography Variant="XFTypographyVariant.Body2" Color="XFTypographyColor.Secondary">@item.Description</XFTypography></XFStack></ChildContent></XFCard> }</XFGrid>;');
  o.push('    private RenderFragment NameValueTable(IReadOnlyList<NameValueRow> rows, string h1, string h2) => @<XFTable Striped="true" Hoverable="true" Compact="true"><XFTableHeader><XFTableRow><XFTableCell Header="true">@h1</XFTableCell><XFTableCell Header="true">@h2</XFTableCell></XFTableRow></XFTableHeader><XFTableBody>@foreach (var r in rows) { <XFTableRow><XFTableCell><XFCode Text="@r.Name" /></XFTableCell><XFTableCell>@r.Value</XFTableCell></XFTableRow> }</XFTableBody></XFTable>;');
  o.push('    private RenderFragment ApiTable(IReadOnlyList<ApiRow> rows) => @<XFTable Striped="true" Hoverable="true" Compact="true"><XFTableHeader><XFTableRow><XFTableCell Header="true">@T("Parameter", "Parameter")</XFTableCell><XFTableCell Header="true">Type</XFTableCell><XFTableCell Header="true">Default</XFTableCell><XFTableCell Header="true">@T("Description", "Description")</XFTableCell></XFTableRow></XFTableHeader><XFTableBody>@foreach (var r in rows) { <XFTableRow><XFTableCell><XFCode Text="@r.Name" /></XFTableCell><XFTableCell>@r.Type</XFTableCell><XFTableCell><XFCode Text="@r.DefaultValue" /></XFTableCell><XFTableCell>@r.Description</XFTableCell></XFTableRow> }</XFTableBody></XFTable>;');
  o.push("");  o.push("    private IReadOnlyList<DocTile> QuickFacts => [");
  o.push('        new(T("Native semantics", "Native semantics"), T("Renders semantic HTML with accessibility.", "Renders semantic HTML with accessibility."), "code"),');
  o.push('        new(T("Enterprise-ready", "Enterprise-ready"), T("Enterprise patterns with i18n and themes.", "Enterprise patterns with i18n and themes."), "building"),');
  o.push('        new(T("Accessible", "Accessible"), T("Full WAI-ARIA and keyboard navigation support.", "Full WAI-ARIA and keyboard navigation support."), "universal-access"),');
  o.push('        new(T("Customizable", "Customizable"), T("Extensive parameters for customization.", "Extensive parameters for customization."), "palette")');
  o.push("    ];");
  o.push("    private IReadOnlyList<NameValueRow> MetadataRows => [");
  o.push('        new("Component", "' + compName + '"),');
  o.push('        new("Namespace", "' + actualNs + '"),');
  o.push('        new("Package", "XForge.DesignSystem"),');
  o.push('        new("Category", "Components / ' + category + '"),');
  o.push('        new("Status", T("Stable", "Stable")),');
  o.push('        new("Accessibility", T("aria-label, focus ring, keyboard navigation.", "aria-label, focus ring, keyboard navigation.")),');
  o.push('        new("Theme support", T("Light, dark, data-brand via tokens.", "Light, dark, data-brand via tokens."))');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> OverviewItems => [");
  o.push('        new(T("' + docName + ' Component", "' + docName + ' Component"), T("Component of ' + category + ' category.", "Component of ' + category + ' category."), "square"),');
  o.push('        new(T("Primary use", "Primary use"), T("User interaction with the interface.", "User interaction with the interface."), "hand-pointer"),');
  o.push('        new(T("Integration", "Integration"), T("Integrates with other XForge components.", "Integrates with other XForge components."), "puzzle-piece")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> PurposeItems => [");
  o.push('        new(T("Interface", "Interface"), T("Renders ' + docName + ' in the user interface.", "Renders ' + docName + ' in the user interface."), "window"),');
  o.push('        new(T("Interaction", "Interaction"), T("Allows user interaction with the system.", "Allows user interaction with the system."), "hand-pointer"),');
  o.push('        new(T("Data", "Data"), T("Displays and manipulates system data.", "Displays and manipulates system data."), "database")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> ProblemItems => [");
  o.push('        new(T("Before", "Before"), T("Non-standardized components without accessibility.", "Non-standardized components without accessibility."), "xmark"),');
  o.push('        new(T("After", "After"), T("Standardized, accessible and customizable component.", "Standardized, accessible and customizable component."), "check"),');
  o.push('        new(T("Reduced risk", "Reduced risk"), T("Accessibility and consistency ensured.", "Accessibility and consistency ensured."), "shield-check")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> UseItems => [");
  o.push('        new(T("Forms", "Forms"), T("Use ' + docName + ' in forms and controls.", "Use ' + docName + ' in forms and controls."), "clipboard-list"),');
  o.push('        new(T("Data", "Data"), T("Use ' + docName + ' to display data.", "Use ' + docName + ' to display data."), "database")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> AvoidItems => [");
  o.push('        new(T("Incorrect use", "Incorrect use"), T("Do not use ' + docName + ' for unrelated purposes.", "Do not use ' + docName + ' for unrelated purposes."), "xmark"),');
  o.push('        new(T("Accessibility", "Accessibility"), T("Always ensure accessibility when using ' + docName + '.", "Always ensure accessibility when using ' + docName + '."), "triangle-exclamation")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> AnatomyItems => [");
  o.push('        new(T("Container", "Container"), T("Root element of ' + docName + '.", "Root element of ' + docName + '."), "square"),');
  o.push('        new(T("Content", "Content"), T("Internal content of the component.", "Internal content of the component."), "layer-group"),');
  o.push('        new(T("Visual state", "Visual state"), T("States: disabled, active, loading.", "States: disabled, active, loading."), "eye")');
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> DesignPrincipleItems => [");
  o.push('        new(T("Clarity", "Clarity"), T("Clear and intuitive interface.", "Clear and intuitive interface."), "eye"),');
  o.push('        new(T("Consistency", "Consistency"), T("Consistent patterns with other components.", "Consistent patterns with other components."), "layer-group"),');
  o.push('        new(T("Accessibility", "Accessibility"), T("Full accessibility support.", "Full accessibility support."), "universal-access")');
  o.push("    ];");
  o.push("    private IReadOnlyList<ApiRow> ApiRows => [");
  o.push("        " + paramRows);
  o.push("    ];");
  o.push("    private IReadOnlyList<DocTile> AccessibilityItems => [");
  o.push('        new("aria-label", T("Accessible label for screen readers.", "Accessible label for screen readers."), "universal-access"),');
  o.push('        new("focus-visible", T("Visible focus ring for keyboard navigation.", "Visible focus ring for keyboard navigation."), "eye")');
  o.push("    ];");
  o.push("    private IReadOnlyList<NameValueRow> KeyboardRows => [");
  o.push('        new("Tab", T("Moves focus to component.", "Moves focus to component.")),');
  o.push('        new("Enter", T("Activates component.", "Activates component.")),');
  o.push('        new("Space", T("Activates component.", "Activates component.")),');
  o.push('        new("Escape", T("Removes focus from component.", "Removes focus from component."))');
  o.push("    ];");
  o.push("    private IReadOnlyList<NameValueRow> FaqRows => [");
  o.push('        new(T("How to use ' + docName + '?", "How to use ' + docName + '?"), T("Add the component to your Razor page.", "Add the component to your Razor page.")),');
  o.push('        new(T("Is it accessible?", "Is it accessible?"), T("Yes, follows WAI-ARIA guidelines.", "Yes, follows WAI-ARIA guidelines."))');
  o.push("    ];");
  o.push("    private IReadOnlyList<NameValueRow> ChangelogRows => [");
  o.push('        new("' + TODAY + '", T("Initial documentation generated.", "Initial documentation generated."))');
  o.push("    ];");
  o.push("");
  o.push("    private sealed record DocTile(string Title, string Description, string Icon);");
  o.push("    private sealed record NameValueRow(string Name, string Value);");
  o.push("    private sealed record ApiRow(string Name, string Type, string DefaultValue, string Description);");
  o.push("}");
  return o.join("\n");
}

function main() {
  var razorFiles = walkDir(COMPONENTS_ROOT, ".razor");
  var existingDocs = getExistingDocs();
  var generated = [];
  var skipped = [];

  for (var i = 0; i < razorFiles.length; i++) {
    var razorPath = razorFiles[i];
    var compName = path.basename(razorPath, ".razor");
    if (SKIP.has(compName)) { skipped.push(compName + " (skip)"); continue; }

    var docName = getDocName(compName);
    if (existingDocs.has(docName)) { skipped.push(compName + " (exists)"); continue; }

    var relativePath = path.relative(COMPONENTS_ROOT, path.dirname(razorPath));
    var category = getCat(relativePath);

    var csPath = razorPath + ".cs";
    var ns = null;
    var params = [];

    if (fs.existsSync(csPath)) {
      var csContent = fs.readFileSync(csPath, "utf-8");
      ns = extractNs(csContent);
      params = extractParams(csContent);
    }

    if (!ns || !params.length) {
      var razorContent = fs.readFileSync(razorPath, "utf-8");
      if (!ns) ns = extractNsR(razorContent);
      if (!params.length) {
        var rp = extractParams(razorContent);
        if (rp.length) params = rp;
      }
    }

    if (!ns) {
      var parts = relativePath.split(path.sep).filter(Boolean);
      parts.unshift("XForge.DesignSystem.Components");
      ns = parts.join(".");
    }

    var fullDoc = generateDoc(compName, ns, category, params) + generateCodeBlock(compName, docName, category, ns, buildParamRows(params));
    var docFileName = docName + "Docs.razor";
    var docFilePath = path.join(DOCS_ROOT, docFileName);
    fs.writeFileSync(docFilePath, fullDoc, "utf-8");
    generated.push(compName);
    console.log("  Generated: " + docFileName);
  }

  console.log("");
  console.log("=== gen_docs Summary ===");
  console.log("  Components scanned: " + razorFiles.length);
  console.log("  Docs generated: " + generated.length);
  console.log("  Skipped: " + skipped.length);
  if (generated.length > 0) {
    console.log("");
    console.log("Generated docs:");
    for (var j = 0; j < generated.length; j++) {
      console.log("  - " + getDocName(generated[j]) + "Docs.razor");
    }
  }
}

main();