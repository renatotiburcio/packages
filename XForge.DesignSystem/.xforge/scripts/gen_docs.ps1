$docs = Get-ChildItem -Path "src\XForge.DesignSystem.Docs\Components\Pages" -Filter "*Docs.razor" -File | ForEach-Object { $_.BaseName -replace "Docs$","" }
$comps = Get-ChildItem -Path "src\XForge.DesignSystem\Components" -Filter "*.razor" -Recurse -File | ForEach-Object { $_.Name -replace "\.razor$","" }
$missing = @()
foreach ($c in $comps) {
  $name = $c -replace "^XF",""
  if ($name -notin $docs) { $missing += "$c|$name" }
}
$outDir = "src\XForge.DesignSystem.Docs\Components\Pages"
$created = 0
foreach ($m in $missing) {
  $parts = $m -split "\|"
  $comp = $parts[0]
  $docName = $parts[1]
  $outFile = Join-Path $outDir "${docName}Docs.razor"
  if (Test-Path $outFile) { continue }
  $razorFile = Get-ChildItem -Path "src\XForge.DesignSystem\Components" -Filter "$comp.razor" -Recurse -File | Select-Object -First 1
  $ns = "UNKNOWN"
  $category = "General"
  $paramRows = @()
  if ($razorFile) {
    $folderPath = $razorFile.Directory.FullName
    $relativePath = $folderPath.Replace("src\XForge.DesignSystem\Components\", "")
    $category = $relativePath.Split("\")[-1]
    $csFile = $razorFile.FullName -replace "\.razor$",".razor.cs"
    if (Test-Path $csFile) {
      $content = Get-Content $csFile -Raw
      $nsMatch = [regex]::Match($content, "namespace\s+([\w\.]+)")
      if ($nsMatch.Success) { $ns = $nsMatch.Groups[1].Value }
      $params = [regex]::Matches($content, "\[Parameter\]\s+public\s+(\S+)\s+(\S+)")
      foreach ($p in $params) {
        $type = $p.Groups[1].Value
        $pname = $p.Groups[2].Value
        $paramRows += "        new(\""$pname\"", \""$type\"", \""""\", T(\""Parametro $pname do componente $comp.\""$pname parameter of $comp component.\"")),"
      }
    }
  }
  $paramSection = if ($paramRows.Count -gt 0) { ($paramRows -join "`n") } else { "" }
  $docLower = $docName.ToLower()
  $razor = "@page ""/components/$docLower""`n@using XForge.DesignSystem.Core.Primitives`n@using $NS`n@inject XForgeDocsLanguageService LanguageService`n<XForgeDocsShell ProductLabel=""@ProductLabelText"" Title=""@PageTitleText"" Status=""@PageStatusText"" Items=""XForgeDocsCatalog.CoreItems"">`n    <XFStack Orientation=""XFOrientation.Vertical"" Gap=""6"" FullWidth=""true"">`n        <XFCard Variant=""XFCardVariant.Outlined"" Padding=""XFPadding.Xl"" AreaRestricted=""false"">`n            <ChildContent>`n                <XFStack Orientation=""XFOrientation.Vertical"" Gap=""5"" FullWidth=""true"">`n                    <XFStack Orientation=""XFOrientation.Horizontal"" Gap=""2"" Wrap=""XFWrap.Wrap"">`n                        <XFBadge Variant=""XFBadgeVariant.Solid"" Color=""XFBadgeColor.Primary"" Text=""$comp"" />`n                        <XFBadge Variant=""XFBadgeVariant.Soft"" Color=""XFBadgeColor.Success"" Text=""WAI-ARIA"" />`n                        <XFBadge Variant=""XFBadgeVariant.Soft"" Color=""XFBadgeColor.Info"" Text=""@T("""Enterprise"""", """Enterprise""")"" />`n                    </XFStack>`n                    <XFStack Orientation=""XFOrientation.Vertical"" Gap=""2"" FullWidth=""true"">`n                        <XFTypography Variant=""XFTypographyVariant.H2"" Weight=""XFTypographyWeight.Bold"">`n                            @T("""Componente $docName do XForge Design System."""", """$docName component of XForge Design System.""")`n                        </XFTypography>`n                        <XFTypography Variant=""XFTypographyVariant.Body1"" Color=""XFTypographyColor.Secondary"">`n                            @T("""Descricao do componente $docName."""", ""Description of $docName component."")`n                        </XFTypography>`n                    </XFStack>`n                </XFStack>`n            </ChildContent>`n        </XFCard>`n    </XFStack>`n</XForgeDocsShell>`n@code {`n    private string ProductLabelText => ""$comp"";`n    private string PageTitleText => ""$docName"";`n    private string PageStatusText => T(""Estavel"", ""Stable"");`n    private string CodeBasic => T(""\\x3C$comp /\\x3E"", ""\\x3C$comp /\\x3E"");`n    private string CodeAdvanced => T(""\\x3C$comp Variant="""""""" Size="""""""" /\\x3E"", ""\\x3C$comp Variant="""""""" Size="""""""" /\\x3E"");`n$paramSection`n}"
  Set-Content -Path $outFile -Value $razor -Encoding UTF8
  $created++
}
Write-Output "Created: $created"
