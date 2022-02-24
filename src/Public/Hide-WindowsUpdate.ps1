function Hide-WindowsUpdate {
    [Alias('hwu')]
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
        Assert-RunAsAdmin -Operation 'hide Windows Update' | Out-Null
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
            if ($UpdateToHide.IsMandatory) {
                $ex = [System.InvalidOperationException]::new('Mandatory updates cannot be hidden. The Windows Update is mandatory.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'MandatoryUpdate',
                    [System.Management.Automation.ErrorCategory]::InvalidOperation,
                    $UpdateToHide
                )
                $er.ErrorDetails = "Mandatory updates cannot be hidden. The Windows Update $UpdateToHide is mandatory."
                $PSCmdlet.WriteError($er)
            }
            elseif ($UpdateToHide.IsHidden) {
                $ex = [System.InvalidOperationException]::new('The Windows Update is already hidden.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'AlreadyHidden',
                    [System.Management.Automation.ErrorCategory]::InvalidOperation,
                    $UpdateToHide
                )
                $er.ErrorDetails = "The Windows Update $UpdateToHide is already hidden."
                $er.ErrorDetails.RecommendedAction = 'The update is currently in the expected state. However, to successfully run this operation first unhide the Windows Update using the ''Show-WindowsUpdate'' command.'
                $PSCmdlet.WriteError($er)
            }
            elseif ($PSCmdlet.ShouldProcess(
                "Hiding Windows Update $UpdateToHide.",
                "Hide Windows Update $UpdateToHide`?",
                'Confirm: hide Windows Update'))
            {
                try {
                    $UpdateToHide.IsHidden = $true
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