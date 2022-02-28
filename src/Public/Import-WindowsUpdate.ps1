function Import-WindowsUpdate
{
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel])]
    [Alias('ipwu')]
    [CmdletBinding(SupportsShouldProcess, DefaultParameterSetName = 'PathSet')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'PathSet', Position = 0)]
        [Alias('FilePath')]
        [string]
        $Path,

        [Parameter(Mandatory, ParameterSetName = 'LiteralSet', ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string]
        $LiteralPath,

        [Parameter(Mandatory, ValueFromPipeline)]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel]
        $WindowsUpdate,

        [Parameter()]
        [switch]
        $PassThru
    )
    process {
        [System.Management.Automation.PathInfo]$ResolvedPath = $null
        if ($Path) {
            [System.Management.Automation.PathInfo]$ResolvedPath = Resolve-Path -Path $Path
            if (!$?) { return }
        }
        else {
            [System.Management.Automation.PathInfo]$ResolvedPath = Resolve-Path -LiteralPath $LiteralPath
            if (!$?) { return }
        }
        $AllContents = @(Get-ChildItem -LiteralPath $ResolvedPath.ProviderPath -Recurse)
        $StringCollection = New-Object -ComObject Microsoft.Update.StringColl
        foreach ($File in $AllContents)
        {
            if (!$File.PSIsContainer) {
                [void]$StringCollection.Add($File.FullName)
            }
        }
        try {
            if ($PSCmdlet.ShouldProcess(
                "Importing contents from $($StringCollection.Count) file(s) under '$ResolvedPath' into Windows Update $WindowsUpdate.",
                "Import contents from $($StringCollection.Count) file(s) under '$ResolvedPath' into Windows Update $WindowsUpdate?",
                'Confirm: import Windows Update'
            ))
            {
                $WindowsUpdate.ComObject.CopyToCache($StringCollection)
                if ($? -and $PassThru) {
                    $WindowsUpdate
                }
            }
        }
        catch [System.Runtime.InteropServices.COMException] {
            $er = New-ComErrorRecord -HResult $_.Exception.ErrorCode -Exception $_.Exception -TargetObject $WindowsUpdate
            $PSCmdlet.WriteError($er)
        }
    }
}