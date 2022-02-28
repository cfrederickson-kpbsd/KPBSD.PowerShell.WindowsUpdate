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
$CSharpFilesToLoad = Get-ChildItem -Path "$SourceDir/KPBSD.PowerShell.WindowsUpdate" -Include '*.cs' -Recurse | Where-Object { $_.FullName -notmatch 'bin|obj'}

# Load the module entirely in-memory so that we don't have to leave the file during our session
$TempAssemblyPath = Join-Path ([System.IO.Path]::GetTempPath()) 'KPBSD.PowerShell.WindowsUpdate.dll'
if (Test-Path $TempAssemblyPath) { Remove-Item $TempAssemblyPath -Force -ErrorAction Stop }
Add-Type -Path $CSharpFilesToLoad -IgnoreWarnings -OutputAssembly $TempAssemblyPath
$AssemblyData = [System.IO.File]::ReadAllBytes($TempAssemblyPath)
$Assembly = [System.Reflection.Assembly]::Load($AssemblyData)
Remove-Item -Path $TempAssemblyPath
Import-Module -Assembly $Assembly
# Import-Module $TempAssemblyPath

foreach ($file in $ScriptFilesToImport) {
    . $file.FullName
}
Update-TypeData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.types.ps1xml')
Update-FormatData -PrependPath (Join-Path $SourceDir 'KPBSD.PowerShell.WindowsUpdate.formats.ps1xml')

if ($ExportAllFunctions) {
}
else {
    $PublicFunctions = $ScriptFilesToImport | Where-Object 'FullName' -like '*Public*' | Select-Object -ExpandProperty BaseName
    $PublicAliases = Get-Alias -Definition $PublicFunctions -ErrorAction Ignore | Select-Object -ExpandProperty Name
    Export-ModuleMember -Function $PublicFunctions -Alias $PublicAliases
}