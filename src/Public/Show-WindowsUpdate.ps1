function Show-WindowsUpdate {
    [Alias('shwu')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel])]
    [CmdletBinding(SupportsShouldProcess, DefaultParameterSetName = 'TitleSet')]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'TitleSet')]
        [SupportsWildcards()]
        [string[]]
        $Title,

        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'IdSet')]
        [Guid[]]
        $UpdateId,

        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ParameterSetName = 'UpdateSet')]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel[]]
        $WindowsUpdate,

        [Parameter()]
        [switch]
        $PassThru
    )
    begin {
        Assert-RunAsAdmin -Operation 'show Windows Update' -AllowPowerUser | Out-Null
    }
    process {
        if ($PSCmdlet.ParameterSetName -eq 'TitleSet') {
            [object[]]$Updates = Get-WindowsUpdate -Title $Title -IncludeHidden -IncludeInstalled
        }
        elseif ($PSCmdlet.ParameterSetName -eq 'IdSet') {
            [object[]]$Updates = Get-WindowsUpdate -Title $Title -IncludeHidden -IncludeInstalled
        }
        else {
            [object[]]$Updates = $WindowsUpdate
        }
        foreach ($UpdateToHide in $Updates) {
            if (!$UpdateToHide.IsHidden) {
                $ex = [System.InvalidOperationException]::new('The Windows Update is already visible.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'AlreadyVisible',
                    [System.Management.Automation.ErrorCategory]::InvalidOperation,
                    $UpdateToHide
                )
                $er.ErrorDetails = "The Windows Update $UpdateToHide is already visible."
                $er.ErrorDetails.RecommendedAction = 'The update is currently in the expected state. However, to successfully run this operation first hide the Windows Update using the ''Hide-WindowsUpdate'' command.'
                $PSCmdlet.WriteError($er)
            }
            elseif ($PSCmdlet.ShouldProcess(
                "Un-hiding Windows Update $UpdateToHide.",
                "Un-hide Windows Update $UpdateToHide`?",
                'Confirm: reveal Windows Update'))
            {
                try {
                    $UpdateToHide.IsHidden = $false
                    if ($PassThru) {
                        $UpdateToHide
                    }
                }
                catch [System.Runtime.InteropServices.COMException] {
                    $er = New-ComErrorRecord -ErrorCode $_.Exception.ErrorCode -Exception $_.Exception -TargetObject $UpdateToHide
                    $PSCmdlet.WriteError($er)
                }
            }
        }
    }
}