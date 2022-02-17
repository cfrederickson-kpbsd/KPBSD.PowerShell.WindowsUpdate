# Import the module components separately, export the appropriate parts.
# For testing and debugging before compiling the module scripts.

param(
    [Parameter()]
    [switch]
    $ExportAllFunctions
)

$ProjectDir = Split-Path $PSScriptRoot -Parent
$SourceDir = Join-Path $ProjectDir 'src'
$ScriptFilesToImport = Get-ChildItem -Path $SourceDir -Include '*.ps1' -Recurse

foreach ($file in $ScriptFilesToImport) {
    . $file.FullName
}
#Update-TypeData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.types.ps1xml')
#Update-FormatData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.formats.ps1xml')

if ($ExportAllFunctions) {
}
else {
    $PublicFunctions = $ScriptFilesToImport | Where-Object 'Path' -like '*Public*' | Select-Object -ExpandProperty BaseName
    Export-ModuleMember -Function $PublicFunctions
}