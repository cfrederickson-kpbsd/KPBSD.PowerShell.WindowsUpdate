# Import the module components separately, export the appropriate parts.
# For testing and debugging before compiling the module scripts.

param(
    [Parameter()]
    [switch]
    $ExportAllFunctions
)

$ProjectDir = Split-Path $PSScriptRoot -Parent | Split-Path -Parent
$SourceDir = Join-Path $ProjectDir 'src'
$ScriptFilesToImport = Get-ChildItem -Path $SourceDir -Include '*.ps1' -Recurse
$CSharpFilesToLoad = Get-ChildItem -Path "$SourceDir/KPBSD.PowerShell.WindowsUpdate" -Include '*.cs' -Recurse

$TempAssemblyPathBase = Join-Path ([System.IO.Path]::GetTempPath()) 'KPBSD.PowerShell.WindowsUpdate'
# Remove any previous iterations of the assembly that are no longer in use
Remove-Item -Path "$TempAssemblyPathBase*.dll" -ErrorAction Ignore
$TempAssemblyPath = "$TempAssemblyPathBase.dll"
[int]$TempAssemblyPathId = 0
while (Test-Path $TempAssemblyPath) {
    $TempAssemblyPathId++
    $TempAssemblyPath = "$TempAssemblyPathBase$TempAssemblyPathId.dll"
}
Add-Type -Path $CSharpFilesToLoad -IgnoreWarnings -OutputAssembly $TempAssemblyPath
Import-Module $TempAssemblyPath

foreach ($file in $ScriptFilesToImport) {
    . $file.FullName
}
Update-TypeData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.types.ps1xml')
Update-FormatData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.formats.ps1xml')

if ($ExportAllFunctions) {
}
else {
    $PublicFunctions = $ScriptFilesToImport | Where-Object 'FullName' -like '*Public*' | Select-Object -ExpandProperty BaseName
    Export-ModuleMember -Function $PublicFunctions
}