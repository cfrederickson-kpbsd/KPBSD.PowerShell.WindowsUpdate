# Build script files into module in $PSScriptRoot\KPBSD.PowerShell.WindowsUpdate\{version}
# Include type and format files

#Requires -Version 7.2.0
#Requires -Module PSSharp.ModuleFactory
#Requires -Module PlatyPS

[CmdletBinding()]
param(
    [version]
    $Version = '1.0.0'
)

$ModuleName = 'KPBSD.PowerShell.WindowsUpdate'
$SourceDirectory = Join-Path $PSScriptRoot '..' 'src' | Resolve-Path | Select-Object -ExpandProperty Path
$PSSourceFiles = Get-ChildItem -Path $SourceDirectory -Include '*.ps1' -Recurse
$ManifestTemplatePath = Join-Path $PSScriptRoot 'KPBSD.PowerShell.WindowsUpdate.psd1'
[string[]]$PublicFunctions = $PSSourceFiles | Where-Object FullName -like *Public* | Select-Object -ExpandProperty BaseName
[string]$OutputDirectory = Join-Path $PSScriptRoot 'KPBSD.PowerShell.WindowsUpdate' $Version
[string]$OutputScriptModule = Join-Path $OutputDirectory "$ModuleName.psm1"

Import-Module 'PSSharp.ModuleFactory'
if (Test-Path $OutputDirectory) {
    Remove-Item $OutputDirectory -Force -Recurse -ErrorAction Stop
}
New-Item -ItemType Directory -Path $OutputDirectory
$Manifest = Import-PowerShellDataFile -Path $ManifestTemplatePath
foreach ($key in $Manifest['PrivateData']['PSData'].Keys) {
    $Manifest[$Key] = $Manifest['PrivateData']['PSData'][$Key]
}
$Manifest.Remove('PrivateData')

# Build PowerShell module output
$ScriptModuleData = $PSSourceFiles | Build-ScriptModule -OutputPath $OutputScriptModule -Force
$PublicAliases = @()
foreach ($ScriptFunction in $ScriptModuleData.Aliases.Keys) {
    if ($PublicFunctions -Contains $ScriptFunction) {
        $PublicAliases += $ScriptModuleData.Aliases[$ScriptFunction]
    }
}
$Manifest['FunctionsToExport'] = $PublicFunctions
$Manifest['AliasesToExport'] = $PublicAliases
$Manifest['VariablesToExport'] = @()

# Build C# module output
$ResolvedDotNetDestination = Resolve-Path $OutputDirectory
$ResolvedDotNetSource = Resolve-Path (Join-Path $SourceDirectory 'KPBSD.PowerShell.WindowsUpdate')
dotnet publish $ResolvedDotNetSource.ProviderPath --output $ResolvedDotNetDestination.ProviderPath  --configuration release --os Windows | Write-Debug
# No cmdlets exported
$Manifest['CmdletsToExport'] = @()

# Build module documentation output
$DocumentationSourceDirectory = Join-Path $SourceDirectory 'Documentation'
New-ExternalHelp -Path $DocumentationSourceDirectory -OutputPath $OutputDirectory

# Export type and format files
Get-ChildItem -Path $SourceDirectory -Recurse -Include '*.*.ps1xml' | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination (Join-Path $OutputDirectory $_.Name)
}

# Create module manifest
Push-Location $OutputDirectory -StackName 'GetOutputFiles'
$OutputFiles = Get-ChildItem -Path $OutputDirectory | Resolve-Path -Relative
Pop-Location -StackName 'GetOutputFiles'
$Manifest['Path'] = Join-Path $OutputDirectory 'KPBSD.PowerShell.WindowsUpdate.psd1'
$Manifest['Nested'] = $OutputFiles | Where-Object { $_ -like '*.dll' -or $_ -like '*.psm1' }
$Manifest['ModuleVersion'] = $Version
$Manifest['RequiredAssemblies'] = $OutputFiles | Where-Object { $_ -like '*.dll' }
$Manifest['TypesToProcess'] = $OutputFiles | Where-Object { $_ -like '*.types.ps1xml' }
$Manifest['FormatsToProcess'] = $OutputFiles | Where-Object { $_ -like '*.formats.ps1xml' }
$Manifest['FileList'] = $OutputFiles
New-ModuleManifest @Manifest