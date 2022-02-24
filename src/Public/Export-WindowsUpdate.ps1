function Export-WindowsUpdate {
    [Alias('epwu')]
    [OutputType([System.IO.FileInfo])]
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline)]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel]
        $WindowsUpdate,

        [Parameter(Mandatory, Position = 1, ValueFromPipelineByPropertyName)]
        [string]
        $OutputPath,

        [Parameter()]
        [switch]
        $NoClobber,

        [Parameter()]
        [switch]
        $PassThru
    )
    process {
        if ($NoClobber -and (Test-Path $OutputPath)) {
            $ex = [System.InvalidOperationException]::new('A file exists at the export destination path.')
            $er = [System.Management.Automation.ErrorRecord]::new(
                $ex,
                'FileExists',
                [System.Management.Automation.ErrorCategory]::ResourceExists,
                $OutputPath
            )
            $er.ErrorDetails = "The Windows Update $WindowsUpdate cannot be exported because the -NoClobber parameter was set and a file exists at the export destination path '$OutputPath'."
            $er.ErrorDetails.RecommendedAction = 'Remove the existing item or run the command again without the -NoClobber parameter.'
            $PSCmdlet.WriteError($er)
            return
        }
        $DirectoryPath = Split-Path $OutputPath -Parent
        $DestinationFileName = Split-Path $OutputPath -Leaf
        $CreatedDirectory = $false
        if (-not (Test-Path $DirectoryPath)) {
            $CreatedDirectory = $true
            New-Item -Path $DirectoryPath -ItemType Directory -ErrorAction Stop -Confirm:$false -WhatIf:$false
        }

        $ResolvedDirectoryPath = Resolve-Path $DirectoryPath
        $ResolvedDestinationPath = Join-Path $ResolvedDirectoryPath.ProviderPath -ChildPath $DestinationFileName

        if ($PSCmdlet.ShouldProcess(
            "Exporting the update $WindowsUpdate to '$ResolvedDestinationPath'.",
            "Export the update $WindowsUpdate to '$ResolvedDestinationPath'?",
            "Confirm: export Windows Update"
        ))
        {
            try {
                $WindowsUpdate.ComObject.CopyFromCache($ResolvedDestinationPath, $false)
                Get-Item -LiteralPath $ResolvedDestinationPath | Where-Object { $PassThru }
            }
            catch [System.Runtime.InteropServices.COMException] {
                $er = New-ComErrorRecord -HResult $_.Exception.ErrorCode -Exception $_.Exception -TargetObject $WindowsUpdate
                $PSCmdlet.WriteError($er)
            }
        }
        else
        {
            if ($CreatedDirectory)
            {
                Remove-Item -Path $DirectoryPath -ErrorAction SilentlyContinue
            }
        }
    }
}